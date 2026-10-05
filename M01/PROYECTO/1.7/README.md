# AceriaData — Punto 1.7: Change Tracker

Este checkpoint parte de [1.6](../1.6) y reproduce las demostraciones del punto **1.7** de la práctica.

## Qué se puede ejecutar

El flujo activo realiza las demostraciones principales del punto:

- estados `Detached → Added → Unchanged → Deleted`;
- detección explícita mediante `DetectChanges`;
- `CurrentValues` y `OriginalValues`;
- enumeración de entidades rastreadas;
- `Update`;
- `Attach`;
- `AsNoTracking`;
- `ChangeTracker.Clear`;
- reto final `ReportarCambios`.

Dentro de `DemostrarAttach` se conserva además, como código comentado, la variante de diagnóstico del PDF donde la propiedad se modifica **antes** de `Attach`, demostrando que ese cambio previo no queda marcado como `Modified`.

El modelo no cambia respecto a 1.6 y se conserva el historial de migraciones existente.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El siguiente checkpoint acumulativo es [1.8](../1.8).
