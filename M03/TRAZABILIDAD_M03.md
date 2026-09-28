# Trazabilidad del Módulo 3

| Punto | Tema | Caso de uso E2E | Evidencia principal |
|---|---|---|---|
| 3.1 | Fundamentos de LINQ to Entities | ConsultasLinqUseCase | `3.1 OK` |
| 3.2 | Consultas básicas: Where, OrderBy y ThenBy | ConsultasBasicasUseCase | `3.2 OK` |
| 3.3 | Proyecciones con Select y tipos anónimos | ProyeccionesUseCase | `3.3 OK` |
| 3.4 | Proyecciones a DTOs | ProyeccionesDtoUseCase | `3.4 OK` |
| 3.5 | Consultas de agregación: Count, Sum, Average, Min y Max | AgregacionesUseCase | `3.5 OK` |
| 3.6 | Agrupaciones con proyección | AgrupacionesUseCase | `3.6 OK` |
| 3.7 | Joins y navegación en consultas | JoinsUseCase | `3.7 OK` |
| 3.8 | Eager Loading con Include y ThenInclude | CargaEagerUseCase | `3.8 OK` |
| 3.9 | Lazy Loading: configuración, funcionamiento y riesgos | CargaLazyUseCase | `3.9 OK` |
| 3.10 | Explicit Loading | CargaExplicitaUseCase | `3.10 OK` |
| 3.11 | Composición de consultas y ejecución diferida | ComposicionConsultasUseCase | `3.11 OK` |
| 3.12 | Buenas prácticas en el acceso a datos y composición de consultas | BuenasPracticasUseCase | `3.12 OK` |

## Reglas de continuidad

- `M03/PROYECTO/3.1` parte de `M02/PROYECTO/2.12`.
- Cada estado contiene su propia `AceriaData.sln` y los cuatro proyectos.
- Se conserva el snapshot y todas las migraciones heredadas de M2.
- Ningún estado usa `EnsureCreated()`; el esquema se aplica con `Database.Migrate()`.
- 3.8 configura `AutoInclude` sobre `Planchas`, demuestra `IgnoreAutoIncludes` y usa `AsSplitQuery` con dos colecciones hermanas.
- 3.9 habilita Lazy Loading con proxies de forma demostrativa; 3.10 lo desactiva.
- 3.12 elimina `IQueryable<OrdenFabricacion>` del puerto `IOrdenRepositorio` y retira `AutoInclude` del modelo final.
- Teoría, práctica y código comparten los mismos doce puntos y marcadores E2E.
