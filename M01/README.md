# Módulo 1 — Fundamentos de Entity Framework Core

Este módulo desarrolla los puntos 1.1 a 1.12 mediante teoría, práctica y una evolución acumulativa del proyecto **AceriaData** sobre .NET 8, Entity Framework Core 8 y SQL Server LocalDB.

## Regla del proyecto acumulativo

El directorio [PROYECTO](PROYECTO) contiene un estado completo por punto:

`1.1 → 1.2 → 1.3 → 1.4 → 1.5 → 1.6 → 1.7 → 1.8 → 1.9 → 1.10 → 1.11 → 1.12`

Cada carpeta contiene **su propia solución `AceriaData.sln` y el proyecto local `AceriaData.Console.csproj`**. No hay una solución global en la raíz del repositorio. Cada estado se abre directamente en Visual Studio y continúa el estado anterior.

## Índice de prácticas y estados del proyecto

| Punto | Tema | Solución autónoma |
|---|---|---|
| 1.1 | Introducción al ORM y preparación | [PROYECTO/1.1/AceriaData.sln](PROYECTO/1.1/AceriaData.sln) |
| 1.2 | Arquitectura general de EF Core | [PROYECTO/1.2/AceriaData.sln](PROYECTO/1.2/AceriaData.sln) |
| 1.3 | Componentes principales y migraciones | [PROYECTO/1.3/AceriaData.sln](PROYECTO/1.3/AceriaData.sln) |
| 1.4 | DbContext, responsabilidades y DbSet | [PROYECTO/1.4/AceriaData.sln](PROYECTO/1.4/AceriaData.sln) |
| 1.5 | Ciclo de vida del DbContext | [PROYECTO/1.5/AceriaData.sln](PROYECTO/1.5/AceriaData.sln) |
| 1.6 | DbSet y operaciones básicas | [PROYECTO/1.6/AceriaData.sln](PROYECTO/1.6/AceriaData.sln) |
| 1.7 | Change Tracker | [PROYECTO/1.7/AceriaData.sln](PROYECTO/1.7/AceriaData.sln) |
| 1.8 | Gestión de entidades | [PROYECTO/1.8/AceriaData.sln](PROYECTO/1.8/AceriaData.sln) |
| 1.9 | SaveChanges y unidad de trabajo | [PROYECTO/1.9/AceriaData.sln](PROYECTO/1.9/AceriaData.sln) |
| 1.10 | Introducción práctica a las migraciones: generación, aplicación y seguimiento | [PROYECTO/1.10/AceriaData.sln](PROYECTO/1.10/AceriaData.sln) |
| 1.11 | Proveedores de datos con SQL Server | [PROYECTO/1.11/AceriaData.sln](PROYECTO/1.11/AceriaData.sln) |
| 1.12 | Inyección de dependencias y AddDbContext | [PROYECTO/1.12/AceriaData.sln](PROYECTO/1.12/AceriaData.sln) |

## Qué significa terminar un punto

Un punto sólo se considera válido cuando su carpeta contiene una solución autónoma, restaura, compila y se ejecuta, conserva todo lo anterior, no adelanta contenidos posteriores y está trazada con la práctica correspondiente.

## Material docente

- [Teoría](TEORIA/M01_TEORIA.md)
- [Práctica](PRACTICA/M01_PRACTICA.md)
- [Índice de prácticas](PRACTICA/README.md)
- [Índice de soluciones](PROYECTO/README.md)
- [Trazabilidad](TRAZABILIDAD_M01.md)
