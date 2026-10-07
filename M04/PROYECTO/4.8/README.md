# AceriaData - Punto 4.8: Split Queries: cuándo y cómo usarlas

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **4.8**.

Parte físicamente de `M04/PROYECTO/4.7`, mantiene las cuatro capas —Domain, Application, Infrastructure y Console— y conserva la historia real de migraciones heredada. El punto añade únicamente su delta docente; no se crean migraciones vacías.

La base de demostración se reinicia de forma determinista y se reconstruye mediante **`Database.Migrate()`**. No se utiliza `EnsureCreated()`.

## Compilar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
```

## Validar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
dotnet ef database update --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe conservarse la migración final heredada **`M2_2_12_Architecture`**.

## Ejecutar

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución del checkpoint termina con **`4.8 OK`**.

## Trazabilidad

La práctica definitiva `M04/PRACTICA/M04_PRACTICA.md` documenta el delta físico respecto al punto anterior, reproduce el código docente relevante y explica línea a línea Repository, caso de uso y `Program.cs`.
