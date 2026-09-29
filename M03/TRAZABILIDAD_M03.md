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

## Delta contractual y cierre E2E por checkpoint

| Punto | Delta contractual introducido | Cierre ejecutable |
|---|---|---|
| 3.1 | `Consulta`, `ObtenerSqlFundamentos` | Contrasta materialización previa frente a composición `IQueryable` y muestra `ToQueryString()`. |
| 3.2 | `ObtenerPendientesPorCliente`, `ObtenerPorEstadoOrdenadasPorFecha`, `ObtenerPorRangoDeFechas`, `ObtenerPorClienteOrdenadas`, `ObtenerPorClienteYRangoDeFechas`, `ObtenerSqlConsultaBasica` | Ejecuta los seis métodos y comprueba además el orden exacto de `ThenByDescending`. |
| 3.3 | `ObtenerClientesUnicos`, `ObtenerResumenes`, `ObtenerResumenesPorEstado`, `ObtenerOrdenesConTotales`, `ObtenerSqlProyeccion` | Valida proyecciones escalares, DTOs, filtro + proyección y agregados proyectados. |
| 3.4 | `ObtenerOrdenesConPlanchas`, `ObtenerOrdenesConDetalle`, `ObtenerOrdenesCompletas`, `ObtenerSqlProyeccionNavegacion` | Valida colecciones y referencias proyectadas, incluido el detalle opcional. |
| 3.5 | Count/Any/All, Sum/Average/Min/Max y resúmenes GroupBy | Ejecuta todas las operaciones añadidas y contrasta valores deterministas del dataset. |
| 3.6 | agrupación con colecciones internas, clave Cliente/Estado, filtro de grupo, resumen mensual y SQL | Valida las agrupaciones y la estrategia de dos consultas acotadas para colecciones internas. |
| 3.7 | INNER JOIN, LEFT JOIN, detalle, aleaciones y SQL de join | Conserva expresamente una orden sin planchas en el LEFT JOIN. |
| 3.8 | Include, Include + referencia, ThenInclude, Filtered Include, SplitQuery, AutoInclude/IgnoreAutoIncludes y SQL | Ejecuta todas las variantes y comprueba las dos colecciones de SplitQuery. |
| 3.9 | `ObtenerTodasSinInclude` + proxies Lazy | Neutraliza AutoInclude antes de acceder a `Planchas`, haciendo observable Lazy Loading. |
| 3.10 | `ObtenerConCargaExplicita`, `ObtenerConPlanchasPesadasExplicitas` | Valida carga explícita y `Query()` filtrada; la implementación cubre `IsLoaded`. |
| 3.11 | `BuscarOrdenes` | Compone filtros, ordenación, Skip/Take y materializa al final. |
| 3.12 | añade cuatro métodos optimizados; retira `Consulta`, `ObtenerSqlFundamentos`, `ObtenerOrdenesAutoInclude`, `ObtenerOrdenesIgnorandoAutoInclude` | Valida proyección no-tracking, Any, SplitQuery y FirstOrDefault; la auditoría impide reexponer `IQueryable`. |

## Reglas de continuidad

- `M03/PROYECTO/3.1` parte de `M02/PROYECTO/2.12`.
- Cada estado contiene su propia `AceriaData.sln` y los cuatro proyectos.
- Se conserva el snapshot y todas las migraciones heredadas de M2.
- Ningún estado usa `EnsureCreated()`; el esquema se aplica con `Database.Migrate()`.
- 3.8 configura `AutoInclude` sobre `Planchas`, demuestra `IgnoreAutoIncludes` y usa `AsSplitQuery` con dos colecciones hermanas.
- 3.9 habilita Lazy Loading con proxies de forma demostrativa; 3.10 lo desactiva.
- 3.12 elimina `IQueryable<OrdenFabricacion>` del puerto `IOrdenRepositorio` y retira `AutoInclude` del modelo final.
- Teoría, práctica y código comparten los mismos doce puntos y marcadores E2E.
