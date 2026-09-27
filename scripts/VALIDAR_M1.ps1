$ErrorActionPreference = 'Stop'

dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release --no-restore

Push-Location src/AceriaData.Console
try {
    dotnet ef migrations list --configuration Release
    dotnet ef database update --configuration Release
    dotnet run --configuration Release
}
finally {
    Pop-Location
}
