# Trazabilidad - Módulo 2

M2 continúa físicamente desde `M01/PROYECTO/1.12`. Cada punto produce un estado completo de AceriaData y su propia solución de Visual Studio.

| Punto | Contenido canónico | Estado ejecutable | Control de cronología |
|---|---|---|---|
| 2.1 | Convenciones | `PROYECTO/2.1/AceriaData.sln` | No introduce DetalleOrden, OrdenAleacion, annotations ni filtros |
| 2.2 | Entidades y propiedades | `PROYECTO/2.2/AceriaData.sln` | Añade propiedades; evolución por Migrations |
| 2.3 | Uno a muchos | `PROYECTO/2.3/AceriaData.sln` | Sólo OrdenFabricacion -> Planchas |
| 2.4 | Uno a uno | `PROYECTO/2.4/AceriaData.sln` | Añade DetalleOrden y CertificadoCalidad |
| 2.5 | Muchos a muchos | `PROYECTO/2.5/AceriaData.sln` | Añade OrdenAleacion explícita |
| 2.6 | Data Annotations | `PROYECTO/2.6/AceriaData.sln` | Usa PrimaryKey para la clave compuesta |
| 2.7 | Fluent API | `PROYECTO/2.7/AceriaData.sln` | Centraliza configuración sin adelantar filtros |
| 2.8 | Claves | `PROYECTO/2.8/AceriaData.sln` | HasKey y HasAlternateKey |
| 2.9 | Índices y restricciones | `PROYECTO/2.9/AceriaData.sln` | Índices simples/compuestos/filtrados y CHECK |
| 2.10 | Filtros globales | `PROYECTO/2.10/AceriaData.sln` | HasQueryFilter; todavía no IsDeleted |
| 2.11 | Migraciones en el modelado + Soft Delete | `PROYECTO/2.11/AceriaData.sln` | `M2_2_11` añade IsDeleted/DeletedAt; se valida Up/Down, rollback, snapshot, scripts y restauración |
| 2.12 | Clean/Hexagonal | `PROYECTO/2.12/AceriaData.sln` | Domain/Application sin EF Core; Infrastructure lo contiene |

## Decisiones técnicas del módulo

- 2.1: la convención de clave se trabaja con `Id` o `<NombreDelTipo>Id`; las relaciones muchos-a-muchos se reservan para 2.5.
- 2.3: el alcance se limita a la relación uno-a-muchos `OrdenFabricacion -> PlanchaAcero`; `OrdenAleacion` se introduce en 2.5.
- 2.4: la relación uno-a-uno se configura una sola vez, con `DetalleOrden` como dependiente y `OrdenId` como FK requerida.
- 2.6: las claves compuestas se expresan con `[PrimaryKey(...)]`; en 2.8 se estudia la alternativa `HasKey`.
- 2.7: Fluent API se introduce sin adelantar el desarrollo específico de claves, índices/restricciones y filtros de 2.8, 2.9 y 2.10.
- 2.10 estudia filtros globales y 2.11 usa la incorporación de Soft Delete como caso real para estudiar el ciclo completo de una migración incremental: generación, revisión, aplicación, rollback, snapshot y scripts.
- 2.12 utiliza Repositorio y Unidad de Trabajo como puertos de aplicación dentro de la separación Domain/Application/Infrastructure/Console.
- Todo M2 usa Migrations; se excluye `EnsureCreated()` del flujo docente.
