# Proyecto acumulativo AceriaData — Módulo 1

Este directorio contiene **doce estados completos y ejecutables** del mismo proyecto AceriaData.

`1.1 → 1.2 → 1.3 → 1.4 → 1.5 → 1.6 → 1.7 → 1.8 → 1.9 → 1.10 → 1.11 → 1.12`

Cada carpeta representa cómo debe quedar el proyecto al terminar ese punto. No son doce proyectos independientes creados desde cero.

## Índice

| Estado | Cambio principal | README |
|---|---|---|
| 1.1 | Proyecto base + EF Core SQL Server | [1.1](1.1/README.md) |
| 1.2 | DbContext + OrdenFabricacion | [1.2](1.2/README.md) |
| 1.3 | PlanchaAcero + Aleacion + migraciones | [1.3](1.3/README.md) |
| 1.4 | EstadoOrden | [1.4](1.4/README.md) |
| 1.5 | Ciclo de vida y factoría manual | [1.5](1.5/README.md) |
| 1.6 | CRUD y operaciones DbSet | [1.6](1.6/README.md) |
| 1.7 | Change Tracker | [1.7](1.7/README.md) |
| 1.8 | Gestión de estados y entidades | [1.8](1.8/README.md) |
| 1.9 | SaveChanges y unidad de trabajo | [1.9](1.9/README.md) |
| 1.10 | Configuración externa y logging | [1.10](1.10/README.md) |
| 1.11 | Proveedor SQL Server y SQL generado | [1.11](1.11/README.md) |
| 1.12 | DI + repositorio + servicio | [1.12](1.12/README.md) |

## Ejecución de cualquier estado

Desde la carpeta del punto:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

El estado final del módulo es **1.12** y es el proyecto referenciado por la solución raíz `AceriaData.sln`.

La trazabilidad detallada con el documento de prácticas está en [../TRAZABILIDAD_M01.md](../TRAZABILIDAD_M01.md).
