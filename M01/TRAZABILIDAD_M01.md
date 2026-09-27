# Trazabilidad — Módulo 1

Cada punto enlaza el contenido docente con **una solución completa y autónoma de Visual Studio** situada en `M01/PROYECTO/1.x/AceriaData.sln`.

| Punto | Cambio acumulativo | Solución local | Validación |
|---|---|---|---|
| 1.1 | Crear proyecto y paquetes | `PROYECTO/1.1/AceriaData.sln` | restore/build/run |
| 1.2 | DbContext + OrdenFabricacion + LocalDB | `PROYECTO/1.2/AceriaData.sln` | restore/build/run + LocalDB |
| 1.3 | PlanchaAcero + Aleacion + migraciones | `PROYECTO/1.3/AceriaData.sln` | restore/build/migrations/run |
| 1.4 | EstadoOrden + evolución del modelo | `PROYECTO/1.4/AceriaData.sln` | restore/build/migrations/run |
| 1.5 | Ciclo de vida y factoría manual | `PROYECTO/1.5/AceriaData.sln` | restore/build/run |
| 1.6 | CRUD y operaciones DbSet | `PROYECTO/1.6/AceriaData.sln` | restore/build/run |
| 1.7 | Change Tracker | `PROYECTO/1.7/AceriaData.sln` | restore/build/run |
| 1.8 | Gestión de entidades | `PROYECTO/1.8/AceriaData.sln` | restore/build/run |
| 1.9 | SaveChanges y UoW | `PROYECTO/1.9/AceriaData.sln` | restore/build/run |
| 1.10 | Configuración y logging | `PROYECTO/1.10/AceriaData.sln` | restore/build/run |
| 1.11 | Proveedor SQL Server y SQL generado | `PROYECTO/1.11/AceriaData.sln` | restore/build/run |
| 1.12 | DI + repositorio + servicio | `PROYECTO/1.12/AceriaData.sln` | restore/build/migrations/E2E |

## Regla de continuidad

Cada solución parte del estado anterior y conserva sus elementos, salvo refactorizaciones explícitas necesarias para el nuevo punto. Ninguna solución puede depender de conceptos todavía no introducidos.

No hay una solución global en la raíz del repositorio: **la unidad de trabajo docente es la solución local de cada punto**.
