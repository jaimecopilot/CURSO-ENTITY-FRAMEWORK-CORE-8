$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$Artifacts = Join-Path $PSScriptRoot "artifacts"
$Infrastructure = Join-Path $Root "src/AceriaData.Infrastructure"
$Startup = Join-Path $Root "src/AceriaData.Console"

New-Item -ItemType Directory -Force -Path $Artifacts | Out-Null
$env:DOTNET_ENVIRONMENT = "Production"

dotnet ef migrations script --idempotent `
  --project $Infrastructure `
  --startup-project $Startup `
  --configuration Release `
  --output (Join-Path $Artifacts "aceria-idempotent.sql")

if ($LASTEXITCODE -ne 0) {
    throw "No se pudo generar el script idempotente"
}

dotnet ef migrations bundle `
  --project $Infrastructure `
  --startup-project $Startup `
  --configuration Release `
  --output (Join-Path $Artifacts "aceria-efbundle.exe") `
  --force

if ($LASTEXITCODE -ne 0) {
    throw "No se pudo generar el migration bundle"
}

Write-Host "Artefactos generados en $Artifacts"
Write-Host "No incluya cadenas de conexión de producción en estos archivos."
