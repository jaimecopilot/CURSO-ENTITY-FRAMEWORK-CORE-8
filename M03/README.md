# Módulo 3 - Consultas con LINQ

**12 puntos · 6 horas · AceriaData · .NET 8 · Entity Framework Core 8 · SQL Server LocalDB**

M3 continúa directamente desde el estado validado `M02/PROYECTO/2.12`. Cada carpeta `M03/PROYECTO/3.x` contiene una solución autónoma y acumulativa.

## Secuencia acumulativa

`3.1 -> 3.2 -> 3.3 -> 3.4 -> 3.5 -> 3.6 -> 3.7 -> 3.8 -> 3.9 -> 3.10 -> 3.11 -> 3.12`

## Material docente

- [Teoría - Markdown](TEORIA/M03_TEORIA.md)
- [Teoría - PDF](TEORIA/M03_TEORIA.pdf)
- [Práctica - índice](PRACTICA/README.md)
- [Práctica - Markdown](PRACTICA/M03_PRACTICA.md)
- [Práctica - PDF](PRACTICA/M03_PRACTICA.pdf)
- [Proyecto acumulativo 3.1-3.12](PROYECTO/README.md)
- [Trazabilidad práctica ↔ código](TRAZABILIDAD_M03.md)

## Temario canónico

| Punto | Tema | Solución |
|---|---|---|
| 3.1 | Fundamentos de LINQ to Entities | [3.1](PROYECTO/3.1/AceriaData.sln) |
| 3.2 | Consultas básicas: Where, OrderBy y ThenBy | [3.2](PROYECTO/3.2/AceriaData.sln) |
| 3.3 | Proyecciones con Select y tipos anónimos | [3.3](PROYECTO/3.3/AceriaData.sln) |
| 3.4 | Proyecciones a DTOs | [3.4](PROYECTO/3.4/AceriaData.sln) |
| 3.5 | Consultas de agregación: Count, Sum, Average, Min y Max | [3.5](PROYECTO/3.5/AceriaData.sln) |
| 3.6 | Agrupaciones con proyección | [3.6](PROYECTO/3.6/AceriaData.sln) |
| 3.7 | Joins y navegación en consultas | [3.7](PROYECTO/3.7/AceriaData.sln) |
| 3.8 | Eager Loading con Include y ThenInclude | [3.8](PROYECTO/3.8/AceriaData.sln) |
| 3.9 | Lazy Loading: configuración, funcionamiento y riesgos | [3.9](PROYECTO/3.9/AceriaData.sln) |
| 3.10 | Explicit Loading | [3.10](PROYECTO/3.10/AceriaData.sln) |
| 3.11 | Composición de consultas y ejecución diferida | [3.11](PROYECTO/3.11/AceriaData.sln) |
| 3.12 | Buenas prácticas en el acceso a datos y composición de consultas | [3.12](PROYECTO/3.12/AceriaData.sln) |

## Reglas técnicas fijadas

Todos los estados usan la historia real de migraciones heredada de M2 y fueron ejecutados contra SQL Server LocalDB. No se usa `EnsureCreated()`: el esquema se aplica con `Database.Migrate()`. La capa Application no depende de EF Core. Los mecanismos docentes de `IQueryable`, `AutoInclude` y Lazy Loading se introducen de forma controlada y el estado final 3.12 vuelve a encapsular el acceso a datos, retira `AutoInclude` global y mantiene Lazy Loading desactivado.
