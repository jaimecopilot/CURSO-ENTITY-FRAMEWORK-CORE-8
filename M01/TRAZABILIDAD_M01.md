# Trazabilidad — Módulo 1

Cada punto enlaza el contenido docente con **el estado completo de AceriaData al terminar ese punto**.

| Punto | Cambio acumulativo | Estado completo del proyecto | Validación |
|---|---|---|---|
| 1.1 | Crear solución/proyecto y paquetes | `PROYECTO/1.1` | restore/build/run |
| 1.2 | DbContext + OrdenFabricacion + acceso inicial a LocalDB | `PROYECTO/1.2` | build/run + LocalDB |
| 1.3 | PlanchaAcero + Aleacion + InitialCreate + AddAleacion | `PROYECTO/1.3` | migrations/list/update + run |
| 1.4 | EstadoOrden + AddEstadoOrden + inspección Model/ChangeTracker | `PROYECTO/1.4` | migrations + esquema + run |
| 1.5 | Ciclo de vida y factoría manual | `PROYECTO/1.5` | build/run |
| 1.6 | CRUD + Find/Any/Count y consultas | `PROYECTO/1.6` | build/run + datos |
| 1.7 | Change Tracker, estados y DetectChanges | `PROYECTO/1.7` | build/run + estados |
| 1.8 | Add/Range/Update/Remove/Attach/Entry | `PROYECTO/1.8` | build/run |
| 1.9 | SaveChanges, async, errores y unidad de trabajo | `PROYECTO/1.9` | build/run + errores controlados |
| 1.10 | appsettings, opciones y logging | `PROYECTO/1.10` | build/run + SQL log |
| 1.11 | Auditoría del proveedor SQL Server y SQL generado | `PROYECTO/1.11` | ProviderName/ToQueryString/migrations/run |
| 1.12 | AddDbContext + repositorio + servicio + DI | `PROYECTO/1.12` | restore/build/migrations/run E2E |

## Regla de continuidad

Cada estado parte del anterior y conserva los elementos ya incorporados, salvo refactorizaciones explícitas necesarias para el nuevo punto. Ningún estado puede depender de conceptos que todavía no hayan sido introducidos: por ejemplo, la inyección de dependencias pertenece a 1.12 y no debe aparecer en 1.11.

El estado final de M1 es **`PROYECTO/1.12`** y será la base del primer punto del módulo siguiente.
