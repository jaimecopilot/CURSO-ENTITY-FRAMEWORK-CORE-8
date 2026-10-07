$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$Workspace = Join-Path $env:RUNNER_TEMP "aceria-m5-5-8-team"
if ([string]::IsNullOrWhiteSpace($env:RUNNER_TEMP)) {
    $Workspace = Join-Path ([System.IO.Path]::GetTempPath()) "aceria-m5-5-8-team"
}

$Server = "(localdb)\MSSQLLocalDB"
$Database = "AceriaDB_M5_8_Team"
$Connection = "Server=(localdb)\MSSQLLocalDB;Database=$Database;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;Connect Timeout=30;"

function Assert-Exit([string]$Message) {
    if ($LASTEXITCODE -ne 0) { throw "$Message (exit code $LASTEXITCODE)" }
}

function Copy-Baseline([string]$Destination) {
    New-Item -ItemType Directory -Force -Path $Destination | Out-Null
    Get-ChildItem $Root -Force | Where-Object { $_.Name -notin @("bin","obj") } | ForEach-Object {
        Copy-Item $_.FullName -Destination $Destination -Recurse -Force
    }
}

function Add-TeamProperty([string]$ProjectRoot, [string]$PropertyName) {
    $entities = Join-Path $ProjectRoot "src/AceriaData.Domain/Entities.cs"
    $content = Get-Content $entities -Raw
    $anchor = '    public string? Observaciones { get; set; }'
    if ($content -notmatch [regex]::Escape($anchor)) { throw "No se encontro el punto de insercion en Entities.cs" }
    $replacement = $anchor + [Environment]::NewLine + "    public string? $PropertyName { get; set; }"
    $content = $content.Replace($anchor, $replacement)
    Set-Content -Path $entities -Value $content -Encoding utf8
}

function Add-Migration([string]$ProjectRoot, [string]$Name) {
    Push-Location $ProjectRoot
    try {
        dotnet ef migrations add $Name --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
        Assert-Exit "No se pudo generar $Name"
    }
    finally { Pop-Location }
}

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw "sqlcmd debe estar disponible para verificar el esquema final en SQL Server LocalDB."
}

if (Test-Path $Workspace) { Remove-Item $Workspace -Recurse -Force }
New-Item -ItemType Directory -Force -Path $Workspace | Out-Null

$BranchA = Join-Path $Workspace "branch-a"
$BranchB = Join-Path $Workspace "branch-b-parallel"
$Merged = Join-Path $Workspace "merged-correct"

Write-Host "== Rama A desde baseline =="
Copy-Baseline $BranchA
Add-TeamProperty $BranchA "EquipoRevisionA"
Add-Migration $BranchA "M5_5_8_TeamA"

Write-Host "== Rama B paralela desde el mismo baseline =="
Copy-Baseline $BranchB
Add-TeamProperty $BranchB "EquipoRevisionB"
Add-Migration $BranchB "M5_5_8_TeamBParallel"

$designerB = Get-ChildItem (Join-Path $BranchB "src/AceriaData.Infrastructure/Migrations") -Filter "*_M5_5_8_TeamBParallel.Designer.cs" | Select-Object -First 1
if ($null -eq $designerB) { throw "No se encontro el designer de la migracion paralela B" }
$parallelMetadata = Get-Content $designerB.FullName -Raw
if ($parallelMetadata -match "EquipoRevisionA") {
    throw "La rama B paralela no deberia conocer el cambio A"
}
if ($parallelMetadata -notmatch "EquipoRevisionB") {
    throw "La rama B paralela no contiene su propio cambio"
}
Write-Host "La migracion B paralela desconoce A: renombrarla no fusionaria sus metadatos."

Write-Host "== Estado fusionado correcto: incorporar A y regenerar B =="
Copy-Item $BranchA -Destination $Merged -Recurse -Force
$nested = Join-Path $Merged (Split-Path $BranchA -Leaf)
if (Test-Path $nested) {
    $temp = Join-Path $Workspace "merged-flat"
    Move-Item $nested $temp
    Remove-Item $Merged -Recurse -Force
    Move-Item $temp $Merged
}
Add-TeamProperty $Merged "EquipoRevisionB"
Add-Migration $Merged "M5_5_8_TeamBRegenerated"

$designerRegenerated = Get-ChildItem (Join-Path $Merged "src/AceriaData.Infrastructure/Migrations") -Filter "*_M5_5_8_TeamBRegenerated.Designer.cs" | Select-Object -First 1
if ($null -eq $designerRegenerated) { throw "No se encontro el designer regenerado B" }
$mergedMetadata = Get-Content $designerRegenerated.FullName -Raw
if ($mergedMetadata -notmatch "EquipoRevisionA" -or $mergedMetadata -notmatch "EquipoRevisionB") {
    throw "La migracion regenerada no representa el modelo fusionado A+B"
}

Push-Location $Merged
try {
    dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
    Assert-Exit "El modelo fusionado deja cambios pendientes"

    & sqlcmd -S $Server -d master -E -I -b -Q "IF DB_ID(N'$Database') IS NOT NULL BEGIN ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$Database]; END;"
    Assert-Exit "No se pudo limpiar la base aislada"

    dotnet ef database update --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release --connection $Connection
    Assert-Exit "No se pudo aplicar la cadena fusionada"
}
finally { Pop-Location }

& sqlcmd -S $Server -d $Database -E -I -b -Q @"
SET NOCOUNT ON;
IF COL_LENGTH(N'dbo.OrdenesFabricacion', N'EquipoRevisionA') IS NULL
    THROW 51001, 'Falta EquipoRevisionA en dbo.OrdenesFabricacion', 1;
IF COL_LENGTH(N'dbo.OrdenesFabricacion', N'EquipoRevisionB') IS NULL
    THROW 51002, 'Falta EquipoRevisionB en dbo.OrdenesFabricacion', 1;
SELECT c.name AS ColumnaEquipo
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE s.name = N'dbo'
  AND t.name = N'OrdenesFabricacion'
  AND c.name IN (N'EquipoRevisionA', N'EquipoRevisionB')
ORDER BY c.name;
"@
Assert-Exit "El esquema final no contiene ambas columnas de las ramas"

& sqlcmd -S $Server -d $Database -E -I -b -Q @"
SET NOCOUNT ON;
IF NOT EXISTS (
    SELECT 1 FROM dbo.__EFMigrationsHistory
    WHERE MigrationId LIKE N'%M5_5_8_TeamA'
)
    THROW 51003, 'Falta M5_5_8_TeamA en __EFMigrationsHistory', 1;
IF NOT EXISTS (
    SELECT 1 FROM dbo.__EFMigrationsHistory
    WHERE MigrationId LIKE N'%M5_5_8_TeamBRegenerated'
)
    THROW 51004, 'Falta M5_5_8_TeamBRegenerated en __EFMigrationsHistory', 1;
SELECT MigrationId
FROM dbo.__EFMigrationsHistory
WHERE MigrationId LIKE N'%M5_5_8_Team%'
ORDER BY MigrationId;
"@
Assert-Exit "El historial no contiene las dos migraciones de la resolucion correcta"

Write-Host "Designer paralelo B contiene A: False"
Write-Host "Designer regenerado contiene A+B: True"
Write-Host "Columnas A+B verificadas directamente por SQL Server: True"
Write-Host "Migraciones de equipo verificadas en __EFMigrationsHistory: True"
Write-Host "5.8 EQUIPOS OK"

<#
# RETO M05 5.8 - TERCERA RAMA C
$BranchC = Join-Path $Workspace "branch-c-parallel"
$MergedABC = Join-Path $Workspace "merged-abc-correct"

Write-Host "== Reto: Rama C paralela desde el mismo baseline =="
Copy-Baseline $BranchC
Add-TeamProperty $BranchC "EquipoRevisionC"
Add-Migration $BranchC "M5_5_8_TeamCParallel"

$designerC = Get-ChildItem (Join-Path $BranchC "src/AceriaData.Infrastructure/Migrations") -Filter "*_M5_5_8_TeamCParallel.Designer.cs" | Select-Object -First 1
if ($null -eq $designerC) { throw "Reto 5.8: no se encontro el designer paralelo C" }
$parallelMetadataC = Get-Content $designerC.FullName -Raw
if ($parallelMetadataC -match "EquipoRevisionA" -or $parallelMetadataC -match "EquipoRevisionB") {
    throw "Reto 5.8: la rama C paralela no deberia conocer A ni B"
}
if ($parallelMetadataC -notmatch "EquipoRevisionC") {
    throw "Reto 5.8: la rama C paralela no contiene su propio cambio"
}

Write-Host "== Reto: integrar C despues de A y B regenerada =="
Copy-Item $Merged -Destination $MergedABC -Recurse -Force
$nestedABC = Join-Path $MergedABC (Split-Path $Merged -Leaf)
if (Test-Path $nestedABC) {
    $tempABC = Join-Path $Workspace "merged-abc-flat"
    Move-Item $nestedABC $tempABC
    Remove-Item $MergedABC -Recurse -Force
    Move-Item $tempABC $MergedABC
}

Add-TeamProperty $MergedABC "EquipoRevisionC"
Add-Migration $MergedABC "M5_5_8_TeamCRegenerated"

$designerCRegenerated = Get-ChildItem (Join-Path $MergedABC "src/AceriaData.Infrastructure/Migrations") -Filter "*_M5_5_8_TeamCRegenerated.Designer.cs" | Select-Object -First 1
if ($null -eq $designerCRegenerated) { throw "Reto 5.8: no se encontro el designer regenerado C" }
$mergedMetadataABC = Get-Content $designerCRegenerated.FullName -Raw
foreach ($property in @("EquipoRevisionA","EquipoRevisionB","EquipoRevisionC")) {
    if ($mergedMetadataABC -notmatch $property) {
        throw "Reto 5.8: la migracion C regenerada no representa A+B+C; falta $property"
    }
}

Push-Location $MergedABC
try {
    dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
    Assert-Exit "Reto 5.8: el modelo A+B+C deja cambios pendientes"
}
finally { Pop-Location }

Write-Host "Reto 5.8 OK | orden seguro=A -> B regenerada -> C regenerada | metadata A+B+C=True"
#>

