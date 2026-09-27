# AceriaData — Punto 1.9: SaveChanges y unidad de trabajo

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.9**. Parte del estado [1.8](../1.8).

## Qué incorpora este punto

- Uso síncrono y asíncrono de `SaveChanges`.
- Tratamiento reproducible de `DbUpdateException`.
- Operaciones agrupadas como unidad de trabajo.

Todo lo introducido anteriormente permanece en el proyecto. El siguiente estado acumulativo es [1.10](../1.10).

## Relación con la práctica

La explicación paso a paso, código, resultados esperados, errores frecuentes y retos están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.9**.

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**. La auditoría comprueba además la trazabilidad práctica ↔ código, la continuidad acumulativa y que no se introduzcan contenidos antes de su punto correspondiente.



[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
