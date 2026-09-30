# AceriaData - Punto 5.4: Transacciones, SaveChanges y savepoints

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.4**.

Parte físicamente de `M05/PROYECTO/5.3`. No cambia el modelo y conserva como última migración `M5_5_2_ConcurrencyTokens`.

## Delta del punto

La práctica demuestra contra SQL Server LocalDB:

1. **Atomicidad de un único `SaveChanges`**: una inserción válida y otra que viola la clave alternativa se intentan en la misma llamada; al fallar, la válida tampoco queda persistida.
2. **Transacción explícita + `Commit`**: dos llamadas a `SaveChanges` quedan agrupadas y ambas persisten al confirmar.
3. **Transacción explícita + `Rollback`**: una primera escritura ya enviada a SQL Server se revierte cuando una operación posterior falla.
4. **Savepoint manual**: se conserva la primera escritura, se revierte la segunda con `RollbackToSavepoint` y después se confirma la transacción.

## Correcciones importantes respecto a ejemplos simplificados

- En una transacción explícita, EF Core puede crear savepoints automáticamente antes de `SaveChanges`.
- Con SQL Server, esos savepoints **no son compatibles con MARS habilitado**. Por ello el `appsettings.json` de este punto usa `MultipleActiveResultSets=false`.
- SQL Server usa `SAVE TRANSACTION nombre` y `ROLLBACK TRANSACTION nombre` para savepoints. No se presenta un supuesto `RELEASE SAVEPOINT` como SQL de SQL Server.
- Un fallo de `Commit` no se describe como una garantía universal de rollback automático; el código debe tratar las excepciones según el estado real de la transacción.
- No se publican tiempos prefijados como “850 ms frente a 120 ms”. El coste depende del entorno, del batching, del proveedor y de la carga.

## Compilar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
```

## Validar que el modelo no cambió

```powershell
dotnet ef migrations has-pending-model-changes \
  --project src/AceriaData.Infrastructure \
  --startup-project src/AceriaData.Console
```

## Ejecutar

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución debe terminar con:

```text
5.4 OK
```
