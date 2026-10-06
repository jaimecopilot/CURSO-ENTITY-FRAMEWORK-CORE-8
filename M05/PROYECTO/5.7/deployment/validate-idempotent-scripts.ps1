$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$Artifacts = Join-Path $PSScriptRoot "artifacts-5.7"
$Infrastructure = Join-Path $Root "src/AceriaData.Infrastructure"
$Startup = Join-Path $Root "src/AceriaData.Console"
$Server = "(localdb)\MSSQLLocalDB"
$Database = "AceriaDB_M5_7_Idempotent"

function Assert-LastExitCode([string]$Message) {
    if ($LASTEXITCODE -ne 0) {
        throw "$Message (exit code $LASTEXITCODE)"
    }
}

function Read-Scalar([string]$Query) {
    $lines = & sqlcmd -S $Server -d $Database -E -b -h -1 -W -Q "SET NOCOUNT ON; $Query"
    Assert-LastExitCode "Fallo la consulta de verificacion"
    $value = $lines | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" } | Select-Object -Last 1
    if ($null -eq $value) { throw "La consulta no devolvio un valor" }
    return $value
}

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw "sqlcmd no esta disponible en PATH; es necesario para demostrar la aplicacion real del script SQL."
}

New-Item -ItemType Directory -Force -Path $Artifacts | Out-Null

$Complete = Join-Path $Artifacts "aceria-completo.sql"
$Idempotent = Join-Path $Artifacts "aceria-idempotente.sql"
$Range = Join-Path $Artifacts "aceria-rango.sql"
$Downgrade = Join-Path $Artifacts "aceria-downgrade.sql"

dotnet ef migrations script --project $Infrastructure --startup-project $Startup --configuration Release --output $Complete
Assert-LastExitCode "No se pudo generar el script completo"

dotnet ef migrations script --idempotent --project $Infrastructure --startup-project $Startup --configuration Release --output $Idempotent
Assert-LastExitCode "No se pudo generar el script idempotente"

dotnet ef migrations script M2_2_12_Architecture M5_5_2_ConcurrencyTokens --project $Infrastructure --startup-project $Startup --configuration Release --output $Range
Assert-LastExitCode "No se pudo generar el script de rango"

dotnet ef migrations script M5_5_2_ConcurrencyTokens M2_2_12_Architecture --project $Infrastructure --startup-project $Startup --configuration Release --output $Downgrade
Assert-LastExitCode "No se pudo generar el script de downgrade"

$idempotentSql = Get-Content $Idempotent -Raw
if ($idempotentSql -notmatch "__EFMigrationsHistory") { throw "El script idempotente no usa __EFMigrationsHistory" }
if ($idempotentSql -notmatch "IF NOT EXISTS") { throw "El script no contiene guardas idempotentes" }
if ($idempotentSql -notmatch "M5_5_2_ConcurrencyTokens") { throw "El script no alcanza la migracion final real" }

& sqlcmd -S $Server -d master -E -b -Q "IF DB_ID(N'$Database') IS NOT NULL BEGIN ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$Database]; END; CREATE DATABASE [$Database];"
Assert-LastExitCode "No se pudo preparar la base aislada"

Write-Host "== Primera aplicacion del script idempotente =="
& sqlcmd -S $Server -d $Database -E -I -b -i $Idempotent
Assert-LastExitCode "La primera aplicacion del script idempotente fallo"

$count1 = [int](Read-Scalar "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;")
if ($count1 -le 0) { throw "El historial quedo vacio despues de la primera aplicacion" }

$lastMigration = Read-Scalar "SELECT TOP (1) MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId DESC;"
if ($lastMigration -notmatch "M5_5_2_ConcurrencyTokens") {
    throw "La migracion final aplicada no es M5_5_2_ConcurrencyTokens: $lastMigration"
}

$schemaCount = [int](Read-Scalar @"
SELECT COUNT(*)
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
WHERE (t.name = N'OrdenesFabricacion' AND c.name = N'RowVersion')
   OR (t.name = N'DetallesOrden' AND c.name = N'EstadoDetalle');
"@)
if ($schemaCount -ne 2) { throw "El esquema esperado de concurrencia no esta completo" }

Write-Host "== Segunda aplicacion del mismo script idempotente =="
& sqlcmd -S $Server -d $Database -E -I -b -i $Idempotent
Assert-LastExitCode "La segunda aplicacion del mismo script fallo"

$count2 = [int](Read-Scalar "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;")
if ($count2 -ne $count1) {
    throw "La segunda aplicacion altero el numero de migraciones: antes=$count1 despues=$count2"
}

$schemaCount2 = [int](Read-Scalar @"
SELECT COUNT(*)
FROM sys.columns c
JOIN sys.tables t ON t.object_id = c.object_id
WHERE (t.name = N'OrdenesFabricacion' AND c.name = N'RowVersion')
   OR (t.name = N'DetallesOrden' AND c.name = N'EstadoDetalle');
"@)
if ($schemaCount2 -ne $schemaCount) { throw "La segunda aplicacion altero inesperadamente el esquema" }

Write-Host "Migraciones tras primera aplicacion: $count1"
Write-Host "Migraciones tras segunda aplicacion: $count2"
Write-Host "Migracion final: $lastMigration"
Write-Host "Elementos de esquema verificados: $schemaCount2"
Write-Host "5.7 IDEMPOTENCIA OK"

<#
# RETO M05 5.7 - RANGO SEGUIDO DE IDEMPOTENTE
$ChallengeDatabase = "AceriaDB_M5_7_RangeThenIdempotent"
$BaselineToRangeStart = Join-Path $Artifacts "aceria-base-hasta-arquitectura.sql"

dotnet ef migrations script 0 M2_2_12_Architecture --project $Infrastructure --startup-project $Startup --configuration Release --output $BaselineToRangeStart
Assert-LastExitCode "Reto 5.7: no se pudo generar la base previa al rango"

& sqlcmd -S $Server -d master -E -b -Q "IF DB_ID(N'$ChallengeDatabase') IS NOT NULL BEGIN ALTER DATABASE [$ChallengeDatabase] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$ChallengeDatabase]; END; CREATE DATABASE [$ChallengeDatabase];"
Assert-LastExitCode "Reto 5.7: no se pudo preparar la base aislada"

& sqlcmd -S $Server -d $ChallengeDatabase -E -I -b -i $BaselineToRangeStart
Assert-LastExitCode "Reto 5.7: no se pudo preparar el estado M2_2_12_Architecture"

& sqlcmd -S $Server -d $ChallengeDatabase -E -I -b -i $Range
Assert-LastExitCode "Reto 5.7: fallo la aplicacion del script de rango"

$challengeCountRangeLines = & sqlcmd -S $Server -d $ChallengeDatabase -E -b -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;"
Assert-LastExitCode "Reto 5.7: no se pudo leer el historial tras el rango"
$challengeCountRange = [int]($challengeCountRangeLines | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" } | Select-Object -Last 1)

& sqlcmd -S $Server -d $ChallengeDatabase -E -I -b -i $Idempotent
Assert-LastExitCode "Reto 5.7: fallo el idempotente despues del rango"

$challengeCountFinalLines = & sqlcmd -S $Server -d $ChallengeDatabase -E -b -h -1 -W -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;"
Assert-LastExitCode "Reto 5.7: no se pudo leer el historial final"
$challengeCountFinal = [int]($challengeCountFinalLines | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" } | Select-Object -Last 1)

$challengeLastLines = & sqlcmd -S $Server -d $ChallengeDatabase -E -b -h -1 -W -Q "SET NOCOUNT ON; SELECT TOP (1) MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId DESC;"
Assert-LastExitCode "Reto 5.7: no se pudo leer la migracion final"
$challengeLast = $challengeLastLines | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" } | Select-Object -Last 1

if ($challengeCountFinal -ne $challengeCountRange) {
    throw "Reto 5.7: el idempotente altero el historial despues del rango: rango=$challengeCountRange final=$challengeCountFinal"
}
if ($challengeLast -notmatch "M5_5_2_ConcurrencyTokens") {
    throw "Reto 5.7: la migracion final no es la esperada: $challengeLast"
}

Write-Host "Reto 5.7 OK | historial rango=$challengeCountRange | historial idempotente=$challengeCountFinal | final=$challengeLast"
#>

