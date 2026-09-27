# Curso Entity Framework Core 8 — AceriaData

Repositorio del curso profesional de **Entity Framework Core 8 sobre .NET 8**.

## Estado actual

El material fuente entregado permite cerrar de forma completa el **Módulo 1 — Fundamentos de Entity Framework Core (1.1–1.12)**. Los módulos 2–5 aparecen definidos en la guía de trazabilidad, pero su teoría/práctica fuente completa no fue incluida en los ficheros recibidos; por ello no se inventa ni se genera contenido docente para esos módulos.

## Entorno principal

- Visual Studio Community
- .NET 8
- Entity Framework Core 8
- SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`)
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.31

SQL Server es el proveedor operativo del curso. La excepción futura expresamente prevista por el temario es **5.10**, dedicado a EF Core InMemory y SQLite in-memory para testing.

## Módulo disponible

- [M01/README.md](M01/README.md)
- [M01/TEORIA/M01_TEORIA.md](M01/TEORIA/M01_TEORIA.md)
- [M01/PRACTICA/M01_PRACTICA.md](M01/PRACTICA/M01_PRACTICA.md)
- [M01/TRAZABILIDAD_M01.md](M01/TRAZABILIDAD_M01.md)
- [M01/CORRECCIONES_TECNICAS.md](M01/CORRECCIONES_TECNICAS.md)
- Código acumulativo final: [src/AceriaData.Console](src/AceriaData.Console)
- Checkpoints: [checkpoints](checkpoints)

## Validación local

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
cd src/AceriaData.Console
dotnet tool install --global dotnet-ef --version 8.0.31
dotnet ef migrations list
dotnet run --configuration Release
```

La automatización de GitHub valida el proyecto en un runner Windows con SQL Server LocalDB.

## Validación automatizada

El workflow `.github/workflows/validate-m1.yml` compila el estado final, valida migraciones y ejecución sobre SQL Server LocalDB, compila/ejecuta los 12 checkpoints y comprueba la estructura documental.
