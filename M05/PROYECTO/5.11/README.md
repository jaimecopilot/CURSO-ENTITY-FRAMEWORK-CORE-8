# AceriaData - Punto 5.11: Testing con EF Core

Este checkpoint hereda físicamente **5.10**. No cambia el modelo oficial: la cadena real de migraciones sigue terminando en `M5_5_2_ConcurrencyTokens`.

## Estrategia de pruebas

La fuente original introduce xUnit, InMemory y SQLite in-memory. En 5.11 se conservan esos conceptos, pero se corrigen dos límites importantes:

- no se usa `EnsureCreated()`; la integración real usa las migraciones oficiales;
- SQLite no se usa para afirmar que reproduce el `rowversion` autogenerado de SQL Server.

La suite queda separada por intención:

1. **Unit tests — xUnit + Moq, sin EF Core.** Las abstracciones de Application se sustituyen por mocks; no se mockea `DbSet`.
2. **Provider behavior — InMemory y SQLite.** Un modelo mínimo de laboratorio demuestra que InMemory no impone semántica relacional y SQLite sí puede imponer FK. El esquema SQLite se crea explícitamente con SQL solo para esta comparación aislada.
3. **Integration DB — SQL Server LocalDB.** `AceriaDbContext` usa la cadena real de migraciones. Se valida `rowversion` real y concurrencia real.
4. **Aislamiento — Respawn.** Limpia datos entre pruebas y conserva `__EFMigrationsHistory`.
5. **Integration HTTP — WebApplicationFactory.** La API mínima `AceriaData.Api` ejecuta endpoints reales contra la base SQL Server de pruebas.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

También se valida que el modelo oficial no tenga cambios pendientes:

```powershell
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
```

## Por qué no basta con InMemory o SQLite

InMemory es útil para ciertos tests de lógica, pero no es un proveedor relacional y puede aceptar operaciones que SQL Server rechazaría.

SQLite es relacional y sirve para enseñar diferencias de proveedor, pero tiene dialecto, tipos y comportamiento distintos de SQL Server. En particular, no reproduce el `rowversion` autogenerado por SQL Server. Por eso la prueba de concurrencia de este punto se ejecuta en LocalDB.

Marcador de ejecución del proyecto acumulativo:

```text
5.11 OK
```
