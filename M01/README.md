# Módulo 1 — Fundamentos de Entity Framework Core

Este módulo desarrolla los puntos 1.1 a 1.12 mediante teoría, práctica y una evolución acumulativa del proyecto **AceriaData** sobre .NET 8, Entity Framework Core 8 y SQL Server LocalDB.

## Regla del proyecto acumulativo

El directorio [PROYECTO](PROYECTO) contiene un estado completo por punto:

`1.1 → 1.2 → 1.3 → 1.4 → 1.5 → 1.6 → 1.7 → 1.8 → 1.9 → 1.10 → 1.11 → 1.12`

Cada carpeta es un proyecto ejecutable completo. El estado de un punto se construye continuando el punto anterior; no son ejemplos independientes. **1.12 es el estado final del módulo**.

## Índice de prácticas y estados del proyecto

| Punto | Tema | Proyecto acumulativo |
|---|---|---|
| 1.1 | Introducción al ORM y preparación | [PROYECTO/1.1](PROYECTO/1.1) |
| 1.2 | Arquitectura general de EF Core | [PROYECTO/1.2](PROYECTO/1.2) |
| 1.3 | Componentes principales y migraciones | [PROYECTO/1.3](PROYECTO/1.3) |
| 1.4 | DbContext, responsabilidades y DbSet | [PROYECTO/1.4](PROYECTO/1.4) |
| 1.5 | Ciclo de vida del DbContext | [PROYECTO/1.5](PROYECTO/1.5) |
| 1.6 | DbSet y operaciones básicas | [PROYECTO/1.6](PROYECTO/1.6) |
| 1.7 | Change Tracker | [PROYECTO/1.7](PROYECTO/1.7) |
| 1.8 | Gestión de entidades | [PROYECTO/1.8](PROYECTO/1.8) |
| 1.9 | SaveChanges y unidad de trabajo | [PROYECTO/1.9](PROYECTO/1.9) |
| 1.10 | Configuración, conexión y logging | [PROYECTO/1.10](PROYECTO/1.10) |
| 1.11 | Proveedores de datos con SQL Server | [PROYECTO/1.11](PROYECTO/1.11) |
| 1.12 | Inyección de dependencias y AddDbContext | [PROYECTO/1.12](PROYECTO/1.12) |

## Qué significa “terminar un punto”

Para considerar válido un punto deben cumplirse conjuntamente estas condiciones:

1. el contenido del documento de práctica describe el cambio;
2. la carpeta `PROYECTO/1.x` contiene el proyecto completo resultante;
3. el proyecto restaura paquetes, compila y se ejecuta;
4. conserva todo lo introducido en los puntos anteriores;
5. no adelanta conceptos reservados a puntos posteriores;
6. la CI puede trazar el contenido de la práctica hacia ese estado de código.

## Validación de M1

La CI de M1 compila y ejecuta **los doce estados acumulativos** y valida además el estado final contra SQL Server LocalDB, migraciones, E2E y trazabilidad práctica ↔ código.

La auditoría automática verifica, entre otras reglas, que:

- `PlanchaAcero` y `Aleacion` no aparezcan antes de 1.3;
- `EstadoOrden` no aparezca antes de 1.4;
- la configuración externa no se adelante a 1.10;
- DI, repositorio y servicio no aparezcan antes de 1.12;
- `1.12` sea el estado final utilizado por `AceriaData.sln`.

## Material docente

- [Teoría — Markdown](TEORIA/M01_TEORIA.md)
- [Teoría — PDF](TEORIA/M01_TEORIA.pdf)
- [Práctica — README](PRACTICA/README.md)
- [Práctica — Markdown](PRACTICA/M01_PRACTICA.md)
- [Práctica — PDF](PRACTICA/M01_PRACTICA.pdf)
- [Proyecto acumulativo — índice](PROYECTO/README.md)
- [Trazabilidad](TRAZABILIDAD_M01.md)
