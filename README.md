# Curso Entity Framework Core 8 — AceriaData

Curso profesional de **Entity Framework Core 8 sobre .NET 8**, desarrollado mediante el proyecto acumulativo **AceriaData**.

## Entorno principal

- Visual Studio Community
- .NET 8
- Entity Framework Core 8
- SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`)
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.31

SQL Server es el proveedor operativo principal. El bloque específico de testing del módulo 5 estudiará EF Core InMemory y SQLite in-memory.

## Regla de construcción del código

El código se organiza **dentro de cada módulo**. Cada punto contiene una copia completa y ejecutable del proyecto tal como debe quedar al terminar ese punto.

```text
M01/
└── PROYECTO/
    ├── 1.1/
    ├── 1.2/
    ├── 1.3/
    ├── ...
    └── 1.12/
```

`1.2` continúa desde el estado terminado de `1.1`; `1.3` continúa desde `1.2`; y así sucesivamente. El estado final de M1 es **`M01/PROYECTO/1.12`**. La misma regla se aplicará a los módulos siguientes: el primer punto de un módulo parte del último estado válido del módulo anterior.

## Módulo 1 — Fundamentos de Entity Framework Core

- [Descripción del módulo](M01/README.md)
- [Teoría — Markdown](M01/TEORIA/M01_TEORIA.md)
- [Teoría — PDF](M01/TEORIA/M01_TEORIA.pdf)
- [Práctica — Markdown](M01/PRACTICA/M01_PRACTICA.md)
- [Práctica — PDF](M01/PRACTICA/M01_PRACTICA.pdf)
- [Proyecto acumulativo por punto](M01/PROYECTO)
- [Trazabilidad](M01/TRAZABILIDAD_M01.md)

## Ejecución del estado final de M1

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
cd M01/PROYECTO/1.12
dotnet tool install --global dotnet-ef --version 8.0.31
dotnet ef migrations list
dotnet ef database update
dotnet run --configuration Release
```
