# AceriaData — Punto 1.10: Configuración, conexión y logging

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.10**. Parte del estado [1.9](../1.9).

## Qué incorpora este punto

- `appsettings.json` y `ConfigurationBuilder`.
- Configuración de `UseSqlServer`.
- Logging de EF Core y `EnableDetailedErrors`.

Todo lo introducido anteriormente permanece en el proyecto. El siguiente estado acumulativo es [1.11](../1.11).

## Relación con la práctica

La explicación paso a paso, código, resultados esperados, errores frecuentes y retos están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.10**.

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**. La auditoría comprueba además la trazabilidad práctica ↔ código, la continuidad acumulativa y que no se introduzcan contenidos antes de su punto correspondiente.



[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
