# AceriaData — Punto 1.3: Componentes principales y migraciones

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.3**. Parte del estado [1.2](../1.2).

## Qué incorpora este punto

- `PlanchaAcero` y `Aleacion`.
- `DbSet` correspondientes.
- Migraciones iniciales del modelo.

Todo lo introducido en puntos anteriores permanece disponible. Este estado no incorpora todavía contenidos reservados a puntos posteriores.

## Relación con la práctica

La explicación paso a paso, bloques de código, resultados esperados, errores frecuentes y retos están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.3**.

El siguiente estado acumulativo es [1.4](../1.4).

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**. La auditoría adicional comprueba su trazabilidad con la práctica y la continuidad acumulativa del proyecto.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
