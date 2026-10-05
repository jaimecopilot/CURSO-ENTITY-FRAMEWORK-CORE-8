# AceriaData — Punto 1.11: Proveedores de datos

Este checkpoint parte de [1.10](../1.10) y reproduce el punto **1.11** de la práctica. AceriaData continúa utilizando **exclusivamente SQL Server LocalDB**; SQLite, PostgreSQL e InMemory se estudian conceptualmente pero no se instalan ni se convierten en proveedores operativos.

## Cambios de este punto

- registro de `AceriaDbContext` mediante `ServiceCollection` y `AddDbContext`;
- resolución del contexto mediante `CreateScope()` y `GetRequiredService<AceriaDbContext>()`;
- `ProviderName`;
- `ToQueryString()` y materialización con `ToListAsync()`;
- observación del mapeo CLR → SQL Server;
- paginación `Skip/Take`;
- consulta de existencia mediante `Any`;
- reintentos, timeout, logging y diagnóstico detallado;
- reto `AuditarProveedor`.

## Variantes del PDF disponibles como código comentado

En `Program.cs` quedan preparadas para descomentar:

- conexión a una instancia LocalDB inexistente para diagnosticar el error;
- creación temporal de `AceriaDB_Laboratorio` manteniendo el mismo proveedor SQL Server.

Después de probarlas, se debe restaurar la configuración oficial `(localdb)\MSSQLLocalDB / AceriaDB`.

## Migraciones

Se conserva la misma historia acumulativa:

`InitialCreate -> AddAleacion -> AddEstadoOrden`.

La fábrica de diseño sigue permitiendo que `dotnet ef` construya el contexto.

## Ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list --configuration Release
dotnet ef database update --configuration Release
dotnet ef migrations script --configuration Release --output migraciones-sqlserver.sql
dotnet run --project AceriaData.Console.csproj --configuration Release
```

El siguiente estado acumulativo es [1.12](../1.12).
