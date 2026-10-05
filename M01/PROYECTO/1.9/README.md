# AceriaData — Punto 1.9: SaveChanges y unidad de trabajo

Este checkpoint parte de [1.8](../1.8) y reproduce el punto **1.9** de la práctica.

## Qué demuestra el flujo activo

- valor devuelto por `SaveChanges` al insertar y actualizar;
- atomicidad de una única llamada a `SaveChanges`;
- rollback cuando una clave foránea inválida provoca `DbUpdateException`;
- propagación de claves generadas por SQL Server;
- manejo explícito de `DbUpdateException`;
- `SaveChangesAsync`;
- varias entidades relacionadas guardadas como una unidad de trabajo;
- reto final `InsertarOrdenConPlanchas`, con navegación y una sola llamada a `SaveChanges`.

La prueba pedagógica comprueba además que `OF-003` no queda persistida después del error y que las claves foráneas de las planchas de `OF-007` se propagan correctamente.

## Variantes conservadas para el alumno

En `Program.cs` permanecen preparadas como código comentado:

- `LogTo(..., LogLevel.Information)` para observar SQL y transacciones;
- `SaveChanges()` sin cambios para comprobar que devuelve `0`.

## Migraciones

1.9 no cambia el modelo. Hereda `InitialCreate`, `AddAleacion` y `AddEstadoOrden`, sus `Designer.cs` y `AceriaDbContextModelSnapshot.cs`.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El siguiente checkpoint acumulativo es [1.10](../1.10).
