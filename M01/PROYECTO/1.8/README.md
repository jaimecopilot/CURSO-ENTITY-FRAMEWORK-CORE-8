# AceriaData — Punto 1.8: Gestión de entidades

Este checkpoint parte de [1.7](../1.7) y reproduce el punto **1.8** de la práctica.

## Qué incorpora

El flujo activo demuestra:

- `Add` y `AddRange`;
- `Update`;
- `Attach`;
- manipulación manual mediante `Entry.State`;
- `Remove`;
- modificación selectiva mediante `Entry.Property`;
- reto final `ActualizarSoloCliente`.

El modelo no cambia respecto a 1.7 y se conserva el historial real de migraciones.

## Variantes conservadas para el alumno

`Program.cs` deja preparadas como alternativas:

- logging de EF Core mediante `LogTo` para comparar el SQL;
- diagnóstico de una segunda llamada a `Add` sobre la misma instancia;
- `UpdateRange`;
- `RemoveRange`.

### Nota técnica sobre el diagnóstico de Add duplicado

La práctica afirma que llamar dos veces a `Add` sobre la **misma instancia** ya rastreada produce `InvalidOperationException`. El E2E ejecutado con EF Core 8.0.31 ha comprobado que esa segunda llamada no produce esa excepción y la entidad continúa rastreada. El PDF se conserva sin cambios, tal como se ha requerido, y el código comentado permite observar el comportamiento real.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project AceriaData.Console/AceriaData.Console.csproj
dotnet run --project AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El siguiente checkpoint acumulativo es [1.9](../1.9).
