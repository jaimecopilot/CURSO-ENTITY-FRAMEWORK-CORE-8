# AceriaData — Punto 1.5: Ciclo de vida del DbContext

Este checkpoint parte de [1.4](../1.4) y reproduce el punto **1.5** de la práctica: contextos de vida corta, fábrica manual, Change Tracker y comparación con un contexto compartido.

## Qué cambia en 1.5

- Se incorpora `AceriaDbContextFactory`.
- Cada inserción y cada consulta utilizan su propio `DbContext`.
- Se conserva `EstadoOrden`, su `DbSet` y la migración `AddEstadoOrden` heredada.
- No se añade ninguna migración nueva porque el modelo no cambia.
- El reto final inserta `OF-003` y dos planchas dentro de una unidad de trabajo.
- Las variantes del PDF para inspeccionar el Change Tracker y utilizar un contexto compartido permanecen comentadas en `Program.cs`.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list --project AceriaData.Console/AceriaData.Console.csproj
dotnet ef migrations has-pending-model-changes --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución base crea `OF-001` y `OF-002`, las consulta con contextos independientes y ejecuta después el reto `OF-003` con dos planchas.

## Probar las alternativas del punto

En `Program.cs` están delimitados dos bloques experimentales del propio ejercicio:

- la variante que muestra una entidad rastreada antes y después de `SaveChanges`;
- la variante con un único `DbContext` compartido, que mantiene dos entidades en el Change Tracker.

El alumno puede comentar el flujo activo y descomentar la variante que quiera observar.

El siguiente checkpoint acumulativo es [1.6](../1.6).
