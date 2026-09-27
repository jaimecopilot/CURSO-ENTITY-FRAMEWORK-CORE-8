# AceriaData — Punto 1.8: Gestión de entidades

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.8**. Parte del estado [1.7](../1.7).

## Qué incorpora este punto

- Uso acumulativo de `Add`, `AddRange`, `Update`, `Remove`, `Attach` y `Entry`.
- Cambios de estado explícitos sobre entidades.
- Ejecución de operaciones contra LocalDB.

Todo lo introducido anteriormente permanece en el proyecto; este punto añade únicamente la evolución correspondiente a 1.8.

## Relación con la práctica

La explicación paso a paso, código, resultados esperados, errores frecuentes y reto resuelto están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.8**.

El siguiente estado acumulativo es [1.9](../1.9).

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**, además de la auditoría práctica ↔ código y de continuidad acumulativa.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
