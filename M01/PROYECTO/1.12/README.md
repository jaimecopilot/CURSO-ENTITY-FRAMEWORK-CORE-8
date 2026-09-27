# AceriaData — Punto 1.12: Inyección de dependencias y AddDbContext

Este directorio contiene **el proyecto completo y ejecutable al finalizar el punto 1.12**. Parte del estado [1.11](../1.11).

## Qué incorpora este punto

- `AddDbContext`.
- `IOrdenRepositorio` / `OrdenRepositorio`.
- `IServicioOrdenes` / `ServicioOrdenes`.
- Factoría de diseño y estado final de M1.

Todo lo introducido anteriormente permanece en el proyecto. Este es **el estado final del Módulo 1** y la base desde la que debe comenzar M2.

## Relación con la práctica

La explicación paso a paso, código, resultados esperados, errores frecuentes y retos están en [M01_PRACTICA.md](../../PRACTICA/M01_PRACTICA.md), punto **1.12**.

## Ejecutar este estado

```powershell
dotnet restore
dotnet build --configuration Release
dotnet run --configuration Release
```

Para validar también las migraciones del estado final:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.31
dotnet ef migrations list
dotnet ef database update
```

## Validación

Este estado forma parte de la CI de M1 y ha sido validado mediante **restore + build + run**. La auditoría comprueba además la trazabilidad práctica ↔ código, la continuidad acumulativa y que no se introduzcan contenidos antes de su punto correspondiente.

El estado 1.12 pasa además la validación final de migraciones, SQL Server LocalDB y ejecución E2E del módulo.

[Volver al índice del proyecto](../README.md) · [Ver trazabilidad](../../TRAZABILIDAD_M01.md)
