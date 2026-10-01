# MigrationsHistoryTable: cuándo personalizarla

EF Core registra las migraciones aplicadas en `__EFMigrationsHistory` de forma predeterminada.

La API `MigrationsHistoryTable(nombre, esquema)` permite personalizarla, por ejemplo:

```csharp
options.UseSqlServer(
    connectionString,
    sql => sql.MigrationsHistoryTable("__AceriaMigraciones"));
```

Pero esa decisión no es inocua en una base que ya tiene migraciones aplicadas. Si se cambia el nombre después, EF no mueve automáticamente el historial anterior. El equipo es responsable de migrar esa tabla y de validar que todos los entornos conservan los mismos identificadores de migración.

Por eso AceriaData **mantiene `__EFMigrationsHistory`** en el punto 5.6. El alumno aprende la API y su implicación sin introducir una ruptura artificial en una base acumulativa que ya viene de los módulos anteriores.
