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
