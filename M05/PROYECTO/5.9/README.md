# AceriaData - Punto 5.9: Repositorio y Unidad de Trabajo

Checkpoint acumulativo de AceriaData al terminar el punto **5.9**. Parte físicamente de **5.8** y añade únicamente el delta de Repository / Unit of Work.

## Decisión arquitectónica

EF Core ya ofrece en `DbContext` y `DbSet` capacidades equivalentes a Unit of Work y Repository. AceriaData mantiene una abstracción propia porque Application no debe depender de EF Core y porque determinadas operaciones del dominio se expresan mediante interfaces específicas.

Esto es una decisión de diseño de AceriaData, no una regla universal para todos los proyectos EF Core.

## Cambios de 5.9

- `IRepositorio<T>` y `Repositorio<T>` para operaciones comunes.
- `IOrdenRepositorio` pasa a heredar de `IRepositorio<OrdenFabricacion>` sin perder los métodos acumulados del curso.
- `IDetalleOrdenRepositorio` y `DetalleOrdenRepositorio`.
- `IUnidadDeTrabajo` coordina `Ordenes` y `Detalles` con un único `SaveChanges`.
- `IDbContextFactory<AceriaDbContext>` demuestra creación de contextos independientes bajo demanda.
- Test unitario introductorio con xUnit + Moq sin base de datos.
- No se introduce ninguna migración: el modelo persiste sin cambios desde 5.2.

## Evidencia en lugar de cifras inventadas

La fuente original incluye cifras fijas de tiempo y memoria. Este checkpoint no las reproduce. El ejecutable demuestra:

- equivalencia funcional entre consulta directa y consulta mediante Repository;
- SQL real emitido por `ToQueryString()` para la consulta directa;
- persistencia coordinada de Orden + Detalle mediante Unit of Work;
- contextos distintos creados mediante `IDbContextFactory`.

Para atribuir costes de rendimiento o memoria al patrón haría falta un benchmark controlado, calentamiento, múltiples iteraciones, aislamiento de I/O y análisis estadístico.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

Marcadores esperados:

```text
Orden creada y recuperada: True
Detalle creado y recuperado: True
Resultados equivalentes: True
IDbContextFactory crea contextos distintos: True
5.9 OK
```
