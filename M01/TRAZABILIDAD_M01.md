# Trazabilidad — Módulo 1

La matriz enlaza cada punto con su teoría, práctica, cambio acumulativo, checkpoint y validación.

| Punto | Teoría | Práctica / cambio en AceriaData | Archivos principales | Checkpoint | Validación |
|---|---|---|---|---|---|
| 1.1 | ORM y necesidad de EF Core | Crear solución/proyecto y paquetes | AceriaData.sln; csproj | M1-CP01 | restore/build/run |
| 1.2 | Arquitectura general | DbContext + OrdenFabricacion + EnsureCreated temporal | Program.cs | M1-CP02 | build/run + LocalDB |
| 1.3 | DbContext/DbSet/ChangeTracker/proveedor/migraciones | PlanchaAcero + Aleacion + InitialCreate + AddAleacion | Program.cs; Migrations | M1-CP03 | migrations list/update |
| 1.4 | DbContext y sus responsabilidades | EstadoOrden + AddEstadoOrden + inspección Model/ChangeTracker | Program.cs; Migrations | M1-CP04 | migrations + esquema 4 tablas |
| 1.5 | Ciclo de vida | Factory manual + UoW cortas | Program.cs | M1-CP05 | build/run |
| 1.6 | DbSet y acceso básico | CRUD + Find/Any/Count/AsNoTracking | Program.cs | M1-CP06 | run + datos |
| 1.7 | Change Tracker | Estados, DetectChanges, OriginalValues, Clear, AcceptAllChanges | Program.cs | M1-CP07 | run + estados |
| 1.8 | Gestión de entidades | Add/Range/Update/Remove/Attach/Entry | Program.cs | M1-CP08 | run + estados/SQL |
| 1.9 | SaveChanges y UoW | Atomicidad, async, claves, errores, UoW | Program.cs | M1-CP09 | run + logging/errores controlados |
| 1.10 | Opciones/conexión/logging | appsettings + configuración + logging | Program.cs; appsettings.json | M1-CP10 | build/run + SQL log |
| 1.11 | Proveedores | Auditoría del proveedor SQL Server; sin alternar motor | Program.cs; appsettings.json | M1-CP11 | ProviderName/ToQueryString/migrations script |
| 1.12 | DI y AddDbContext | AddDbContext + repositorio + servicio | Program.cs; DesignTimeFactory | M1-CP12 | restore/build/migrations/run E2E |

## Regla de continuidad

Cada checkpoint parte del anterior. Los cambios de ubicación realizados durante la auditoría conservan el material original: PlanchaAcero y Aleacion se consolidan en 1.3; EstadoOrden en 1.4; 1.5 se dedica al ciclo de vida y a la fábrica manual.
