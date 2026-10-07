# Punto 2.11 - Migraciones en el modelado: ciclo completo con Soft Delete

Este checkpoint parte de 2.10 y utiliza un cambio funcional real para estudiar migraciones avanzadas.

## Delta del modelo

`OrdenFabricacion`, `PlanchaAcero`, `Aleacion` y `EstadoOrden` incorporan `IsDeleted` y `DeletedAt`. La migración real es `20260927204841_M2_2_11`.

## Qué se valida

- revisión de `Up` y `Down`;
- `AceriaDbContextModelSnapshot`;
- `__EFMigrationsHistory`;
- `dotnet ef migrations list`;
- aplicación de 2.11;
- rollback a `20260927204833_M2_2_10`;
- reaplicación de 2.11;
- generación de scripts SQL y `--idempotent`;
- E2E de Soft Delete y restauración.

```powershell
dotnet ef migrations list --configuration Release
dotnet ef database update 20260927204833_M2_2_10 --configuration Release
dotnet ef database update --configuration Release
dotnet ef migrations script 20260927204833_M2_2_10 20260927204841_M2_2_11 --configuration Release --output softdelete.sql
dotnet ef migrations script --idempotent --configuration Release --output migraciones_idempotentes.sql
dotnet run --project AceriaData.Console.csproj --configuration Release --no-build
```

El reto del punto utiliza `softdelete.sql` para inspeccionar exclusivamente el delta 2.10 → 2.11 y comprobar las cuatro parejas `DeletedAt` / `IsDeleted`. El script `migraciones_idempotentes.sql` permite comprobar además el uso de `__EFMigrationsHistory` antes de aplicar cada bloque.

La práctica explica `migrations remove` sobre una copia desechable; no se elimina la migración canónica que forma parte de la historia validada del curso.
