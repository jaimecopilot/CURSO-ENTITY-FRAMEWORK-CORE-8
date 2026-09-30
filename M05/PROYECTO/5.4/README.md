# AceriaData - Punto 5.3: Resolución de conflictos de concurrencia

Estado completo, autónomo y acumulativo de AceriaData al terminar el punto **5.3**.

Parte físicamente de `M05/PROYECTO/5.2`. No cambia el modelo: conserva la migración `M5_5_2_ConcurrencyTokens`.

## Estrategias demostradas

- **Cliente gana:** se obtienen los valores actuales de la base de datos, se actualizan `OriginalValues` y se reintenta conscientemente.
- **Base de datos gana:** `Reload()` descarta el cambio local.
- **Resolución personalizada:** el cliente local prevalece para `Cliente` y la base de datos para `Estado`.
- **Notificación:** se devuelven valores original/actual/base de datos y no se sobrescribe nada.
- **Reintento acotado:** nunca se reintenta indefinidamente.
- **Fila eliminada:** si `GetDatabaseValues()` devuelve `null`, se reconoce que la fila ya no existe y la entrada se desacopla; no se intenta eliminar de nuevo.

No se usan cifras de milisegundos prefijadas para decidir qué estrategia es “más rápida”. Se capturan los comandos SQL reales y se comparan las operaciones adicionales requeridas por cada política.

## Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La ejecución debe terminar con:

```text
5.3 OK
```
