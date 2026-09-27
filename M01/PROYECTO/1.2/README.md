# AceriaData — Punto 1.2: Arquitectura general de EF Core

Este directorio contiene **una solución completa y autónoma de Visual Studio al finalizar el punto 1.2**. Parte del estado [1.1](../1.1).

## Abrir en Visual Studio

Abrir directamente:

```text
AceriaData.sln
```

La solución referencia únicamente el proyecto local `AceriaData.Console.csproj`; no depende de ninguna solución situada fuera de esta carpeta.

## Qué incorpora este punto

- `OrdenFabricacion`.
- `AceriaDbContext`.
- Primer acceso a SQL Server LocalDB.

Todo lo introducido anteriormente permanece en el proyecto. El siguiente estado acumulativo es [1.3](../1.3).

## Relación con la práctica

La explicación paso a paso está en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.2**.

## Ejecutar este estado

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project AceriaData.Console.csproj --configuration Release
```

## Validación

La CI restaura y compila **esta solución local** y ejecuta su proyecto. La auditoría comprueba además continuidad acumulativa y trazabilidad con la práctica.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
