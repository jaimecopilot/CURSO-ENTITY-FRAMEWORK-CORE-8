# M05 — Contrato interno de calidad

> Documento interno de construcción. No debe incorporarse a los PDF/MD entregables del alumno.

## Baseline

- M5 parte físicamente de `M04/PROYECTO/4.12`.
- Cada punto `5.1 -> 5.12` será una solución completa, independiente y acumulativa.
- `5.n` debe contener íntegramente el estado de `5.(n-1)` más únicamente el delta docente del punto.
- La lógica de Application no puede depender de Entity Framework Core.
- SQL Server LocalDB sigue siendo el proveedor operativo principal.
- InMemory y SQLite se usan únicamente donde el objetivo docente justifica un doble de pruebas.
- Ningún texto de auditoría interna, fuente, checksum, CI, E2E, checkpoint o trazabilidad debe aparecer en teoría/práctica del alumno.

## Correcciones obligatorias aportadas por el autor

### 5.10 — Logging y diagnóstico
Debe incluir, tanto en teoría como en práctica y código ejecutable:

- ILogger / ILoggerFactory y categorías de EF Core.
- Serilog y logging estructurado.
- Archivo con rotación temporal y por tamaño:
  - `fileSizeLimitBytes`
  - `rollOnFileSizeLimit: true`
  - `retainedFileCountLimit`
- DiagnosticSource/DiagnosticListener con observador completo, suscripción, filtrado y liberación.
- Application Insights aplicable al host de consola, con la integración correcta para aplicaciones no HTTP.
- Contadores de rendimiento de EF Core 8 mediante EventCounters y `dotnet-counters`.
- Evidencias observables: comandos, duración, número de eventos, counters y archivos rotados.

### 5.11 — Testing con EF Core
Debe incluir:

- xUnit.
- EF Core InMemory, explicando sus limitaciones y sin presentarlo como sustituto fiel de SQL Server.
- SQLite in-memory, explicando las diferencias respecto a SQL Server.
- Moq para mocks de repositorios/casos de uso; no enseñar mocking de DbSet como estrategia preferente.
- Proyecto/host ASP.NET Core mínimo `AceriaData.Api` para demostrar `WebApplicationFactory<Program>`.
- Tests de integración contra SQL Server LocalDB real y una base de datos de pruebas aislada.
- `DatabaseFixture` con ciclo de vida explícito.
- `Respawn` para devolver la base de datos SQL Server a un estado conocido entre pruebas.
- Migraciones reales en las pruebas SQL Server.
- Pruebas de integración del API con WebApplicationFactory y base SQL Server de pruebas.
- Separación clara entre unit tests, provider-fake tests e integration tests.

### 5.12 — Buenas prácticas y anti-patrones
Debe incluir:

- Código ejecutable que compare anti-patrón y alternativa correcta.
- Consecuencias reales y observables: comandos SQL, tracking, filas/columnas, roundtrips, excepciones, consistencia o coste.
- Al menos un caso completo de refactorización paso a paso:
  1. anti-patrón reproducible;
  2. evidencia del problema;
  3. primera corrección;
  4. segunda mejora;
  5. versión final;
  6. prueba de equivalencia funcional;
  7. comparación técnica antes/después.
- Matriz de anti-patrones con síntoma, consecuencia, evidencia, refactor y trade-off.
- No presentar `Include`, `AsSplitQuery`, Fluent API, repositorios o transacciones como reglas universales; justificar por contexto.

## Correcciones técnicas detectadas en la fuente que deben auditarse

- 5.1: una demostración de actualización perdida no puede basarse en dos contextos que modifican propiedades distintas y confiar en que EF Core sobrescriba automáticamente columnas no modificadas.
- 5.2: `rowversion` debe configurarse explícitamente como token; no asumir que el nombre de la propiedad basta.
- 5.2: no afirmar que SQL Server crea automáticamente un índice por usar `rowversion`.
- 5.2: revisar el SQL real generado por EF Core 8; no codificar manualmente SQL supuesto como evidencia.
- 5.2/5.11: `rowversion` es específico de SQL Server; SQLite requiere otra estrategia para un token autogestionado.
- 5.9: cualquier comparación de tiempo o memoria debe ser observacional y no usar cifras fijas como conclusión reproducible.
- 5.11: InMemory no se describirá como una base relacional realista ni necesariamente más rápida; se seguirá la guía oficial de estrategia de testing de EF Core.
- 5.12: un método .NET no traducible dentro de `Where` en EF Core 8 debe tratarse como fallo de traducción salvo frontera cliente explícita.
- 5.12: funciones sobre columnas pueden afectar sargabilidad, pero la conclusión sobre índices depende del proveedor, collation, índices y plan.
- 5.12: Split Query y repositorios son decisiones de diseño, no optimizaciones obligatorias.

## QA obligatorio antes de cerrar M5

- Auditoría fuente 5.1-5.12.
- Verificación técnica contra documentación oficial EF Core 8/.NET 8.
- Restore/build de 5.1-5.12.
- Aplicación de migraciones sobre SQL Server LocalDB.
- E2E real de cada punto.
- En 5.11: `dotnet test` con unit tests, SQLite/InMemory y SQL Server real.
- Verificación de WebApplicationFactory.
- Verificación de Respawn/DatabaseFixture.
- Auditoría de continuidad física entre puntos.
- Teoría y práctica con código real y explicaciones específicas línea a línea.
- Generación MD/PDF con el sistema visual de M4/M3.
- Auditoría automática de contenido y prohibición de metadatos internos.
- Render e inspección visual de todas las páginas.
- Promoción a main solo después de que todos los gates estén verdes.
