# AceriaData — Punto 1.6: DbSet y operaciones básicas de acceso a datos

Este checkpoint parte de [1.5](../1.5) y reproduce el punto **1.6** de la práctica.

## Qué incorpora

- inserción con `Add`;
- listado ordenado con `ToList`;
- actualización con `FirstOrDefault` + `SaveChanges`;
- existencia con `Any`;
- conteo con `Count`;
- búsqueda por clave con `Find`;
- eliminación segura con `Remove`;
- reto `InsertarPlancha` asociada a una orden existente;
- variante `AsNoTracking` conservada como código comentado;
- variante de caché de identidad con dos llamadas a `Find` conservada como código comentado;
- diagnóstico de eliminación de una orden inexistente conservado como llamada comentada.

El modelo no cambia respecto a 1.5, por lo que se conserva exactamente el historial de migraciones heredado hasta `AddEstadoOrden`.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El flujo activo ejecuta el CRUD completo y, al final, inserta una plancha en `OF-002`. Para repetir `AsNoTracking` o la caché de identidad, sigue las indicaciones de los bloques comentados de `Program.cs`.

El siguiente checkpoint acumulativo es [1.7](../1.7).
