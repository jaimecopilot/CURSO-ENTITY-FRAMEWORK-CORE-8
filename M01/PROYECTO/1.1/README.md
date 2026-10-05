# AceriaData — Punto 1.1: Introducción al ORM y preparación del proyecto

Este directorio representa el estado del proyecto al finalizar el punto **1.1** de la práctica. El directorio `PROYECTO/1.1` equivale a la carpeta `AceriaData` que el alumno crea en el PDF.

## Estructura resultante

```text
AceriaData/
├── AceriaData.sln
└── AceriaData.Console/
    ├── AceriaData.Console.csproj
    └── Program.cs
```

## Qué incorpora este punto

- Solución `AceriaData.sln`.
- Proyecto de consola .NET 8 `AceriaData.Console`.
- `Microsoft.EntityFrameworkCore.SqlServer` 8.0.31.
- `Microsoft.EntityFrameworkCore.Design` 8.0.31.
- Mensaje base de tres líneas indicado por la práctica.
- Reto resuelto del propio punto conservado como código comentado en `Program.cs` para que el alumno pueda activarlo manualmente.

## Ejecutar este estado

Desde esta carpeta:

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida base debe contener:

```text
=== ACERÍA DEL NORTE ===
Sistema de gestión de órdenes de fabricación
Proyecto AceriaData inicializado
```

## Relación con la práctica

La especificación de este checkpoint es el punto **1.1** de `M01_PRACTICA.pdf`. El código activo reproduce el resultado base de la práctica y las variantes/retos del punto se conservan comentados para experimentación del alumno.

El siguiente estado acumulativo es [1.2](../1.2).
