# Módulo 5 — Trazabilidad de requisitos

Este documento relaciona los objetivos docentes de **Persistencia empresarial** con la teoría, las prácticas, el código ejecutable y las comprobaciones automáticas del proyecto acumulativo **AceriaData**.

La secuencia parte de `M04/PROYECTO/4.12` y mantiene un estado completo por punto en `M05/PROYECTO/5.1` a `M05/PROYECTO/5.12`. La plataforma de referencia es **.NET 8, C# 12, Entity Framework Core 8 y SQL Server Express LocalDB**.

## Cómo leer la matriz

- **Teoría**: sección `Punto 5.n` de `M05/TEORIA/M05_TEORIA.md`.
- **Práctica**: sección `Punto 5.n` de `M05/PRACTICA/M05_PRACTICA.md`.
- **Implementación**: archivos principales que materializan el requisito.
- **Prueba / evidencia**: test automatizado o comportamiento observable exigido al ejecutar el punto.
- **CI**: gate del workflow `.github/workflows/validate-m5.yml` que impide dar por válido el punto si la evidencia falla.

## 5.1 — Concurrencia optimista: concepto y necesidad

**Requisito.** Reproducir concurrencia real con dos contextos independientes, demostrar una actualización perdida sobre la misma propiedad y diferenciarla de cambios concurrentes sobre propiedades distintas.

- Teoría: `Punto 5.1 — Concurrencia optimista: concepto y necesidad`.
- Práctica: `Punto 5.1 — Concurrencia optimista: concepto y necesidad`.
- Implementación: `M05/PROYECTO/5.1/src/AceriaData.Infrastructure/Repositories/ConcurrenciaOptimistaM5Repositorio.cs` y `M05/PROYECTO/5.1/src/AceriaData.Application/ConcurrenciaOptimistaM5UseCase.cs`.
- Prueba / evidencia: pérdida del cambio de A sobre la misma propiedad, conservación de cambios independientes y marcador `5.1 OK`.
- CI: **Run 5.1 against SQL Server LocalDB**.

## 5.2 — Configuración de tokens de concurrencia

**Requisito.** Configurar `rowversion` de SQL Server y un token de propiedad, comprobar el SQL real y demostrar que el conflicto se convierte en `DbUpdateConcurrencyException`.

- Teoría: `Punto 5.2 — Configuración de tokens de concurrencia`.
- Práctica: `Punto 5.2 — Configuración de tokens de concurrencia`.
- Implementación: `M05/PROYECTO/5.2/src/AceriaData.Infrastructure/Persistence/Configurations/ConcurrencyTokensConfiguration.cs` y `M05/PROYECTO/5.2/src/AceriaData.Infrastructure/Repositories/TokensConcurrenciaM5Repositorio.cs`.
- Migración: `M5_5_2_ConcurrencyTokens`.
- Prueba / evidencia: conflicto detectado con `rowversion`, conflicto con token de propiedad, SQL con `RowVersion` / `EstadoDetalle` y ausencia de índice automático por el mero token.
- CI: **Validate and apply 5.2 migration chain** y **Run 5.2 against SQL Server LocalDB**.

## 5.3 — Resolución de conflictos

**Requisito.** Aplicar estrategias cliente gana, base de datos gana y merge; tratar eliminación concurrente y reintentos acotados.

- Teoría: `Punto 5.3 — Resolución de conflictos de concurrencia`.
- Práctica: `Punto 5.3 — Resolución de conflictos de concurrencia`.
- Implementación: `M05/PROYECTO/5.3/src/AceriaData.Infrastructure/Repositories/ResolucionConflictosM5Repositorio.cs`.
- Prueba / evidencia: resultados finales diferenciados para las tres estrategias, `GetDatabaseValues` ante fila eliminada, entrada desacoplada y reintento limitado.
- CI: **Validate 5.3 has no pending model changes** y **Run 5.3 against SQL Server LocalDB**.

## 5.4 — Transacciones y savepoints

**Requisito.** Demostrar atomicidad de `SaveChanges`, transacciones explícitas, commit, rollback y rollback a savepoint con las condiciones reales de SQL Server.

- Teoría: `Punto 5.4 — Transacciones: SaveChanges y transacciones explícitas`.
- Práctica: `Punto 5.4 — Transacciones: SaveChanges y transacciones explícitas`.
- Implementación: `M05/PROYECTO/5.4/src/AceriaData.Infrastructure/Repositories/TransaccionesM5Repositorio.cs`.
- Prueba / evidencia: commit, rollback, rollback parcial, MARS desactivado y estado final de las órdenes.
- CI: **Validate 5.4 model and SQL Server savepoint prerequisites** y **Run 5.4 against SQL Server LocalDB**.

## 5.5 — Transacciones ambientales

**Requisito.** Utilizar `TransactionScope` con flujo async, explicar niveles de aislamiento, detectar promoción y evitar prometer atomicidad sobre recursos que no participan en la transacción.

- Teoría: `Punto 5.5 — Transacciones ambientales y buenas prácticas`.
- Práctica: `Punto 5.5 — Transacciones ambientales y buenas prácticas`.
- Implementación: `M05/PROYECTO/5.5/src/AceriaData.Infrastructure/Repositories/TransaccionesAmbientalesM5Repositorio.cs`.
- Prueba / evidencia: `TransactionScopeAsyncFlowOption.Enabled`, ausencia de transacción explícita interna, comprobación de `DistributedIdentifier` y ejecución LocalDB.
- CI: **Validate 5.5 model and TransactionScope guardrails** y **Run 5.5 against SQL Server LocalDB**.

## 5.6 — Migraciones en producción

**Requisito.** Separar desarrollo de despliegue controlado y trabajar con scripts revisables, `IMigrator`, migration bundles e historial real de migraciones.

- Teoría: `Punto 5.6 — Migraciones en entornos de producción: estrategias y despliegue`.
- Práctica: `Punto 5.6 — Migraciones en entornos de producción`.
- Implementación: `M05/PROYECTO/5.6/src/AceriaData.Infrastructure/Repositories/MigracionesProduccionM5Repositorio.cs` y `M05/PROYECTO/5.6/deployment/generate-production-artifacts.ps1`.
- Prueba / evidencia: aplicación con `IMigrator`, artefactos de despliegue, bundle ejecutado sobre LocalDB aislado y migración final `M5_5_2_ConcurrencyTokens`.
- CI: **Validate 5.6 production migration guardrails**, **Run 5.6 with IMigrator against SQL Server LocalDB**, **Generate 5.6 production deployment artifacts** y **Execute 5.6 migration bundle on an isolated LocalDB**.

## 5.7 — Scripts SQL idempotentes

**Requisito.** Generar scripts completos, por rango, downgrade e idempotentes; aplicar el idempotente dos veces y demostrar que el historial y el esquema permanecen coherentes.

- Teoría: `Punto 5.7 — Migraciones idempotentes y scripts SQL`.
- Práctica: `Punto 5.7 — Migraciones idempotentes y scripts SQL`.
- Implementación: `M05/PROYECTO/5.7/deployment/validate-idempotent-scripts.ps1`.
- Prueba / evidencia: doble aplicación sobre base aislada, `__EFMigrationsHistory`, migración final y columnas de concurrencia verificadas.
- CI: **Validate 5.7 model and idempotent SQL scripts**.

## 5.8 — Migraciones en equipos

**Requisito.** Reproducir dos migraciones paralelas y demostrar que renombrar no fusiona metadatos; incorporar la primera rama y regenerar la segunda sobre el modelo fusionado.

- Teoría: `Punto 5.8 — Migraciones en equipos: conflictos y buenas prácticas`.
- Práctica: `Punto 5.8 — Migraciones en equipos: conflictos y buenas prácticas`.
- Implementación: `M05/PROYECTO/5.8/team-migrations/validate-team-migrations.ps1`.
- Prueba / evidencia: designer paralelo sin cambio A, designer regenerado con A+B, `has-pending-model-changes`, columnas A+B y dos migraciones verificadas en SQL Server.
- CI: **Validate 5.8 team migration workflow**.

## 5.9 — Repository y Unit of Work

**Requisito.** Presentar Repository/UoW como decisión arquitectónica, no obligación de EF Core; demostrar abstracción, persistencia y test unitario sin base de datos.

- Teoría: `Punto 5.9 — Patrón Repositorio y Unidad de Trabajo en aplicaciones empresariales`.
- Práctica: `Punto 5.9 — Repository y Unit of Work en aplicaciones empresariales`.
- Implementación: `M05/PROYECTO/5.9/src/AceriaData.Infrastructure/Repositories/RepositoryPattern.cs`, `M05/PROYECTO/5.9/src/AceriaData.Infrastructure/Repositories/RepositorioUnidadTrabajoM5Diagnostico.cs` y `M05/PROYECTO/5.9/tests/AceriaData.Tests/RepositoryPatternTests.cs`.
- Prueba / evidencia: orden y detalle persistidos, resultados equivalentes frente a acceso directo, contextos independientes con `IDbContextFactory` y 2 tests xUnit+Moq.
- CI: **Validate 5.9 Repository and Unit of Work**.

## 5.10 — Logging y diagnóstico

**Requisito.** Cubrir logging estructurado, fichero con rotación/retención, `DiagnosticListener`, EventCounters y telemetría Application Insights sin habilitar datos sensibles por defecto.

- Teoría: `Punto 5.10 — Logging y diagnóstico en Entity Framework Core`.
- Práctica: `Punto 5.10 — Logging y diagnóstico en Entity Framework Core`.
- Implementación: `M05/PROYECTO/5.10/src/AceriaData.Console/LoggingDiagnosticoM5Runner.cs`, `M05/PROYECTO/5.10/src/AceriaData.Console/Diagnostics/EfDiagnosticObserver.cs` y `M05/PROYECTO/5.10/src/AceriaData.Console/Diagnostics/EfEventCounterListener.cs`.
- Prueba / evidencia: eventos de diagnóstico > 0, counters > 0, elementos de telemetría > 0 y entre 2 y 3 ficheros de log retenidos.
- CI: **Validate 5.10 logging and diagnostics**.

## 5.11 — Testing con EF Core

**Requisito.** Diferenciar unit testing, pruebas de proveedor, integración SQL Server e integración HTTP; incorporar Moq, InMemory, SQLite, DatabaseFixture, Respawn y `WebApplicationFactory`.

- Teoría: `Punto 5.11 — Testing con EF Core`.
- Práctica: `Punto 5.11 — Testing con EF Core`.
- Implementación: `M05/PROYECTO/5.11/tests/AceriaData.Tests/RepositoryPatternTests.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/ProviderBehavior/ProviderBehaviorTests.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/SqlServerDatabaseFixture.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/SqlServerIntegrationTests.cs`, `M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/ApiIntegrationTests.cs` y `M05/PROYECTO/5.11/src/AceriaData.Api/Program.cs`.
- Prueba / evidencia: 2 unit tests con Moq, 3 pruebas de comportamiento de proveedor, 3 de integración SQL Server y 1 HTTP; total 9 tests en 5.11.
- CI: **Validate 5.11 EF Core testing stack**.

## 5.12 — Buenas prácticas y anti-patrones

**Requisito.** Convertir recomendaciones en evidencia before/after: N+1, `Include`, proyección, over-fetching, tracking, traducción LINQ y trade-offs.

- Teoría: `Punto 5.12 — Buenas prácticas y anti-patrones en persistencia empresarial`.
- Práctica: `Punto 5.12 — Buenas prácticas y anti-patrones en persistencia empresarial`.
- Implementación: `M05/PROYECTO/5.12/src/AceriaData.Infrastructure/BuenasPracticasAntiPatronesM5Diagnostico.cs`, `M05/PROYECTO/5.12/src/AceriaData.Console/BuenasPracticasAntiPatronesM5Runner.cs` y `M05/PROYECTO/5.12/tests/AceriaData.Tests/Integration/BuenasPracticasAntiPatronesM5Tests.cs`.
- Prueba / evidencia: igualdad funcional before/after, menos roundtrips, tracking 0 en lecturas optimizadas, proyección sin `RowVersion`, fallo de método no traducible antes de emitir SQL y evaluación cliente explícita. La suite final contiene 10 tests.
- CI: **Validate 5.12 good practices before-after**.

## Requisitos transversales

| Requisito | Evidencia |
|---|---|
| Los 12 estados deben ser compilables de forma independiente | El workflow restaura y compila `M05/PROYECTO/5.1` a `5.12`. |
| Cada punto debe ser ejecutable y demostrar su comportamiento | Los gates 5.1–5.12 ejecutan el proyecto y exigen el marcador `5.n OK`. |
| SQL Server es el proveedor operativo | Los escenarios principales y la integración usan SQL Server Express LocalDB. |
| La cadena de migraciones debe mantenerse coherente | `M5_5_2_ConcurrencyTokens` es la última migración de modelo; 5.3–5.12 ejecutan `has-pending-model-changes`. |
| Los tests no deben sustituir migraciones por `EnsureCreated()` | El gate 5.12 inspecciona la suite y falla si aparece `EnsureCreated(`. |
| Application no debe depender de EF Core ni Infrastructure | **Verify architecture boundary** recorre 5.1–5.12 y rechaza esas dependencias. |
| La práctica debe apuntar a código real | Cada sección del manual referencia archivos presentes en su estado `5.n`. |
| La teoría y la práctica deben cubrir 5.1–5.12 | Ambos Markdown contienen los doce encabezados `Punto 5.n`. |
| Los PDF deben ser reproducibles | `scripts/generate_m5_pdfs.py` genera los dos PDF desde los Markdown. |
| Los PDF deben ser revisables página a página | `scripts/audit_m5_pdf_pages.py` valida geometría/contenido y `scripts/render_m5_pdf_pages.py` renderiza todas las páginas. |
| Extensión final | Teoría: 50 páginas. Prácticas: 70 páginas. |

## Inventario de pruebas del estado final 5.12

La suite `M05/PROYECTO/5.12/tests/AceriaData.Tests` contiene:

- **2 unitarias con xUnit + Moq**: servicio de Application contra `IOrdenRepositorio`, sin EF Core ni base de datos.
- **3 de comportamiento de proveedor**: InMemory y SQLite para mostrar diferencias relacionales y límites frente a SQL Server.
- **3 de integración SQL Server**: migraciones/esquema, conflicto `rowversion` real y limpieza con Respawn.
- **1 de integración HTTP**: `WebApplicationFactory` + API real + SQL Server.
- **1 de integración de buenas prácticas**: equivalencia funcional y trabajo observable en 5.12.

**Resultado esperado de la suite final: 10 tests aprobados, 0 fallidos.**

## Comprobación automática de esta trazabilidad

El script `scripts/validate_m5_traceability.py` comprueba que:

1. existen los doce puntos en teoría, práctica y esta matriz;
2. existen los archivos de implementación principales citados;
3. cada estado conserva su marcador `5.n OK`;
4. el workflow contiene un gate específico para cada requisito;
5. existen los tests finales de Repository, proveedor, SQL Server, HTTP y 5.12.

De este modo, una modificación futura que rompa la relación entre requisito, documentación, código y prueba hace fallar la validación del módulo.


## Criterio de cierre

La promoción a `main` se realiza únicamente después de ejecutar la validación completa sobre el HEAD definitivo de la rama de construcción, incluyendo esta matriz de trazabilidad.
