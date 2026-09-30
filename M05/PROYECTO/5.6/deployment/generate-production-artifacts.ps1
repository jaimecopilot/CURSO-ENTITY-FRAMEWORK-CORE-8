$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$Artifacts = Join-Path $PSScriptRoot "artifacts"
$Infrastructure = Join-Path $Root "src/AceriaData.Infrastructure"
$Startup = Join-Path $Root "src/AceriaData.Console"

New-Item -ItemType Directory -Force -Path $Artifacts | Out-Null
$env:DOTNET_ENVIRONMENT = "Production"

dotnet ef migrations script --idempotent --project $Infrastructure --startup-project $Startup --configuration Release --output (Join-Path $Artifacts "aceria-idempotent.sql")
dotnet ef migrations bundle --force --project $Infrastructure --startup-project $Startup --configuration Release --output (Join-Path $Artifacts "aceria-efbundle.exe")

Write-Host "Artefactos generados en $Artifacts"
Write-Host "No incluya cadenas de conexión de producción en estos archivos."