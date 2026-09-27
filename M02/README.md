# Módulo 2 - Modelado de datos con Entity Framework Core 8

M2 continúa exactamente desde el estado final validado de M1: `M01/PROYECTO/1.12`.

## Secuencia acumulativa

`2.1 -> 2.2 -> 2.3 -> 2.4 -> 2.5 -> 2.6 -> 2.7 -> 2.8 -> 2.9 -> 2.10 -> 2.11 -> 2.12`

Cada carpeta de [PROYECTO](PROYECTO) es un estado completo del mismo sistema y contiene su propia `AceriaData.sln`. Los puntos 2.1-2.11 mantienen el proyecto de consola acumulativo; 2.12 reorganiza el mismo código en Domain, Application, Infrastructure y Console.

## Material docente

- [Teoría - Markdown](TEORIA/M02_TEORIA.md)
- [Teoría - PDF](TEORIA/M02_TEORIA.pdf)
- [Práctica - índice](PRACTICA/README.md)
- [Práctica - Markdown](PRACTICA/M02_PRACTICA.md)
- [Práctica - PDF](PRACTICA/M02_PRACTICA.pdf)
- [Proyecto acumulativo 2.1-2.12](PROYECTO/README.md)
- [Trazabilidad práctica, código y correcciones](TRAZABILIDAD_M02.md)

## Temario canónico

| Punto | Tema | Solución |
|---|---|---|
| 2.1 | Convenciones de modelado | [2.1](PROYECTO/2.1/AceriaData.sln) |
| 2.2 | Entidades y propiedades | [2.2](PROYECTO/2.2/AceriaData.sln) |
| 2.3 | Relaciones uno a muchos | [2.3](PROYECTO/2.3/AceriaData.sln) |
| 2.4 | Relaciones uno a uno | [2.4](PROYECTO/2.4/AceriaData.sln) |
| 2.5 | Relaciones muchos a muchos | [2.5](PROYECTO/2.5/AceriaData.sln) |
| 2.6 | Data Annotations | [2.6](PROYECTO/2.6/AceriaData.sln) |
| 2.7 | Fluent API | [2.7](PROYECTO/2.7/AceriaData.sln) |
| 2.8 | Claves primarias, alternativas y compuestas | [2.8](PROYECTO/2.8/AceriaData.sln) |
| 2.9 | Índices y restricciones | [2.9](PROYECTO/2.9/AceriaData.sln) |
| 2.10 | Filtros globales de consulta | [2.10](PROYECTO/2.10/AceriaData.sln) |
| 2.11 | Soft Delete | [2.11](PROYECTO/2.11/AceriaData.sln) |
| 2.12 | Clean Architecture y Arquitectura Hexagonal | [2.12](PROYECTO/2.12/AceriaData.sln) |

## Reglas técnicas fijadas

M2 utiliza SQL Server LocalDB y Migrations para evolucionar `AceriaDB`. No se usa `EnsureCreated()`. Los filtros globales se introducen en 2.10, Soft Delete en 2.11 y la separación arquitectónica en 2.12. La CI comprueba la cronología, compila y ejecuta todos los estados y valida la trazabilidad literal del código mostrado en la práctica.
