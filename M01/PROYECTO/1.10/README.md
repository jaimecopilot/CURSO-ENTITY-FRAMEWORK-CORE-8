# AceriaData — Punto 1.10: Configuración, conexión y logging

Este directorio contiene **una solución completa y autónoma de Visual Studio al finalizar el punto 1.10**. Parte del estado [1.9](../1.9).

## Abrir en Visual Studio

Abrir directamente:

```text
AceriaData.sln
```

La solución referencia únicamente el proyecto local `AceriaData.Console.csproj`; no depende de ninguna solución situada fuera de esta carpeta.

## Qué incorpora este punto

- `appsettings.json` y `ConfigurationBuilder`.
- Logging de EF Core y opciones de SQL Server.

Todo lo introducido anteriormente permanece en el proyecto. El siguiente estado acumulativo es [1.11](../1.11).

## Relación con la práctica

La explicación paso a paso está en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.10**.

## Ejecutar este estado

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project AceriaData.Console.csproj --configuration Release
```

## Validación

La CI restaura y compila **esta solución local** y ejecuta su proyecto. La auditoría comprueba además continuidad acumulativa y trazabilidad con la práctica.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
