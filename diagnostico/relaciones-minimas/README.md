# Experimento controlado: E1 y E2 (EF Core 8)

**Objeto**: comprobar en un solo `DbContext`, sin segundas unidades de trabajo, si una clave externa y una navegación se sincronizan de inmediato y distinguir lecturas de memoria de lecturas SQL.

Tres casos independientes:
1. `e2.E1Id = e1B.Id` y lectura inmediata de `e2.E1.Id`: la navegación puede mantener el valor anterior hasta `DetectChanges()`.
2. `e2.E1 = e1A` y lectura inmediata de `e2.E1Id`: la FK puede mantener el valor anterior hasta `DetectChanges()`.
3. `e2.E1.Estado = "Nuevo"`: la misma instancia devuelve inmediatamente Nuevo, pero una proyección SQL ve Anterior hasta `SaveChanges()`.

La base de datos es SQLite **en memoria**, solo para aislar las reglas de EF Core. No es un test completo del proyecto del ZIP ni sustituye SQL Server LocalDB. No hay modificaciones en `main`.

Para ejecutar: `dotnet run --project diagnostico/relaciones-minimas/Mini.csproj`.
