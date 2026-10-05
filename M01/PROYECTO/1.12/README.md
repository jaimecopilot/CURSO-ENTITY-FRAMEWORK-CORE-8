# AceriaData — Punto 1.12: Inyección de dependencias y AddDbContext

Este checkpoint representa el estado final del **Módulo 1** descrito por el punto 1.12 de la práctica.

## Flujo activo

El proyecto:

- registra `AceriaDbContext` con `AddDbContext`;
- registra `IOrdenRepositorio`/ `OrdenRepositorio` como `Scoped`;
- usa `CreateScope` para resolver servicios;
- inserta tres órdenes;
- consulta las órdenes;
- actualiza `OF-002`;
- elimina `OF-003`;
- ejecuta el reto `IServicioOrdenes` / `ServicioOrdenes`;
- conserva SQL Server LocalDB como único proveedor operativo.

## Variantes del PDF conservadas en el código

`Program.cs` incluye, comentadas y listas para activar:

1. dos ámbitos que imprimen `GetHashCode()` para demostrar que cada scope recibe un `DbContext` distinto;
2. la resolución directa de `AceriaDbContext` desde el proveedor raíz para provocar la `InvalidOperationException` explicada en el paso 7.

El alumno puede descomentar cada variante de forma independiente.

## Tiempo de diseño

`AceriaDesignTimeDbContextFactory.cs` reproduce la factoría indicada en la práctica para que `dotnet ef` pueda construir `AceriaDbContext` después de pasar al constructor con `DbContextOptions<AceriaDbContext>`.

## Migraciones

Se conserva la historia acumulativa real del módulo:

- `20260927000100_InitialCreate`;
- `20260927000200_AddAleacion`;
- `20260927000300_AddEstadoOrden`;
- los correspondientes `Designer.cs`;
- `AceriaDbContextModelSnapshot.cs`.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list
dotnet ef database update
dotnet run --project AceriaData.Console.csproj --configuration Release
```

Este estado es la base del Módulo 2.
