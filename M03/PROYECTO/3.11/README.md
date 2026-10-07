# AceriaData - Punto 3.11: Composición de consultas y ejecución diferida

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **3.11**.

Parte del cierre de `M02/PROYECTO/2.12` y mantiene las cuatro capas: Domain, Application, Infrastructure y Console.

La base de demostración se reinicia de forma determinista y se reconstruye mediante **`Database.Migrate()`**. No se utiliza `EnsureCreated()`.

## Compilar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
```

## Validar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

## Ejecutar

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución del checkpoint termina con **`3.11 OK`**.
