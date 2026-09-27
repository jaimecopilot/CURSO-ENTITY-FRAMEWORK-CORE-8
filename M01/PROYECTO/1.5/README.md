# AceriaData — Punto 1.5: Ciclo de vida del DbContext

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.5**. Parte del estado [1.4](../1.4).

## Qué incorpora este punto

- Factoría manual para crear contextos con vida corta.
- Patrón de uso y liberación explícita del `DbContext`.
- Operaciones acumulativas sobre el modelo existente.

Todo lo introducido anteriormente permanece en el proyecto; este punto añade únicamente la evolución correspondiente a 1.5.

## Relación con la práctica

La explicación paso a paso, código, resultados esperados, errores frecuentes y reto resuelto están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.5**.

El siguiente estado acumulativo es [1.6](../1.6).

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**, además de la auditoría práctica ↔ código y de continuidad acumulativa.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
