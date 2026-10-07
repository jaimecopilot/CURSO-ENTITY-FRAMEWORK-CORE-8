# Módulo 5 — Trazabilidad de requisitos

Este documento relaciona los objetivos docentes de **Persistencia empresarial** con la teoría, las prácticas, el código ejecutable y las comprobaciones automáticas del proyecto acumulativo **AceriaData**.

La secuencia parte de `M04/PROYECTO/4.12` y mantiene un estado completo por punto en `M05/PROYECTO/5.1` a `M05/PROYECTO/5.12`. La plataforma de referencia es **.NET 8, C# 12, Entity Framework Core 8 y SQL Server Express LocalDB**.

## Cómo leer la matriz

- **Teoría**: sección `Punto 5.n` de `M05/TEORIA/M05_TEORIA.md`.
- **Práctica**: sección `Punto 5.n` de `M05/PRACTICA/M05_PRACTICA.md`.
- **Implementación**: archivos principales que materializan el requisito.
- **Prueba / evidencia**: test automatizado o comportamiento observable exigido al ejecutar el punto.

## 5.1 — Concurrencia optimista: concepto y necesidad

**Requisito.** Reproducir concurrencia real con dos contextos independientes, demostrar una actualización perdida sobre la misma propiedad y diferenciarla de cambios concurrentes sobre propiedades distintas.

- Teoría: `Punto 5.1 — Concurrencia optimista: concepto y necesidad`.
- Práctica: `Punto 5.1 — Concurrencia optimista: concepto y necesidad`.
- Implementación: `M05/PROYECTO/5.1/src/AceriaData.Infrastructure/Repositories/ConcurrenciaOptimistaM5Repositorio.cs` y `M05/PROYECTO/5.1/src/AceriaData.Application/ConcurrenciaOptimistaM5UseCase.cs`.
- Prueba / evidencia: pérdida del cambio de A sobre la misma propiedad, conservación de cambios independientes y marcador `5.1 OK`.


## 5.2 — Configuración de tokens de concurrencia

**Requisito.** Configurar `rowversion` de SQL Server y un token de propiedad, comprobar el SQL real y demostrar que el conflicto se convierte en `DbUpdateConcurrencyException`.

- Teoría: `Punto 5.2 — Configuración de tokens de concurrencia`.
- Práctica: `Punto 5.2 — Configuración de tokens de concurrencia`.
- Implementación: `M05/PROYECTO/5.2/src/AceriaData.Infrastructure/Persistence/Configurations/ConcurrencyTokensConfiguration.cs` y `M05/PROYECTO/5.2/src/AceriaData.Infrastructure/Repositories/TokensConcurrenciaM5Repositorio.cs`.
- Migración: `M5_5_2_ConcurrencyTokens`.
- Prueba / evidencia: conflicto detectado con `rowversion`, conflicto con token de propiedad, SQL con `RowVersion` / `EstadoDetalle` y ausencia de índice automático por el mero token.


## 5.3 — Resolución de conflictos

**Requisito.** Aplicar estrategias cliente gana, base de datos gana y merge; tratar eliminación concurrente y reintentos acotados.

- Teoría: `Punto 5.3 — Resolución de conflictos de concurrencia`.
- Práctica: `Punto 5.3 — Resolución de conflictos de concurrencia`.
- Implementación: `M05/PROYECTO/5.3/src/AceriaData.Infrastructure/Repositories/ResolucionConflictosM5Repositorio.cs`.
- Prueba / evidencia: resultados finales diferenciados para las tres estrategias, `GetDatabaseValues` ante fila eliminada, entrada desacoplada y reintento limitado.


## 5.4 — Transacciones y savepoints

**Requisito.** Demostrar atomicidad de `SaveChanges`, transacciones explícitas, commit, rollback y rollback a savepoint con las condiciones reales de SQL Server.

- Teoría: `Punto 5.4 — Transacciones: SaveChanges y transacciones explícitas`.
- Práctica: `Punto 5.4 — Transacciones: SaveChanges y transacciones explícitas`.
- Implementación: `M05/PROYECTO/5.4/src/AceriaData.Infrastructure/Repositories/TransaccionesM5Repositorio.cs`.
- Prueba / evidencia: commit, rollback, rollback parcial, MARS desactivado y estado final de las órdenes.


## 5.5 — Transacciones ambientales

**Requisito.** Utilizar `TransactionScope` con flujo async, explicar niveles de aislamiento, detectar promoción y evitar prometer atomicidad sobre recursos que no participan en la transacción.

- Teoría: `Punto 5.5 — Transacciones ambientales y buenas prácticas`.
- Práctica: `Punto 5.5 — Transacciones ambientales y buenas prácticas`.
- Implementación: `M05/PROYECTO/5.5/src/AceriaData.Infrastructure/Repositories/TransaccionesAmbientalesM5Repositorio.cs`.
- Prueba / evidencia: `TransactionScopeAsyncFlowOption.Enabled`, ausencia de transacción explícita interna, comprobación de `DistributedIdentifier` y ejecución LocalDB.


## 5.6 — Migraciones en producción

**Requisito.** Separar desarrollo de despliegue controlado y trabajar con scripts revisables, `IMigrator`, migration bundles e historial real de migraciones.

- Teoría: `Punto 5.6 — Migraciones en entornos de producción: estrategias y despliegue`.
- Práctica: `Punto 5.6 — Migraciones en entornos de producción`.
- Implementación: `M05/PROYECTO/5.6/src/AceriaData.Infrastructure/Repositories/MigracionesProduccionM5Repositorio.cs` y `M05/PROYECTO/5.6/deployment/generate-production-artifacts.ps1`.
- Prueba / evidencia: aplicación con `IMigrator`, artefactos de despliegue, bundle ejecutado sobre LocalDB aislado y migración final `M5_5_2_ConcurrencyTokens`.


## 5.7 — Scripts SQL idempotentes

**Requisito.** Generar scripts completos, por rango, downgrade e idempotentes; aplicar el idempotente dos veces y demostrar que el historial y el esquema permanecen coherentes.

- Teoría: `Punto 5.7 — Migraciones idempotentes y scripts SQL`.
- Práctica: `Punto 5.7 — Migraciones idempotentes y scripts SQL`.
- Implementación: `M05/PROYECTO/5.7/deployment/validate-idempotent-scripts.ps1`.
- Prueba / evidencia: doble aplicación sobre base aislada, `__EFMigrationsHistory`, migración final y columnas de concurrencia verificadas.


## 5.8 — Migraciones en equipos

**Requisito.** Reproducir dos migraciones paralelas y demostrar que renombrar no fusiona metadatos; incorporar la primera rama y regenerar la segunda sobre el modelo fusionado.

- Teoría: `Punto 5.8 — Migraciones en equipos: conflictos y buenas prácticas`.
- Práctica: `Punto 5.8 — Migraciones en equipos: conflictos y buenas prácticas`.
- Implementación: `M05/PROYECTO/5.8/team-migrations/validate-team-migrations.ps1`.
- Prueba / evidencia: designer paralelo sin cambio A, designer regenerado con A+B, `has-pending-model-changes`, columnas A+B y dos migraciones verificadas en SQL Server.


## 5.9 — Repository y Unit of Work

**Requisito.** Presentar Repository/UoW como decisión arquitectónica, no obligación de EF Core; demostrar abstracción, persistencia y test unitario sin base de datos.

- Teoría: `Punto 5.9 — Patrón Repositorio y Unidad de Trabajo en aplicaciones empresariales`.
- Práctica: `Punto 5.9 — Repository y Unit of Work en aplicaciones empresariales`.
- Implementación: `M05/PROYECTO/5.9/src/AceriaData.Infrastructure/Repositories/RepositoryPattern.cs`, `M05/PROYECTO/5.9/src/AceriaData.Infrastructure/Repositories/RepositorioUnidadTrabajoM5Diagnostico.cs` y `M05/PROYECTO/5.9/tests/AceriaData.Tests/RepositoryPatternTests.cs`.
- Prueba / evidencia: orden y detalle persistidos, resultados equivalentes frente a acceso directo, contextos independientes con `IDbContextFactory` y 2 tests xUnit+Moq.


## 5.10 — Logging y diagnóstico

**Requisito.** Cubrir logging estructurado, fichero con rotación/retención, `DiagnosticListener`, EventCounters y telemetría Application Insights sin habilitar datos sensibles por defecto.

- Teoría: `Punto 5.10 — Logging y diagnóstico en Entity Framework Core`.
- Práctica: `Punto 5.10 — Logging y diagnóstico en Entity Framework Core`.
- Implementación: `M05/PROYECTO/5.10/src/AceriaData.Console/LoggingDiagnosticoM5Runner.cs`, `M05/PROYECTO/5.10/src/AceriaData.Console/Diagnostics/EfDiagnosticObserver.cs`, `M05/PROYECTO/5.10/src/AceriaData.Console/Diagnostics/EfEventCounterListener.cs` y `M05/PROYECTO/5.10/src/AceriaData.Console/Diagnostics/AzureMonitorOpenTelemetry.cs`.
- Prueba / evidencia: eventos `DiagnosticListener` > 0, EventCounters > 0, archivo con rotación/retención y pipeline opcional `OpenTelemetry` + `Azure.Monitor.OpenTelemetry.Exporter` cuando existe `APPLICATIONINSIGHTS_CONNECTION_STRING`. Sin cadena real, el laboratorio omite el envío externo y sigue siendo verificable localmente.


## 5.11 — Testing con EF Core

**Requisito.** Diferenciar unit testing, pruebas de proveedor, integración SQL Server e integración HTTP; incorporar Moq, InMemory, SQLite, DatabaseFixture, Respawn y `WebApplicationFactory`.

- Teoría: `Punto 5.11 — Testing con EF Core`.
- Práctica: `Punto 5.11 — Testing con EF Core`.
- Implementación: `M05/PROYECTO/5.11/tests/AceriaData.Tests/RepositoryPatternTests.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/ProviderBehavior/ProviderBehaviorTests.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/SqlServerDatabaseFixture.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/SqlServerIntegrationTests.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/ApiIntegrationTests.cs` y `M05/PROYECTO/5.11/src/AceriaData.Api/Program.cs`.
- Prueba / evidencia: 2 unit tests con Moq, 3 pruebas de comportamiento de proveedor, 3 de integración SQL Server y 1 HTTP; total 9 tests en 5.11.


## 5.12 — Buenas prácticas y anti-patrones

**Requisito.** Convertir recomendaciones en evidencia before/after: N+1, `Include`, proyección, over-fetching, tracking, traducción LINQ y trade-offs.

- Teoría: `Punto 5.12 — Buenas prácticas y anti-patrones en persistencia empresarial`.
- Práctica: `Punto 5.12 — Buenas prácticas y anti-patrones en persistencia empresarial`.
- Implementación: `M05/PROYECTO/5.12/src/AceriaData.Infrastructure/BuenasPracticasAntiPatronesM5Diagnostico.cs`, `M05/PROYECTO/5.12/src/AceriaData.Console/BuenasPracticasAntiPatronesM5Runner.cs` y `M05/PROYECTO/5.12/tests/AceriaData.Tests/Integration/BuenasPracticasAntiPatronesM5Tests.cs`.
- Prueba / evidencia: igualdad funcional before/after, menos roundtrips, tracking 0 en lecturas optimizadas, proyección sin `RowVersion`, fallo de método no traducible antes de emitir SQL y evaluación cliente explícita. La suite final contiene 10 tests.


## Requisitos transversales

| Requisito | Evidencia |
|---|---|
| Los 12 estados deben ser compilables de forma independiente | `Test-M05.ps1 -Suite all` restaura/compila/ejecuta los checkpoints y el run definitivo `37605026463` terminó `SUCCESS`. |
| Cada punto debe ser ejecutable y demostrar su comportamiento | Los gates 5.1–5.12 validan código y comportamiento; 5.10–5.12 tuvieron además validación dirigida en el run `37606235673`, también `SUCCESS`. |
| SQL Server es el proveedor operativo | Los escenarios principales y la integración usan SQL Server Express LocalDB. |
| La cadena de migraciones debe mantenerse coherente | `M5_5_2_ConcurrencyTokens` es la última migración de modelo; 5.3–5.12 ejecutan `has-pending-model-changes`. |
| Los tests no deben sustituir migraciones por `EnsureCreated()` | El gate 5.12 inspecciona la suite y falla si aparece `EnsureCreated(`. |
| Application no debe depender de EF Core ni Infrastructure | **Verify architecture boundary** recorre 5.1–5.12 y rechaza esas dependencias. |
| La práctica debe apuntar a código real | Cada sección del manual referencia archivos presentes en su estado `5.n`. |
| La teoría y la práctica deben cubrir 5.1–5.12 | Ambos Markdown contienen los doce encabezados `Punto 5.n`. |
| Los PDF deben corresponder con los manuales finales | La práctica M5 fue reconstruida/regenerada desde el contrato canónico y el PDF final quedó cerrado antes de la reconciliación de código. |
| Contrato técnico final | `M05/PRACTICA/M05_PRACTICA_CANONICA.md` fija el snapshot usado por `Test-M05Canonical.ps1`. |

## Inventario de pruebas del estado final 5.12

La suite final `M05/PROYECTO/5.12/tests/AceriaData.Tests` contiene:

- **2 unitarias con xUnit + Moq**: servicio de Application contra `IOrdenRepositorio`, sin EF Core ni base de datos.
- **3 de comportamiento de proveedor**: InMemory y SQLite para mostrar diferencias relacionales y límites frente a SQL Server.
- **3 de integración SQL Server**: migraciones/esquema, conflicto `rowversion` real y limpieza con Respawn.
- **1 de integración HTTP**: `WebApplicationFactory` + API real + SQL Server.
- **1 de integración de buenas prácticas**: equivalencia funcional y trabajo observable en 5.12.

**Resultado esperado de la suite final: 10 tests aprobados, 0 fallidos.**


## Cierre técnico definitivo

- Rama: `fix/e2e-pedagogico-m05`.
- E2E acumulativo 5.1–5.12: run `37605026463` — **SUCCESS**.
- Validación dirigida 5.10–5.12: run `37606235673` — **SUCCESS**.
- Estado: **M05 código cerrado contra la práctica canónica**.
