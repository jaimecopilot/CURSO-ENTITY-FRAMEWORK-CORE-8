# AceriaData — Punto 1.6: DbSet y operaciones básicas

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.6**. Parte del estado [1.5](../1.5).

## Qué incorpora este punto

- CRUD sobre `OrdenFabricacion`.
- Búsquedas con `Find`, `Any` y `Count`.
- Operaciones de inserción y consulta relacionadas con `PlanchaAcero`.

Todo lo introducido anteriormente permanece en el proyecto; este punto añade únicamente la evolución correspondiente a 1.6.

## Relación con la práctica

La explicación paso a paso, código, resultados esperados, errores frecuentes y reto resuelto están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.6**.

El siguiente estado acumulativo es [1.7](../1.7).

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**, además de la auditoría práctica ↔ código y de continuidad acumulativa.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
