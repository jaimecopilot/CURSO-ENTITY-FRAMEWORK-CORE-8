# AceriaData - Punto 5.5: Transacciones ambientales y buenas prácticas

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.5**.

Parte físicamente de `M05/PROYECTO/5.4`. No cambia el modelo y conserva `M5_5_2_ConcurrencyTokens` como última migración.

## Qué demuestra el punto

- `TransactionScope` con `ReadCommitted` y timeout explícito.
- `TransactionScopeAsyncFlowOption.Enabled` y comprobación del flujo después de un `await`.
- Dos `DbContext` sobre **una misma conexión SQL abierta**, evitando que el laboratorio dependa accidentalmente de MSDTC.
- Ausencia de `Complete()` => rollback al disponer el scope.
- `Required`, `RequiresNew` y `Suppress`.
- Nivel predeterminado y timeout real observados en el runtime, sin asumir valores mágicos.
- `ReadCommitted` y `Snapshot`, habilitando primero `ALLOW_SNAPSHOT_ISOLATION`.
- Un efecto externo simulado que no participa en `System.Transactions`: la fila SQL se revierte, pero el efecto externo permanece.

## Correcciones importantes

- **ReadCommitted no permite lecturas sucias.** Ese comportamiento corresponde a `ReadUncommitted`.
- `Snapshot` usa versionado de filas para lecturas consistentes, pero no significa “cero bloqueos” ni elimina conflictos de escritura.
- EF Core depende del proveedor para `System.Transactions`; `SqlClient` sí lo soporta.
- Abrir varios recursos durables puede promocar una transacción local a distribuida. En .NET moderno, el soporte de transacciones distribuidas es Windows-only.
- Un servicio HTTP, una cola o cualquier recurso externo normal **no se revierte automáticamente** porque falle un `TransactionScope`; para consistencia entre base de datos y mensajería se necesitan patrones específicos, por ejemplo outbox/compensación.
- No se comparan Snapshot y ReadCommitted con tiempos prefijados. El rendimiento depende de carga, contención, índices y entorno.
- No se inicia un `BeginTransaction` interno dentro de un `TransactionScope` para simular un savepoint.

## Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución debe terminar con:

```text
5.5 OK
```
