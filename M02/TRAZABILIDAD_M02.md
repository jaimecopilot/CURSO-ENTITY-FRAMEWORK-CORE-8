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
| 2.11 | Soft Delete | `PROYECTO/2.11/AceriaData.sln` | IsDeleted, DeletedAt, restauración |
| 2.12 | Clean/Hexagonal | `PROYECTO/2.12/AceriaData.sln` | Domain/Application sin EF Core; Infrastructure lo contiene |

## Correcciones aplicadas al material fuente

- 2.1: la convención de clave es `Id` o `<NombreDelTipo>Id`; se retira el reto muchos-a-muchos adelantado.
- 2.3: el reto de `OrdenAleacion` se mueve conceptualmente a 2.5.
- 2.4: la relación uno-a-uno se configura una sola vez; la nulabilidad de la navegación principal no exige una segunda configuración contradictoria.
- 2.6: una clave compuesta se expresa con `[PrimaryKey(...)]` o `HasKey`, no con dos atributos `[Key]` independientes.
- 2.7: claves, índices/restricciones y filtros se reservan para 2.8, 2.9 y 2.10.
- 2.10 y 2.11: se restituye la separación original entre filtros globales y Soft Delete.
- 2.12: el contenido útil de Repositorio/Unidad de Trabajo se integra en la arquitectura limpia como puertos de aplicación.
- Todo M2 usa Migrations; se excluye `EnsureCreated()` del flujo docente.
