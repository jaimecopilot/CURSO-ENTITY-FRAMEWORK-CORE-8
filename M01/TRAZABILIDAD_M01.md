# Trazabilidad — Módulo 1

Cada punto enlaza el contenido docente con **una solución completa y autónoma de Visual Studio** situada en `M01/PROYECTO/1.x/AceriaData.sln`.

## Fuente canónica de la práctica

La práctica de M01 se reconstruye desde las fuentes originales aportadas por el usuario. No se usa el código existente para decidir qué debe decir la práctica.

| Punto | Fuente | Rango práctico | Estado canónico | Corrección aplicada |
|---|---|---:|---|---|
| 1.1 | `Texto pegado(20260927-090808).txt` | 162–347 | última versión completa localizada | FORMATO + versión EF Core 8 alineada |
| 1.2 | `Texto pegado(20260927-090808).txt` | 545–855 | restaurado desde fuente | OMISIÓN corregida: se recuperan `PlanchaAcero`, relación, pasos y reto |
| 1.3 | `Texto pegado(20260927-090808).txt` | 1035–1403 | restaurado desde fuente | se retiran expansiones no trazables y se mantiene la migración `InitialCreate` + reto `Aleacion` |
| 1.4 | `Texto pegado(20260927-090808).txt` | 1560–1843 | fuente + adaptación acumulativa | `Aleacion` ya procede del reto 1.3; no se vuelve a introducir como si fuera nueva |
| 1.5 | `Texto pegado(20260927-090808).txt` | 2005–2369 | fuente + adaptación acumulativa | `EstadoOrden` ya procede del reto 1.4; después de migraciones no se mezcla `EnsureCreated` con `Migrate` |
| 1.6 | `Texto pegado (2)(3).txt` | 315–755 | última versión completa localizada | CORRECCIÓN TÉCNICA: continuidad con migraciones y rutas/namespaces reales |
| 1.7 | `Texto pegado (2)(3).txt` | 1001–1489 | última versión completa localizada | CORRECCIÓN TÉCNICA: continuidad con migraciones y rutas/namespaces reales |
| 1.8 | `Texto pegado (2)(3).txt` | 1772–2240 | última versión completa localizada | CORRECCIÓN TÉCNICA: continuidad con migraciones y rutas/namespaces reales |
| 1.9 | `Texto pegado (3)(3).txt` | 258–714 | última versión completa localizada | precisión sobre atomicidad/transacción de `SaveChanges`; continuidad con migraciones |
| 1.10 | `Texto pegado(20260928-113102).txt` | 169–438 | versión posterior que sustituye el 1.10 del 27/09 | CORRECCIÓN ACUMULATIVA aprobada: fundamentos de migraciones sobre la cadena real `InitialCreate → AddAleacion → AddEstadoOrden`; no adelantar DI/arquitectura del M2 |
| 1.11 | `Texto pegado (3)(3).txt` | 1608–2099 | fuente original + parche explícito aprobado | SQL Server/LocalDB es el único proveedor operativo; SQLite/PostgreSQL se conservan como lectura no ejecutable |
| 1.12 | `Texto pegado (3)(3).txt` | 2341–2867 | última versión completa localizada | CORRECCIÓN TÉCNICA: `Database.Migrate()`, SQL Server/LocalDB y versiones alineadas |

### Versiones descartadas o subordinadas

- El 1.10 original del 27/09, dedicado a configuración/cadena de conexión/logging, queda **supersedido** por la versión completa del 28/09 que fija 1.10 como fundamentos de migraciones.
- La versión del 28/09 contiene fragmentos que adelantan arquitectura/DI y campos de módulos posteriores. Esos fragmentos no se trasladan literalmente al resultado: se aplica la corrección acumulativa aprobada para mantener M01 en su nivel y conservar la cadena real de migraciones.
- En 1.11 no existe aprobación para una reescritura libre de 25 pasos. Por eso se vuelve a la fuente y sólo se aplica el parche explícito de proveedor operativo: los ejemplos alternativos quedan identificados como **NO EJECUTAR EN M01**.
- `Texto pegado (6).txt` queda descartado: pertenece al curso SGT/Claude Code y no es fuente de AceriaData.

## Clasificación de diferencias frente al MD previo

| Punto | Clasificación principal | Resultado |
|---|---|---|
| 1.1 | FORMATO / CORRECCIÓN TÉCNICA | se conserva |
| 1.2 | OMISIÓN | corregida desde fuente |
| 1.3 | AÑADIDO NO TRAZABLE / OMISIÓN | restaurado a la estructura fuente |
| 1.4 | ADAPTACIÓN ACUMULATIVA | justificada por el reto 1.3 |
| 1.5 | ADAPTACIÓN ACUMULATIVA / CORRECCIÓN TÉCNICA | justificada por el reto 1.4 y por migraciones |
| 1.6 | CORRECCIÓN TÉCNICA | `Migrate` tras introducir migraciones |
| 1.7 | CORRECCIÓN TÉCNICA | `Migrate` tras introducir migraciones |
| 1.8 | CORRECCIÓN TÉCNICA | `Migrate` tras introducir migraciones |
| 1.9 | CORRECCIÓN TÉCNICA | precisión de comportamiento de `SaveChanges` |
| 1.10 | CORRECCIÓN TÉCNICA / ADAPTACIÓN ACUMULATIVA | se usa la versión revisada y la cadena real existente |
| 1.11 | AÑADIDO NO TRAZABLE retirado / CORRECCIÓN TÉCNICA | se restaura la fuente y se neutralizan variantes de proveedor |
| 1.12 | CORRECCIÓN TÉCNICA | DI sobre SQL Server y migraciones |

## Proyecto acumulativo

| Punto | Cambio acumulativo | Solución local | Validación |
|---|---|---|---|
| 1.1 | Crear proyecto y paquetes | `PROYECTO/1.1/AceriaData.sln` | restore/build/run |
| 1.2 | DbContext + OrdenFabricacion + PlanchaAcero + LocalDB | `PROYECTO/1.2/AceriaData.sln` | restore/build/run + LocalDB |
| 1.3 | componentes EF Core + `InitialCreate` + `Aleacion` | `PROYECTO/1.3/AceriaData.sln` | restore/build/migrations/run |
| 1.4 | DbContext + continuidad de `Aleacion` + `EstadoOrden` | `PROYECTO/1.4/AceriaData.sln` | restore/build/migrations/run |
| 1.5 | ciclo de vida y factoría manual | `PROYECTO/1.5/AceriaData.sln` | restore/build/run |
| 1.6 | CRUD y operaciones DbSet | `PROYECTO/1.6/AceriaData.sln` | restore/build/run |
| 1.7 | Change Tracker | `PROYECTO/1.7/AceriaData.sln` | restore/build/run |
| 1.8 | gestión de entidades | `PROYECTO/1.8/AceriaData.sln` | restore/build/run |
| 1.9 | SaveChanges y UoW | `PROYECTO/1.9/AceriaData.sln` | restore/build/run |
| 1.10 | migraciones: cadena real, Up/Down, snapshot, historial, aplicación y rollback | `PROYECTO/1.10/AceriaData.sln` | validar `InitialCreate → AddAleacion → AddEstadoOrden` |
| 1.11 | proveedores: SQL Server operativo; alternativas sólo de lectura | `PROYECTO/1.11/AceriaData.sln` | restore/build/migrations/run; ausencia de paquetes alternativos |
| 1.12 | DI + repositorio + servicio | `PROYECTO/1.12/AceriaData.sln` | restore/build/migrations/E2E |

## Regla de continuidad

Cada solución parte del estado anterior y conserva sus elementos, salvo refactorizaciones explícitas necesarias para el nuevo punto. Ninguna solución puede depender de conceptos todavía no introducidos.

No hay una solución global en la raíz del repositorio: **la unidad de trabajo docente es la solución local de cada punto**.

## Certificación editorial y visual del PDF canónico

- Branch editorial: `fix/revision-editorial-pdf-m01`
- HEAD que contiene el PDF regenerado: `0c1cdc71678caa15874611c968bb6396db1213b1`
- Workflow de generación/QA: `37624839377`
- Resultado del workflow: **SUCCESS**
- Artefacto de revisión visual: `m01-practica-visual-qa` (artifact `11483537556`)
- PDF oficial: `M01/PRACTICA/M01_PRACTICA.pdf`
- Blob Git del PDF: `cd29e19c18482b2c57a6e09c4e244876d5ef53a4`
- SHA-256 del PDF descargado y revisado: `dc57b60c14bfb85dbd53e82f341a43e49917a0cb8de5683009ffdf874de219bb`
- Formato: A4
- Páginas del PDF oficial: **93**
- Previews revisados: **1.1–1.12 completos, 92 páginas renderizadas**

### Revisión visual real

Se inspeccionaron para **cada punto 1.1–1.12** la primera página, una página densa de código o explicación, la página del reto/tabla de errores cuando aplica y la página final. Además, todas las páginas de los previews se renderizaron a PNG y se comprobaron los límites de contenido.

Resultado:

- clipping: **NO detectado**;
- overflow: **NO detectado**;
- texto cortado: **NO detectado**;
- bloques de código mal cerrados o partidos de forma ilegible: **NO detectado**;
- huecos anómalos que oculten contenido: **NO detectado**;
- caracteres de sustitución `�`: **0**;
- escapes de transporte `\\n` visibles: **0**;
- residuos `Fin del Punto`: **0**;
- cercas Markdown visibles en el PDF: **0**;
- contenido tocando los bordes físicos de las 92 páginas de preview: **0 páginas**.

Muestras visuales revisadas:

| Punto | Páginas de preview inspeccionadas |
|---|---|
| 1.1 | 1, 2, 4, 5 |
| 1.2 | 1, 5, 6, 7 |
| 1.3 | 1, 5, 6, 7 |
| 1.4 | 1, 3, 4, 5 |
| 1.5 | 1, 3, 6, 7 |
| 1.6 | 1, 4, 7, 8 |
| 1.7 | 1, 5, 7, 9 |
| 1.8 | 1, 5, 7, 9 |
| 1.9 | 1, 5, 7, 8 |
| 1.10 | 1, 6, 7, 8 |
| 1.11 | 1, 5, 8, 9 |
| 1.12 | 1, 5, 8, 10 |

**Conclusión editorial:** el nuevo `M01_PRACTICA.md/PDF` queda cerrado como contrato pedagógico para la reconciliación de `M01/PROYECTO/1.1–1.12`. A partir de este punto la dirección de validación es PDF → CÓDIGO → E2E.

## Certificación técnica PDF → CÓDIGO → E2E

- Branch técnica: `fix/e2e-pedagogico-m01`
- HEAD técnico certificado: `1a801e0e30bf5aa7e63d6c0a2c7cb7a0caa74f97`
- Workflow E2E: `37630871256`
- Resultado: **SUCCESS**
- Contrato canónico validado: **158 pasos**
- Bloques C# del PDF preservados literalmente y comentados: **63**
- Checkpoints funcionales certificados: **1.1–1.12**
- Base de datos operativa: **AceriaDB sobre SQL Server LocalDB**
- Desde 1.3: **Database.Migrate()**, sin mezclar `EnsureCreated()`
- Cadena de migraciones certificada: `20260927000100_InitialCreate → 20260927000200_AddAleacion → 20260927000300_AddEstadoOrden`
- Reto acumulativo 1.10: rollback a `InitialCreate`, reaplicación explícita de `AddAleacion` y vuelta al estado final: **PASS**
- Punto 1.11: `Microsoft.EntityFrameworkCore.SqlServer` es el único proveedor ejecutable; SQLite/PostgreSQL permanecen sólo como bloques pedagógicos comentados: **PASS**
- Punto 1.12: `AddDbContext`, repositorio, scopes y servicio de negocio: **PASS**

### Regresiones corregidas durante la reconciliación

- 1.1 no representaba el reto final del PDF; se alineó el `Program.cs`.
- 1.2 había perdido `PlanchaAcero`, su `DbSet` y el escenario final con dos planchas; se reconstruyó desde el contrato canónico.
- El reto 1.2 usa `Include(o => o.Planchas)` pero la fuente no declaraba explícitamente la navegación inversa en ese paso. Se añadió `List<PlanchaAcero> Planchas` como **corrección técnica mínima** necesaria para que el código del propio PDF compile y se ejecute.
- La navegación `OrdenFabricacion.Planchas` desaparecía después en 1.5–1.8; se preservó de forma acumulativa.
- 1.6 había perdido el `AsNoTracking()` pedido por la práctica; se restauró.
- Algunos checkpoints usaban nombres internos `AceriaDB_CPxx`; se normalizaron a la base canónica `AceriaDB`.
- Se verificó que no quedan proveedores alternativos ejecutables en 1.11.

**Conclusión técnica:** M01 queda cerrado contra el PDF canónico nuevo. La integración final debe copiar únicamente los artefactos finales y excluir la infraestructura temporal de validación.
