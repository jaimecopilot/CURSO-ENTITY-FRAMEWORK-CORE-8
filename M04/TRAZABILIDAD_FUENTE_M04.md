# Trazabilidad de la fuente docente - Módulo 4

Esta matriz demuestra que la fuente 4.1-4.12 se conserva como especificación docente, distinguiendo lo que se mantiene, lo que se adapta al AceriaData real y lo que se corrige por comportamiento de EF Core 8.

| Punto | Cobertura | Tratamiento principal |
|---|---|---|
| 4.1 | CONSERVADO / ADAPTADO | El checkpoint valida ToQueryString, logging, filtro, Include y proyección. El reto de múltiples colecciones se conserva como puente hacia 4.8, donde se demuestra con dos colecciones reales. |
| 4.2 | CONSERVADO / ADAPTADO / CORREGIDO | La fuente proponía contextos separados para aislar mediciones. AceriaData usa ChangeTracker.Clear() antes de cada escenario, que elimina la contaminación entre mediciones dentro del E2E determinista. |
| 4.3 | CONSERVADO / ADAPTADO / CORREGIDO | La fuente usaba planchas compartidas, pero PlanchaAcero pertenece a una sola orden. La práctica definitiva usa Aleacion, que sí es una entidad compartida por varias relaciones y permite demostrar identidad duplicada de forma real. |
| 4.4 | CONSERVADO / ADAPTADO / CORREGIDO | Lazy Loading permanece desactivado en la baseline. Por eso el N+1 se provoca explícitamente mediante una consulta por orden y se mide con DbCommandInterceptor, sin depender de comportamiento oculto. |
| 4.5 | CONSERVADO / ADAPTADO / CORREGIDO | El checkpoint compara alternativas contando comandos reales. SplitQuery no se presenta como regla universal: se usa en un grafo con dos colecciones donde el trade-off es observable. |
| 4.6 | CONSERVADO / ADAPTADO | El checkpoint 4.6 demuestra directamente el over-fetching de columnas con SQL real. El over-fetching de filas y la paginación se mantienen en teoría y se ejecutan de forma específica en 4.10. |
| 4.7 | CONSERVADO / ADAPTADO / CORREGIDO | Se corrige la fuente: EF Core 8 no filtra silenciosamente en memoria dentro de Where. El checkpoint exige observar InvalidOperationException y solo después demuestra evaluación cliente explícita con AsEnumerable(). |
| 4.8 | CONSERVADO / ADAPTADO / CORREGIDO | La configuración global se conserva como contenido de estudio, pero no se activa en la baseline porque ocultaría la comparación docente. La coherencia se explica en términos de aislamiento/transacción, no como una transacción independiente por subconsulta. |
| 4.9 | CONSERVADO / ADAPTADO / CORREGIDO | El checkpoint ejecutable usa una compiled query síncrona parametrizada para validar equivalencia. Async, proyección y variantes se conservan en teoría y como ampliación, sin inventar una ventaja temporal obligatoria. |
| 4.10 | CONSERVADO / ADAPTADO / CORREGIDO | La fuente advertía del riesgo de usar solo fecha; el checkpoint lo corrige con cursor compuesto FechaCreacion + Id y añade datos suficientes para recorrer varias páginas. |
| 4.11 | CONSERVADO / ADAPTADO / CORREGIDO | La fuente propone un DiagnosticObserver. La baseline validada usa LogTo + DbCommandInterceptor + TagWith para contar comandos y correlacionar consultas de forma determinista. DiagnosticSource se conserva en teoría y como ampliación, no se elimina silenciosamente. |
| 4.12 | CONSERVADO / ADAPTADO / CORREGIDO | Se conserva el checklist, pero se corrige la idea de aplicar todas las técnicas a toda consulta. El cierre exige justificar también por qué Include, SplitQuery o CompiledQuery no aplican a una consulta concreta. |

## Criterios de conservación

- Todos los objetivos de aprendizaje de la fuente aparecen en la teoría definitiva.
- Los subtemas teóricos se mantienen salvo correcciones técnicas explícitas.
- Los bloques de código, SQL y texto técnico de la teoría fuente se conservan o se adaptan explícitamente; no se sustituyen por un único ejemplo final.
- Los retos y errores comunes relevantes se reintroducen en la práctica definitiva como trazabilidad y ampliación.
- El código de la práctica no copia ejemplos esquemáticos que contradicen el modelo real; usa los checkpoints validados 4.1-4.12.
- Las correcciones de EF Core 8 no eliminan el objetivo docente original: lo reformulan con comportamiento reproducible.

## 4.1 - Análisis del SQL generado: ToQueryString y logging

**Focos conservados:**

- ToQueryString antes de materializar y logging de comandos SQL.
- Consultas con Where, OrderBy, Select e Include, incluyendo el filtro global de Soft Delete.
- Análisis de múltiples Include como origen potencial de multiplicación de filas.

**Ejemplos teóricos de la fuente conservados/adaptados:** 18.

**Adaptación/corrección:** El checkpoint valida ToQueryString, logging, filtro, Include y proyección. El reto de múltiples colecciones se conserva como puente hacia 4.8, donde se demuestra con dos colecciones reales.

**Reto conservado/adaptado:** Construye mentalmente una consulta con dos colecciones incluidas y anticipa cómo crecerían las filas; compruébalo después en 4.8.

## 4.2 - Tracking y No Tracking

**Focos conservados:**

- Tracking, AsTracking, AsNoTracking y coste del ChangeTracker.
- Conteo de entidades rastreadas y comparación aislada entre consultas.
- Tracking de grafos con entidades relacionadas.

**Ejemplos teóricos de la fuente conservados/adaptados:** 19.

**Adaptación/corrección:** La fuente proponía contextos separados para aislar mediciones. AceriaData usa ChangeTracker.Clear() antes de cada escenario, que elimina la contaminación entre mediciones dentro del E2E determinista.

**Reto conservado/adaptado:** Carga un grafo con relaciones con y sin tracking y razona qué entidades quedarían registradas.

## 4.3 - AsNoTracking y AsNoTrackingWithIdentityResolution

**Focos conservados:**

- AsNoTracking frente a AsNoTrackingWithIdentityResolution.
- Conteo por referencia usando ReferenceEqualityComparer.
- Escenario donde una misma clave aparece varias veces en el resultado.

**Ejemplos teóricos de la fuente conservados/adaptados:** 12.

**Adaptación/corrección:** La fuente usaba planchas compartidas, pero PlanchaAcero pertenece a una sola orden. La práctica definitiva usa Aleacion, que sí es una entidad compartida por varias relaciones y permite demostrar identidad duplicada de forma real.

**Reto conservado/adaptado:** Compara por referencia las instancias de una aleación compartida con y sin Identity Resolution.

## 4.4 - Problema N+1: identificación y causas

**Focos conservados:**

- Identificación de N+1, sus causas y relación con navegaciones.
- Conteo real de comandos SQL y comparación con una alternativa sin N+1.
- Variantes conceptuales con Lazy Loading, consultas en bucle, FirstOrDefault y proyecciones.

**Ejemplos teóricos de la fuente conservados/adaptados:** 14.

**Adaptación/corrección:** Lazy Loading permanece desactivado en la baseline. Por eso el N+1 se provoca explícitamente mediante una consulta por orden y se mide con DbCommandInterceptor, sin depender de comportamiento oculto.

**Reto conservado/adaptado:** Provoca N+1 al consultar detalle por orden y compáralo conceptualmente con una carga anticipada o proyección.

## 4.5 - Solución a N+1: Include, proyecciones y Split Queries

**Focos conservados:**

- Include y ThenInclude para cargar grafos.
- Proyecciones para obtener solo los datos necesarios.
- AsSplitQuery como alternativa cuando existen varias colecciones.

**Ejemplos teóricos de la fuente conservados/adaptados:** 13.

**Adaptación/corrección:** El checkpoint compara alternativas contando comandos reales. SplitQuery no se presenta como regla universal: se usa en un grafo con dos colecciones donde el trade-off es observable.

**Reto conservado/adaptado:** Combina Include, ThenInclude, Identity Resolution y SplitQuery en un grafo con planchas y aleaciones y justifica el número de comandos.

## 4.6 - Over-fetching: causas y soluciones

**Focos conservados:**

- Over-fetching de columnas y de filas.
- Proyecciones, filtros y paginación para reducir datos transferidos.
- Inspección del SQL para comparar entidad completa frente a shape reducido.

**Ejemplos teóricos de la fuente conservados/adaptados:** 14.

**Adaptación/corrección:** El checkpoint 4.6 demuestra directamente el over-fetching de columnas con SQL real. El over-fetching de filas y la paginación se mantienen en teoría y se ejecutan de forma específica en 4.10.

**Reto conservado/adaptado:** Compara el SELECT de entidad completa y proyección y relaciona las columnas eliminadas con transferencia y materialización.

## 4.7 - Consultas ineficientes: traducción y frontera cliente/servidor

**Focos conservados:**

- Filtros no traducibles y frontera cliente/servidor.
- Funciones aplicadas a columnas y posible pérdida de sargabilidad.
- Reescritura de expresiones y uso de collation cuando corresponda.

**Ejemplos teóricos de la fuente conservados/adaptados:** 13.

**Adaptación/corrección:** Se corrige la fuente: EF Core 8 no filtra silenciosamente en memoria dentro de Where. El checkpoint exige observar InvalidOperationException y solo después demuestra evaluación cliente explícita con AsEnumerable().

**Reto conservado/adaptado:** Reescribe una validación de formato para usar operaciones traducibles y explica qué parte debe seguir ejecutándose en SQL.

## 4.8 - Split Queries: cuándo y cómo usarlas

**Focos conservados:**

- AsSingleQuery frente a AsSplitQuery con varias colecciones.
- Explosión cartesiana, duplicación de datos y roundtrips.
- Coherencia entre varios comandos y configuración global de Split Queries.

**Ejemplos teóricos de la fuente conservados/adaptados:** 14.

**Adaptación/corrección:** La configuración global se conserva como contenido de estudio, pero no se activa en la baseline porque ocultaría la comparación docente. La coherencia se explica en términos de aislamiento/transacción, no como una transacción independiente por subconsulta.

**Reto conservado/adaptado:** Analiza cómo cambiaría el comportamiento si SplitQuery fuera global y qué advertencias querrías convertir en señal de diagnóstico.

## 4.9 - Compiled Queries

**Focos conservados:**

- EF.CompileQuery y EF.CompileAsyncQuery, parámetros y proyecciones.
- Caché interna de consultas de EF Core y coste que realmente evita una compiled query.
- Medición en hot paths sin prometer una mejora universal.

**Ejemplos teóricos de la fuente conservados/adaptados:** 12.

**Adaptación/corrección:** El checkpoint ejecutable usa una compiled query síncrona parametrizada para validar equivalencia. Async, proyección y variantes se conservan en teoría y como ampliación, sin inventar una ventaja temporal obligatoria.

**Reto conservado/adaptado:** Diseña una compiled query proyectada y explica qué coste de EF evita frente al coste de red y SQL Server.

## 4.10 - Paginación eficiente: Skip/Take y keyset pagination

**Focos conservados:**

- Offset pagination con Skip/Take.
- Keyset pagination con orden totalmente determinista.
- Filtro, proyección y dirección de paginación.

**Ejemplos teóricos de la fuente conservados/adaptados:** 13.

**Adaptación/corrección:** La fuente advertía del riesgo de usar solo fecha; el checkpoint lo corrige con cursor compuesto FechaCreacion + Id y añade datos suficientes para recorrer varias páginas.

**Reto conservado/adaptado:** Añade mentalmente un filtro de estado a la paginación y conserva el mismo orden compuesto para no saltar ni repetir filas.

## 4.11 - Diagnóstico con logs, métricas y herramientas

**Focos conservados:**

- LogTo, categorías, niveles, ILoggerFactory, SensitiveDataLogging, DetailedErrors y ConfigureWarnings.
- Tiempo, número de comandos, filas y tracking como métricas observables.
- DiagnosticSource/DiagnosticListener y detección de consultas lentas.

**Ejemplos teóricos de la fuente conservados/adaptados:** 16.

**Adaptación/corrección:** La fuente propone un DiagnosticObserver. La baseline validada usa LogTo + DbCommandInterceptor + TagWith para contar comandos y correlacionar consultas de forma determinista. DiagnosticSource se conserva en teoría y como ampliación, no se elimina silenciosamente.

**Reto conservado/adaptado:** Diseña un observador de consultas lentas con un umbral configurable y explica qué aporta frente al interceptor de conteo.

## 4.12 - Estrategias de optimización y checklist de rendimiento

**Focos conservados:**

- Checklist, ciclo medir-identificar-aplicar-verificar-documentar y anti-patrones.
- Métricas de tiempo, comandos, volumen, memoria y coste de materialización.
- Estado acumulativo final de AceriaData y documentación de decisiones.

**Ejemplos teóricos de la fuente conservados/adaptados:** 9.

**Adaptación/corrección:** Se conserva el checklist, pero se corrige la idea de aplicar todas las técnicas a toda consulta. El cierre exige justificar también por qué Include, SplitQuery o CompiledQuery no aplican a una consulta concreta.

**Reto conservado/adaptado:** Audita una consulta completa y documenta cada decisión: aplicada, descartada y evidencia que la sustenta.
