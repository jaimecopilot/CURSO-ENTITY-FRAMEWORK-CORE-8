# Punto 1.10 - Introducción práctica a las migraciones

Este estado continúa 1.9 sin reescribir la historia previa de AceriaData. La cadena real de migraciones es:

```text
InitialCreate -> AddAleacion -> AddEstadoOrden
```

El punto enseña a inspeccionar `Up`/`Down`, usar `AceriaDbContextModelSnapshot`, consultar `__EFMigrationsHistory`, ejecutar `dotnet ef migrations list`, aplicar y revertir con `dotnet ef database update` y generar SQL con `dotnet ef migrations script`.

La configuración externa, la cadena de conexión y el logging se conservan porque permiten construir el DbContext tanto en ejecución como en tiempo de diseño. `AceriaDesignTimeDbContextFactory` proporciona el contexto a `dotnet ef`.

## Validación esperada

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list --configuration Release
dotnet ef database update --configuration Release
dotnet ef database update 20260927000200_AddAleacion --configuration Release
dotnet ef database update --configuration Release
dotnet ef migrations script --configuration Release --output migraciones.sql
dotnet run --project AceriaData.Console.csproj --configuration Release
```

El siguiente estado 1.11 conserva estos elementos y profundiza en el proveedor SQL Server y el SQL generado.
