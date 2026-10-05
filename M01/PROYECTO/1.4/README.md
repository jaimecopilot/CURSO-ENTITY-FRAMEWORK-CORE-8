# AceriaData — Punto 1.4: DbContext, responsabilidades y DbSet

Este checkpoint se construye acumulativamente desde [1.3](../1.3) siguiendo el punto **1.4** de `M01_PRACTICA.pdf`.

## Qué añade 1.4

- Conserva `Aleacion` y su migración del punto 1.3.
- Explora `Database`, `Model`, `ChangeTracker` y `SaveChanges`.
- Añade `EstadoOrden`.
- Añade `DbSet<EstadoOrden> EstadosOrden`.
- Genera únicamente la nueva migración `AddEstadoOrden`.
- Conserva los `Designer` y el `ModelSnapshot` reales de EF Core.
- Deja el reto final de inspección del modelo como escenario activo.
- Conserva las demás pruebas del PDF como bloques comentados y activables.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list --project AceriaData.Console/AceriaData.Console.csproj
dotnet ef database update --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La base usada por la práctica continúa siendo `AceriaDB`.

## Experimentación

En `Program.cs` queda activo el último reto del punto. Para repetir cualquiera de las demostraciones anteriores, comenta el bloque activo y descomenta el bloque correspondiente a:

- inspección de `Database` y `Model`;
- número de filas devueltas por `SaveChanges`;
- estados del Change Tracker antes y después de guardar.

El siguiente checkpoint acumulativo es [1.5](../1.5).
