# AceriaData - Punto 5.12: Buenas prácticas y anti-patrones

Este checkpoint hereda físicamente **5.11** y cierra el código del Módulo 5. No cambia el modelo ni añade migraciones: la última migración oficial sigue siendo `M5_5_2_ConcurrencyTokens`.

## Un anti-patrón debe demostrarse, no solo nombrarse

El laboratorio reproduce un N+1 intencionado y después aplica dos refactorizaciones:

- `Include`, cuando se necesitan las entidades relacionadas;
- una proyección, cuando solo se necesita un resumen.

Las tres variantes calculan el mismo número de órdenes y planchas. El programa compara:

- roundtrips SQL capturados por el interceptor real;
- entidades que quedan en el ChangeTracker;
- SQL generado;
- resultado funcional.

`Include` no se presenta como respuesta universal. Si solo se necesitan unos campos o agregados, una proyección puede materializar menos datos. `AsSplitQuery` tampoco se aplica automáticamente: puede reducir una explosión cartesiana, pero introduce varios roundtrips y debe elegirse según el grafo y el escenario.

## Over-fetching: before/after

El segundo caso compara:

1. entidad completa con tracking;
2. proyección de solo `NumeroOrden`, `Cliente`, `Estado` y `FechaCreacion` con `AsNoTracking`.

La ejecución comprueba que ambos caminos producen el mismo resumen funcional, pero la proyección:

- materializa menos columnas;
- no selecciona `RowVersion`;
- no deja entidades rastreadas.

Esto no significa que el tracking sea malo: es apropiado cuando las entidades van a modificarse.

## Métodos .NET en Where

En EF Core 8, un método .NET no traducible dentro de un filtro no se convierte silenciosamente en un filtro en memoria. El laboratorio demuestra que la consulta falla antes de emitir SQL.

Si se desea evaluación cliente, el cambio debe ser explícito, por ejemplo con `AsEnumerable()`. Eso puede transferir más filas, por lo que debe ser una decisión consciente.

## Reglas que dependen del contexto

- Data Annotations no son un anti-patrón por sí mismas. Fluent API centraliza la configuración y cubre más escenarios, pero ambas técnicas son válidas.
- Aplicar una función a una columna puede perjudicar la sargabilidad; no significa que cualquier función invalide cualquier índice en cualquier proveedor.
- `DbContext` ya incorpora capacidades de Repository y Unit of Work. AceriaData mantiene abstracciones adicionales como decisión de arquitectura.
- InMemory y SQLite sirven para determinados tipos de tests, pero no sustituyen las pruebas SQL Server cuando se valida comportamiento específico del proveedor.
- Una migración ya desplegada no debe reescribirse de manera unilateral; se usa una migración correctiva o un rollback coordinado.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

La salida incluye la comparación before/after y termina con:

```text
5.12 OK
```
