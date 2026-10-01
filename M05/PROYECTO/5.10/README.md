# AceriaData - Punto 5.10: Logging y diagnóstico

Este checkpoint hereda físicamente **5.9** y añade observabilidad ejecutable sin cambiar el modelo ni la cadena de migraciones.

## Qué se demuestra

- `ILogger` / `ILoggerFactory` y filtros por categorías de EF Core.
- Serilog estructurado con propiedades y scopes.
- Archivo con rotación diaria **y** por tamaño mediante `fileSizeLimitBytes`, `rollOnFileSizeLimit: true` y `retainedFileCountLimit`.
- `DiagnosticListener` completo: suscripción a `AllListeners`, listener de EF Core, filtrado de eventos y liberación de subscriptions.
- EventCounters reales de EF Core 8.
- Application Insights para Console/Worker mediante `AddApplicationInsightsTelemetryWorkerService`.
- Canal local de telemetría en el laboratorio para validar el SDK sin depender de una suscripción Azure ni enviar datos externos.

La fuente original muestra `AddApplicationInsightsTelemetry()`, que es la integración propia del host web ASP.NET Core. En esta aplicación de consola se usa la integración WorkerService. En sistemas actuales también puede exportarse telemetría a Azure Monitor mediante OpenTelemetry; Application Insights continúa siendo un destino/backend de observabilidad.

No se habilita `EnableSensitiveDataLogging()` por defecto porque puede incluir valores sensibles en logs. Debe reservarse para diagnóstico controlado en desarrollo.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución valida automáticamente que existen eventos DiagnosticSource, EventCounters, telemetría Application Insights y al menos dos archivos de log por rotación de tamaño, respetando una retención máxima de tres.

Marcador final:

```text
5.10 OK
```

## Observación externa con dotnet-counters

Para observar desde otro proceso:

```powershell
$env:ACERIA_COUNTER_WAIT_MS=15000
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

Mientras el proceso espera, en otra terminal:

```powershell
dotnet-counters monitor --process-id <PID> Microsoft.EntityFrameworkCore
```

El programa imprime `5.10 COUNTER PID` para facilitar la conexión. Los contadores son los de EF Core 8; no se introducen como EF8 las métricas añadidas en versiones posteriores.
