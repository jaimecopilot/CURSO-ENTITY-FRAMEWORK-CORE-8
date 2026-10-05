# AceriaData — Punto 1.3: Componentes principales y migraciones

Este checkpoint representa el resultado acumulativo de ejecutar el punto **1.3** de `M01_PRACTICA.pdf` a partir del estado 1.2.

## Estructura

```text
AceriaData/
├── AceriaData.sln
└── AceriaData.Console/
    ├── AceriaData.Console.csproj
    ├── Program.cs
    └── Migrations/
```

## Qué incorpora el punto

- `OrdenFabricacion` heredada de 1.2.
- `PlanchaAcero` y su relación con `OrdenFabricacion`.
- `DbSet<PlanchaAcero>`.
- Migración real `InitialCreate`.
- `Aleacion` y `DbSet<Aleacion>`.
- Migración real `AddAleacion`.
- Archivos `*.Designer.cs` y `AceriaDbContextModelSnapshot.cs` generados por EF Core.
- Ejemplos del PDF para inspeccionar proveedor/modelo y Change Tracker.
- Inserción de una plancha.
- Reto resuelto de dos planchas y consulta con `Include`.

El bloque activo de `Program.cs` corresponde al último reto resuelto. Los demás escenarios del punto permanecen como código comentado para poder activarlos manualmente sin perderlos.

> Nota de trazabilidad: el reto del PDF consulta `o.Planchas`. Por ello el checkpoint incorpora la colección de navegación `OrdenFabricacion.Planchas`, necesaria para que el código del propio reto pueda ejecutarse.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La base utilizada por la práctica es `AceriaDB`.

El siguiente estado acumulativo es [1.4](../1.4).
