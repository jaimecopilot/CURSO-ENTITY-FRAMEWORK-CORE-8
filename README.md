# Curso Entity Framework Core 8 — AceriaData

Curso profesional de **Entity Framework Core 8 sobre .NET 8**, desarrollado mediante un proyecto acumulativo de persistencia denominado **AceriaData**.

## Entorno principal

- Visual Studio Community
- .NET 8
- Entity Framework Core 8
- SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`)
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.31

SQL Server es el proveedor operativo principal del curso. El punto **5.10** está reservado al estudio específico de EF Core InMemory y SQLite in-memory para testing.

## Módulo 1 — Fundamentos de Entity Framework Core

- [Descripción del módulo](M01/README.md)
- [Teoría — Markdown](M01/TEORIA/M01_TEORIA.md)
- [Teoría — PDF](M01/TEORIA/M01_TEORIA.pdf)
- [Práctica — Markdown](M01/PRACTICA/M01_PRACTICA.md)
- [Práctica — PDF](M01/PRACTICA/M01_PRACTICA.pdf)
- [Trazabilidad](M01/TRAZABILIDAD_M01.md)
- Código acumulativo final: [src/AceriaData.Console](src/AceriaData.Console)
- Checkpoints: [checkpoints](checkpoints)

## Ejecución

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
cd src/AceriaData.Console
dotnet tool install --global dotnet-ef --version 8.0.31
dotnet ef migrations list
dotnet ef database update
dotnet run --configuration Release
```
