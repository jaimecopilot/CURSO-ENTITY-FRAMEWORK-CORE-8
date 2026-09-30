# AceriaData - Punto 5.2: Configuración de tokens de concurrencia

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.2**.

Parte físicamente de `M05/PROYECTO/5.1`. Conserva íntegramente el punto anterior y añade tokens de concurrencia reales.

## Delta del punto

- `OrdenFabricacion`, `PlanchaAcero` y `Aleacion` incorporan `RowVersion`.
- Fluent API configura esas propiedades con `IsRowVersion()`.
- `DetalleOrden.EstadoDetalle` se configura con `IsConcurrencyToken()`.
- La migración del punto se genera con `dotnet ef migrations add`.
- La demostración usa dos `DbContext` reales contra SQL Server LocalDB.
- Se captura el SQL realmente ejecutado.
- Se comprueba que usar `rowversion` como token no crea automáticamente un índice.

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

La última migración debe ser **`M5_5_2_ConcurrencyTokens`**.

## Ejecutar

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución debe terminar con:

```text
5.2 OK
```
