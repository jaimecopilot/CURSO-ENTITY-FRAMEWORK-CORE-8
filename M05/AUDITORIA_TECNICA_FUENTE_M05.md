# M05 — Auditoría técnica de la fuente antes de construcción

> Documento interno. No forma parte del material del alumno.

## Identidad de las fuentes recibidas

- Fuente 5.1–5.9: `Texto pegado(5).txt`
  - tamaño: 509299 bytes
  - líneas: 10034
  - SHA-256: `c1f6768dc936ca013d46bd007f77a139c3bfc0e32889b0cba10b2e0ac8383ca9`
- Fuente 5.10–5.12: `Texto pegado (2)(1).txt`
  - tamaño: 120332 bytes
  - líneas: 2325
  - SHA-256: `8475838b5a93fafabfcf0f80976f8c7ba335545ef78c17aa48ac2d412b21f62c`

Estructura detectada:
- 5.1 Concurrencia optimista
- 5.2 Tokens de concurrencia
- 5.3 Resolución de conflictos
- 5.4 Transacciones
- 5.5 TransactionScope
- 5.6 Migraciones en producción
- 5.7 Scripts idempotentes
- 5.8 Migraciones en equipos
- 5.9 Repositorio / Unidad de Trabajo
- 5.10 Logging y diagnóstico
- 5.11 Testing
- 5.12 Buenas prácticas y anti-patrones

## Regla de auditoría

La fuente se conserva como inventario docente, pero no se copiará literalmente cuando contradiga el comportamiento real de EF Core 8, SQL Server o .NET 8. Toda corrección técnica debe conservar el objetivo pedagógico original y sustituir el ejemplo incorrecto por uno reproducible.

## 5.1 — Concurrencia optimista

Estado: **REESCRITURA PARCIAL OBLIGATORIA**

Hallazgos:
1. La demostración principal de "actualización perdida" usa dos contextos que modifican propiedades diferentes (`Cliente` y `Estado`) y afirma que el segundo `SaveChanges` sobrescribe el valor de `Cliente` del primero.
2. Con tracking normal, EF Core detecta cambios a nivel de propiedad y normalmente actualiza únicamente las propiedades modificadas. Ese ejemplo no demuestra la pérdida descrita.
3. Las cifras fijas de 48 ms, 235 ms y 245 ms no son resultados reproducibles y no pueden convertirse en conclusiones del manual.
4. "Pesimista = más segura" y "optimista = más rápida" son generalizaciones que dependen del escenario.

Corrección:
- Reproducir la pérdida real haciendo que ambos actores modifiquen la misma propiedad o usando un escenario disconnected que marque una entidad completa como `Modified`.
- Mostrar SQL observado, no SQL inventado.
- Separar "concurrencia sin token", "conflicto sobre misma propiedad" y "merge válido de propiedades distintas".
- Presentar tiempos solo como salida local observacional, sin fijar cifras en el resultado esperado.

## 5.2 — Tokens de concurrencia

Estado: **REESCRITURA PARCIAL OBLIGATORIA**

Hallazgos:
1. La fuente da a entender que una propiedad llamada `RowVersion` de tipo `byte[]` basta por convención. En el curso debe configurarse explícitamente mediante `IsRowVersion()` o `[Timestamp]`.
2. Es falsa la afirmación "RowVersion se indexa automáticamente en SQL Server cuando se usa como token de concurrencia". SQL Server crea índices automáticamente para PRIMARY KEY y UNIQUE constraints, no por el mero uso de `rowversion`.
3. Es falsa la afirmación de que usar a la vez `[ConcurrencyCheck]` e `IsConcurrencyToken()` provoca una excepción por "configuración duplicada". Fluent API tiene mayor precedencia que Data Annotations; una duplicidad equivalente no implica por sí sola una excepción.
4. El SQL de recuperación del nuevo rowversion debe obtenerse de la ejecución real del proveedor EF Core 8; no se debe prometer siempre un `SELECT` separado.
5. Las conclusiones de "microsegundos" y 0,5 ms por actualización no son generalizables.
6. `rowversion` es una característica específica de SQL Server. SQLite no ofrece un token autogenerado equivalente.

Corrección:
- Mantener SQL Server LocalDB como demostración real de `rowversion`.
- Introducir un `Guid` administrado por aplicación como comparación portable.
- Verificar el SQL real en E2E.
- Eliminar conclusiones de rendimiento con números prefijados.

## 5.3 — Resolución de conflictos

Estado: **VÁLIDO CON CORRECCIONES**

Hallazgos:
1. `GetDatabaseValues`, `OriginalValues`, `CurrentValues`, `Reload` y el patrón de reintento son contenidos adecuados.
2. La afirmación "cuando EF Core detecta el conflicto, la transacción se revierte" necesita contexto:
   - un `SaveChanges` crea su transacción cuando la necesita;
   - si ya existe una transacción, EF Core puede crear y revertir a un savepoint.
3. Las comparaciones "cliente gana = 245 ms", "BD gana = 220 ms", etc. no deben ser conclusiones docentes.
4. La resolución debe tener límite de reintentos para evitar bucles infinitos bajo alta contención.

Corrección:
- Demostrar las tres estrategias con el mismo conflicto inicial reproducible.
- Añadir reintento acotado.
- Medir comandos/roundtrips y resultados; tiempos solo informativos.

## 5.4 — Transacciones y savepoints

Estado: **REESCRITURA PARCIAL OBLIGATORIA**

Hallazgos:
1. La atomicidad de una única llamada a `SaveChanges` es correcta en proveedores relacionales que soportan transacciones.
2. Cuando `SaveChanges` se ejecuta dentro de una transacción ya activa, EF Core crea automáticamente un savepoint antes de guardar.
3. Falta la limitación crítica de SQL Server: EF Core no crea savepoints cuando MARS está habilitado.
4. El texto representa `ReleaseSavepoint` como `RELEASE SAVEPOINT`; SQL Server no expone esa sintaxis como operación T-SQL equivalente.
5. El texto "si Commit falla, la transacción se revierte automáticamente" es demasiado absoluto; un fallo durante commit puede requerir tratamiento específico y no debe presentarse como garantía universal.
6. Las transacciones manuales deben tratarse junto con las execution strategies de reintento; controlarlas manualmente puede ser incompatible con estrategias de reintento invocadas implícitamente.
7. Los benchmarks 850 ms vs 120 ms son datos ficticios/no reproducibles.

Corrección:
- Incluir savepoint automático de `SaveChanges`, MARS y comportamiento real de SQL Server.
- Retirar `RELEASE SAVEPOINT` como SQL esperado en SQL Server.
- E2E de commit, rollback y rollback-to-savepoint.
- Benchmark solo medido en la máquina de ejecución y sin conclusión numérica fija.

## 5.5 — TransactionScope y transacciones ambientales

Estado: **REESCRITURA IMPORTANTE OBLIGATORIA**

Hallazgos:
1. `TransactionScopeAsyncFlowOption.Enabled` debe usarse con APIs async.
2. El soporte de `System.Transactions` depende del proveedor.
3. Una operación con varias conexiones puede promoverse a transacción distribuida. El soporte distribuido de System.Transactions en .NET moderno es Windows-only.
4. El laboratorio no puede asumir que dos `DbContext`/conexiones LocalDB distintas siempre funcionarán como una única transacción distribuida.
5. La analogía final afirma que `ReadCommitted` "permite leer datos no confirmados": es incorrecto. Eso corresponde a `ReadUncommitted`; `ReadCommitted` evita lecturas sucias.
6. `Snapshot` reduce determinados bloqueos de lectura mediante versionado, pero no significa "ningún bloqueo" ni elimina conflictos de escritura.
7. Un servicio externo no se revierte porque falle o se abandone un `TransactionScope` salvo que el recurso externo participe explícitamente en la transacción. No debe sugerirse atomicidad distribuida sobre un servicio cualquiera.
8. Los tiempos 285 ms/260 ms no son evidencia general.

Corrección:
- Diseñar el E2E para no depender accidentalmente de MSDTC/LocalDB.
- Enseñar promoción y limitaciones como parte de la teoría.
- Corregir los niveles de aislamiento.
- Sustituir el "servicio externo" por una simulación explícitamente no transaccional o enseñar patrón compensatorio/outbox de forma conceptual, sin fingir rollback del recurso externo.

## 5.6 — Migraciones en producción

Estado: **VÁLIDO CON CORRECCIONES**

Hallazgos:
1. La fuente incluye scripts, bundles, backups y rollback; son contenidos relevantes.
2. `Database.Migrate()` en el arranque no debe presentarse como estrategia normal de producción, especialmente con múltiples instancias.
3. Para despliegue controlado se deben priorizar:
   - SQL revisable cuando se requiere aprobación/DBA;
   - migration bundle para automatización;
   - CLI principalmente en desarrollo/entorno controlado.
4. Un downgrade ejecuta `Down` y puede perder datos; "reversión" no equivale a restauración segura.
5. No todas las operaciones DDL de todos los proveedores tienen idénticas garantías transaccionales.

Corrección:
- Reordenar la teoría por estrategia de despliegue.
- Mantener bundle y scripts como prácticas principales.
- Tratar `Migrate` en startup como alternativa con trade-offs, no recomendación universal.

## 5.7 — Migraciones idempotentes y scripts SQL

Estado: **VÁLIDO CON CORRECCIONES**

Hallazgos:
1. El concepto de script idempotente y `__EFMigrationsHistory` es adecuado.
2. Los tiempos de generación 4200/4100/4000 ms son ejemplos no reproducibles.
3. La aplicación del script debe hacerse sobre una base de pruebas y validarse dos veces para demostrar idempotencia.
4. El script de downgrade puede ser destructivo y requiere prueba de preservación de datos.

Corrección:
- E2E: generar script, aplicar sobre DB limpia, reaplicar, validar history y esquema.
- Separar generación del script de su aplicación.
- No fijar tiempos esperados.

## 5.8 — Migraciones en equipos

Estado: **REESCRITURA IMPORTANTE OBLIGATORIA**

Hallazgos:
1. La fuente ofrece "regeneración" y "renombrado" como estrategias de resolución.
2. Para árboles de migración divergentes, la guía de EF Core indica que no se deben resolver las migraciones paralelas ordenando o renombrando archivos: los metadatos del designer siguen representando un modelo incompleto.
3. La resolución correcta es volver a un estado coherente, descartar/revertir la migración propia sin perder el cambio de modelo, incorporar la migración del compañero y regenerar la propia sobre el snapshot ya fusionado.
4. En EF Core 8 existe `dotnet ef migrations has-pending-model-changes`, útil como gate de CI.
5. Los tiempos fijos de 45 s y 120 s no deben usarse como criterio técnico.

Corrección:
- Eliminar "renombrado" como solución válida.
- Crear un laboratorio reproducible de dos ramas conceptuales y regeneración.
- Añadir gate de pending model changes.
- Incluir regla explícita para migraciones ya aplicadas a una DB compartida: no borrar código; usar migración correctiva o rollback coordinado.

## 5.9 — Repositorio y Unidad de Trabajo

Estado: **VÁLIDO CON REESCRITURA DEL BENCHMARK**

Hallazgos:
1. El punto ya contiene un test con Moq; esto no sustituye la necesidad de tratar Moq sistemáticamente en 5.11.
2. La fuente reconoce correctamente que Repository tiene ventajas y costes y que no siempre es necesario.
3. La comparación de rendimiento de la práctica está mal planteada: ambos bucles terminan llamando al mismo método de repositorio, por lo que no compara acceso directo vs repositorio.
4. La conclusión "0,05 ms por consulta" y "20 bytes por operación" no está sustentada por un benchmark controlado.
5. `GC.GetTotalMemory` alrededor de consultas no permite atribuir con precisión memoria al patrón Repository.
6. "No exponer IQueryable" es una decisión de abstracción de esta arquitectura, no una regla universal de EF Core.

Corrección:
- Mantener Repository/UoW como decisión de diseño de AceriaData.
- Sustituir el benchmark por una comparación metodológicamente válida o retirarlo como afirmación de coste.
- Mantener Moq aquí como introducción y repetirlo en 5.11 como estrategia de unit testing de Application.

## 5.10 — Logging y diagnóstico

Estado: **AMPLIACIÓN OBLIGATORIA + CORRECCIÓN**

La fuente sí menciona `DiagnosticSource` y Application Insights, pero solo superficialmente.

Falta:
- observador `DiagnosticListener` completo;
- filtrado de eventos EF;
- ciclo de vida / unsubscribe / dispose;
- EventCounters de EF Core 8;
- uso de `dotnet-counters`;
- rotación de archivo por tamaño (`fileSizeLimitBytes`, `rollOnFileSizeLimit`, retención);
- integración ejecutable de Application Insights/Azure Monitor.

Corrección adicional:
- No afirmar que `AddApplicationInsightsTelemetry()` en una consola envía automáticamente los eventos EF.
- Para contenido moderno, explicar Azure Monitor + OpenTelemetry como ruta recomendada, manteniendo Application Insights como destino de observabilidad.
- No mezclar métricas `System.Diagnostics.Metrics` introducidas en EF Core 9 con este curso EF Core 8; usar EventCounters para EF8.

## 5.11 — Testing con EF Core

Estado: **AMPLIACIÓN Y REESTRUCTURACIÓN OBLIGATORIA**

La fuente cubre xUnit, InMemory y SQLite in-memory. Faltan los elementos señalados por el autor:
- Moq;
- `WebApplicationFactory`;
- SQL Server real;
- Respawn;
- DatabaseFixture.

Correcciones adicionales:
1. Microsoft desaconseja EF Core InMemory como sustituto general de una base real; no debe presentarse como opción "rápida y fiable".
2. SQLite es relacional, pero no reproduce SQL Server: collation, SQL dialect, funciones específicas y tipos pueden diferir.
3. `rowversion` no funciona como token autogenerado de SQL Server en SQLite.
4. Para testing de queries reales, debe existir una suite contra SQL Server LocalDB.
5. Para unit testing sin base, mockear la interfaz Repository es preferible a mockear `DbSet`/LINQ.
6. Las pruebas SQL Server deben ejecutar la cadena real de migraciones.
7. Respawn se usará para restaurar datos entre tests, no para sustituir la aplicación de migraciones.
8. Para `WebApplicationFactory<Program>` se añadirá un host mínimo `AceriaData.Api`; Domain/Application/Infrastructure no cambian de responsabilidad.

Estructura de pruebas objetivo:
- Unit: xUnit + Moq, sin EF.
- Provider behavior: SQLite/InMemory solo para enseñar diferencias.
- Integration DB: SQL Server LocalDB + migrations + DatabaseFixture + Respawn.
- Integration HTTP: WebApplicationFactory + API + SQL Server test DB.

## 5.12 — Buenas prácticas y anti-patrones

Estado: **AMPLIACIÓN OBLIGATORIA + CORRECCIÓN DE REGLAS ABSOLUTAS**

La fuente ya contiene comparaciones de código aisladas; no está "vacía". Sin embargo, falta profundidad.

Falta:
- consecuencias observables y medidas;
- caso completo de refactorización;
- equivalencia funcional antes/después;
- trade-offs;
- pruebas que demuestren la mejora.

Reglas a corregir:
- `Include` no es siempre la respuesta al N+1; una proyección puede ser mejor si solo se necesitan datos concretos.
- `AsSplitQuery` no debe usarse automáticamente con varias colecciones; reduce explosión cartesiana pero introduce roundtrips y consideraciones de consistencia.
- Data Annotations no son un anti-patrón por sí mismas; Fluent API tiene ventajas de centralización y mayor capacidad, pero la elección depende del diseño.
- Un método .NET no traducible dentro de `Where` normalmente provoca fallo de traducción en EF Core 8 salvo que se cruce explícitamente a evaluación cliente.
- Aplicar una función sobre una columna puede perjudicar la sargabilidad, pero "impide índices" es demasiado absoluto.
- Repository y Unit of Work no son obligatorios: `DbContext` ya implementa conceptos equivalentes; en AceriaData se usan como frontera arquitectónica elegida.

Caso de refactorización obligatorio:
- N+1 intencionado -> captura de comandos -> `Include` -> proyección -> comparación de SQL/roundtrips/tracking/resultado.

## Gates antes de construir PDFs

1. Corregir primero el código de 5.1–5.12.
2. Cada punto debe compilar independientemente.
3. Cada punto debe ejecutar sobre SQL Server LocalDB.
4. Cada punto debe emitir un marcador verificable `5.n OK`.
5. Las migraciones deben heredarse físicamente desde M4 y crecer de forma acumulativa.
6. 5.10 debe demostrar logging, DiagnosticListener, EventCounters y rotación.
7. 5.11 debe ejecutar todos los niveles de test definidos arriba.
8. 5.12 debe ejecutar el caso before/after y demostrar equivalencia funcional.
9. Solo después se generan teoría/práctica.
10. Los generadores deben derivar código mostrado del código validado, no de snippets independientes.
11. Ningún dato de timing fijo se publicará como resultado esperado.
12. Ninguna palabra/metadata interna de QA llegará al material del alumno.

## Referencias técnicas principales

- Microsoft Learn — Handling Concurrency Conflicts (EF Core)
- Microsoft Learn — Change Tracking (EF Core)
- Microsoft Learn — Transactions (EF Core)
- Microsoft Learn — Applying Migrations (EF Core)
- Microsoft Learn — Migrations in Team Environments (EF Core)
- Microsoft Learn — Choosing a testing strategy (EF Core)
- Microsoft Learn — Metrics / EventCounters (EF Core)
- Microsoft Learn — Integration tests in ASP.NET Core
- Microsoft Learn — Azure Monitor OpenTelemetry / Application Insights
