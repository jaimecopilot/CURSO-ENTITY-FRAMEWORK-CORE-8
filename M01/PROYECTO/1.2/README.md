# AceriaData — Punto 1.2: Arquitectura general de EF Core

Este directorio representa el estado acumulativo al finalizar **1.2** y parte del resultado construido en [1.1](../1.1). El directorio `PROYECTO/1.2` equivale a la carpeta raíz `AceriaData` del PDF.

## Estructura

```text
AceriaData/
├── AceriaData.sln
└── AceriaData.Console/
    ├── AceriaData.Console.csproj
    └── Program.cs
```

## Qué incorpora este punto

- Entidad `OrdenFabricacion`.
- `AceriaDbContext` con `DbSet<OrdenFabricacion>`.
- SQL Server LocalDB con la base `AceriaDB`, tal como indica la práctica.
- Código de creación con `EnsureCreated`.
- Inserción de una orden.
- Consulta de órdenes.
- Variante de diagnóstico sin `EnsureCreated`.
- Estado final activo que elimina la base de prototipo con `EnsureDeleted` para preparar el punto 1.3.

Las fases que el PDF propone probar durante el ejercicio permanecen en `Program.cs` como bloques comentados. El alumno puede comentar el bloque activo y descomentar la fase que quiera repetir.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El estado activo de cierre ejecuta `EnsureDeleted()`, porque el PDF exige dejar de usar la base inicializada con `EnsureCreated` antes de entrar en 1.3.

## Fuente pedagógica

La especificación de este checkpoint es el punto **1.2** de `M01_PRACTICA.pdf`. El siguiente estado acumulativo es [1.3](../1.3).
