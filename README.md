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

Cada carpeta de punto contiene **un proyecto completo, compilable y ejecutable**. La misma regla se aplicará a M2, M3, M4 y M5.

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

Los doce estados acumulativos `1.1 → 1.12` han sido sometidos a CI sobre Windows, .NET 8 y SQL Server LocalDB:

- restauración y compilación de los **12 proyectos**;
- ejecución de los **12 proyectos**;
- validación de migraciones y actualización de la base en el estado final;
- ejecución E2E del estado final `1.12`;
- trazabilidad automática de cada punto de práctica con `M01/PROYECTO/1.x`;
- compilación y ejecución adicional del código extraído del documento de prácticas cuando el punto contiene un bloque ejecutable completo;
- control de evolución acumulativa para impedir que un punto pierda elementos anteriores o adelante contenidos de puntos posteriores;
- revisión automática de estructura documental y PDF.

La auditoría de código finaliza con:

```text
AUDITORÍA M1 PASS:
práctica, estados acumulativos 1.1→1.12,
código fuente y E2E trazados.
```

## Ejecutar el estado final de M1

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release

cd M01/PROYECTO/1.12
dotnet tool install --global dotnet-ef --version 8.0.31
dotnet ef migrations list
dotnet ef database update
dotnet run --configuration Release
```

Para trabajar un punto concreto, entrar en su carpeta de `M01/PROYECTO` y consultar su README.
