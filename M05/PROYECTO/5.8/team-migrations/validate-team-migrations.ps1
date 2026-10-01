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

$columnCount = & sqlcmd -S $Server -d $Database -E -I -b -h -1 -W -Q @"
SET NOCOUNT ON;
SELECT COUNT(*)
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
WHERE t.name = N'OrdenesFabricacion'
  AND c.name IN (N'EquipoRevisionA', N'EquipoRevisionB');
"@
Assert-Exit "No se pudo verificar el esquema fusionado"
$columnCount = [int](($columnCount | ForEach-Object { $_.Trim() } | Where-Object { $_ })[-1])
if ($columnCount -ne 2) { throw "El esquema final no contiene ambas columnas de las ramas" }

$historyCount = & sqlcmd -S $Server -d $Database -E -I -b -h -1 -W -Q @"
SET NOCOUNT ON;
SELECT COUNT(*)
FROM dbo.__EFMigrationsHistory
WHERE MigrationId LIKE N'%M5_5_8_TeamA'
   OR MigrationId LIKE N'%M5_5_8_TeamBRegenerated';
"@
Assert-Exit "No se pudo verificar __EFMigrationsHistory"
$historyCount = [int](($historyCount | ForEach-Object { $_.Trim() } | Where-Object { $_ })[-1])
if ($historyCount -ne 2) { throw "El historial no contiene las dos migraciones de la resolucion correcta" }

Write-Host "Designer paralelo B contiene A: False"
Write-Host "Designer regenerado contiene A+B: True"
Write-Host "Columnas A+B en esquema final: $columnCount"
Write-Host "Migraciones de equipo aplicadas: $historyCount"
Write-Host "5.8 EQUIPOS OK"
