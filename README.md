# Curso Entity Framework Core 8 — AceriaData

Curso profesional de **Entity Framework Core 8 sobre .NET 8**, desarrollado mediante el proyecto acumulativo **AceriaData**.

## Entorno principal

- Visual Studio Community
- .NET 8
- Entity Framework Core 8
- SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`)
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.31

SQL Server es el proveedor operativo principal. El bloque específico de testing del módulo 5 estudiará EF Core InMemory y SQLite in-memory.

## Regla obligatoria de construcción del código

El proyecto **no se reinicia en cada ejercicio**. Cada punto parte del proyecto terminado en el punto anterior y conserva todo lo ya incorporado, salvo refactorizaciones explícitas exigidas por el nuevo contenido.

```text
M01/PROYECTO/1.1
        ↓
M01/PROYECTO/1.2
        ↓
       ...
        ↓
M01/PROYECTO/1.12   ← estado final de M1
        ↓
M02/PROYECTO/2.1    ← partirá de M01/PROYECTO/1.12
```

**Cada carpeta de punto contiene su propia solución de Visual Studio `AceriaData.sln`.** No existe una solución central en la raíz del repositorio. Los estados simples contienen el proyecto ejecutable local; cuando el temario introduce una arquitectura multiproyecto, como M2.12, la solución local contiene todos los proyectos de ese estado.

La misma regla se aplicará a M2, M3, M4 y M5.

## Módulo 1 — Fundamentos de Entity Framework Core

- [README del módulo](M01/README.md)
- [Teoría — Markdown](M01/TEORIA/M01_TEORIA.md)
- [Teoría — PDF](M01/TEORIA/M01_TEORIA.pdf)
- [Práctica — índice y reglas](M01/PRACTICA/README.md)
- [Práctica — Markdown](M01/PRACTICA/M01_PRACTICA.md)
- [Práctica — PDF](M01/PRACTICA/M01_PRACTICA.pdf)
- [Proyecto acumulativo — índice 1.1 a 1.12](M01/PROYECTO/README.md)
- [Trazabilidad práctica ↔ proyecto](M01/TRAZABILIDAD_M01.md)

## Estado de validación de M1

Los doce estados acumulativos `1.1 → 1.12` se validan sobre Windows, .NET 8 y SQL Server LocalDB mediante:

- restauración y compilación de las **12 soluciones locales**;
- ejecución de los **12 proyectos**;
- validación de migraciones y actualización de la base en el estado final;
- ejecución E2E del estado final `1.12`;
- trazabilidad automática práctica ↔ estado de código;
- compilación y ejecución adicional de bloques completos extraídos del documento de prácticas cuando procede;
- control de continuidad acumulativa y de cronología de contenidos.

## Abrir o ejecutar el estado final de M1

En Visual Studio, abrir:

```text
M01/PROYECTO/1.12/AceriaData.sln
```

Desde consola:

```powershell
cd M01/PROYECTO/1.12
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet tool install --global dotnet-ef --version 8.0.31
dotnet ef migrations list
dotnet ef database update
dotnet run --project AceriaData.Console.csproj --configuration Release
```


## Módulo 2 - Modelado de datos

- [README del módulo](M02/README.md)
- [Teoría - Markdown](M02/TEORIA/M02_TEORIA.md)
- [Teoría - PDF](M02/TEORIA/M02_TEORIA.pdf)
- [Práctica - Markdown](M02/PRACTICA/M02_PRACTICA.md)
- [Práctica - PDF](M02/PRACTICA/M02_PRACTICA.pdf)
- [Proyecto acumulativo 2.1 a 2.12](M02/PROYECTO/README.md)
- [Trazabilidad M2](M02/TRAZABILIDAD_M02.md)


## Módulo 3 - Consultas con LINQ

- [README del módulo](M03/README.md)
- [Teoría - Markdown](M03/TEORIA/M03_TEORIA.md)
- [Teoría - PDF](M03/TEORIA/M03_TEORIA.pdf)
- [Práctica - índice](M03/PRACTICA/README.md)
- [Práctica - Markdown](M03/PRACTICA/M03_PRACTICA.md)
- [Práctica - PDF](M03/PRACTICA/M03_PRACTICA.pdf)
- [Proyecto acumulativo 3.1 a 3.12](M03/PROYECTO/README.md)
- [Trazabilidad M3](M03/TRAZABILIDAD_M03.md)

## Estado de validación de M3

Los doce estados acumulativos `3.1 -> 3.12` se validaron sobre Windows, .NET 8 y SQL Server LocalDB mediante restauración y compilación de las 12 soluciones locales, aplicación de la cadena de migraciones heredada de M2, ejecución funcional de los 12 estados, comprobación de trazabilidad y verificación del límite arquitectónico final.

## Módulo 4 - Optimización y rendimiento

- [README del módulo](M04/README.md)
- [Teoría - Markdown](M04/TEORIA/M04_TEORIA.md)
- [Teoría - PDF](M04/TEORIA/M04_TEORIA.pdf)
- [Práctica - Markdown](M04/PRACTICA/M04_PRACTICA.md)
- [Práctica - PDF](M04/PRACTICA/M04_PRACTICA.pdf)
- [Proyecto acumulativo 4.1 a 4.12](M04/PROYECTO/README.md)
- [Trazabilidad M4](M04/TRAZABILIDAD_M04.md)

## Estado de validación de M4

Los doce estados acumulativos `4.1 -> 4.12` parten físicamente de `M03/PROYECTO/3.12` y fueron validados sobre Windows, .NET 8 y SQL Server LocalDB. La validación final incluyó continuidad acumulativa, restore/build de las 12 soluciones, migraciones heredadas, ejecución funcional de todos los estados, frontera arquitectónica, trazabilidad y revisión completa de los PDF.

## Módulo 5 - Persistencia empresarial

- [README del módulo](M05/README.md)
- [Teoría - Markdown](M05/TEORIA/M05_TEORIA.md)
- [Teoría - PDF](M05/TEORIA/M05_TEORIA.pdf)
- [Práctica - Markdown](M05/PRACTICA/M05_PRACTICA.md)
- [Práctica - PDF](M05/PRACTICA/M05_PRACTICA.pdf)
- [Proyecto acumulativo 5.1 a 5.12](M05/PROYECTO/README.md)
- [Trazabilidad M5](M05/TRAZABILIDAD_M05.md)

Los doce estados `5.1 -> 5.12` continúan físicamente desde el Módulo 4 y trabajan sobre SQL Server LocalDB. El estado final conserva `M5_5_2_ConcurrencyTokens` como última migración del modelo y añade resolución de concurrencia, transacciones, despliegue, migraciones en equipo, Repository/Unit of Work, observabilidad, testing y refactorización de anti-patrones.

Los manuales de teoría y práctica se generan de forma reproducible con los scripts del repositorio. Los PDFs finales contienen 50 páginas de teoría y 70 de prácticas.


## Criterio de cierre global del curso

El temario definitivo del curso está compuesto por **cinco módulos (M1–M5), con 12 puntos por módulo y 60 puntos en total**. M5 es el último módulo del curso.

La versión definitiva fue sometida a una validación global conjunta de estructura, compilación, ejecución, migraciones, tests, trazabilidad y documentación antes de publicarse en `main`.


**Release definitiva del curso:** M1–M5, 60 puntos, validada mediante el gate global `Validate Course Final` sobre el HEAD publicado en `main`.


## Estado final del curso

El curso definitivo consta de **cinco módulos (M1–M5) y 60 puntos acumulativos**. Antes de esta publicación final se validaron conjuntamente la estructura completa, las soluciones de los 60 estados, las migraciones, las ejecuciones sobre SQL Server LocalDB, los tests incorporados en los módulos que los requieren, la trazabilidad de cada módulo y los diez manuales MD/PDF.

La rama `main` contiene únicamente la entrega formativa final: documentación, código, trazabilidad y scripts que forman parte de los propios laboratorios.
