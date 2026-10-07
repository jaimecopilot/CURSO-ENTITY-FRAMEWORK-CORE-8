// ========================================================================
// M05 5.6 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.6 - BLOCK 01
// SECTION: Paso 3: Implementar y estudiar `MigracionesProduccionM5Repositorio.cs`
// SOURCE TARGET: Paso 3: Implementar y estudiar `MigracionesProduccionM5Repositorio.cs`
// ------------------------------------------------------------------------
// public async Task<MigracionesProduccionM5Dto> AplicarConIMigratorAsync()
// {
//     using var scope = _scopeFactory.CreateScope();
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//
//     var migrator = context.GetService<IMigrator>();
//     await migrator.MigrateAsync();
//
//     var aplicadas = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
//     var pendientes = (await context.Database.GetPendingMigrationsAsync()).ToArray();
//     var tablaExiste = await ExisteTablaHistorialAsync(context);
//
//     return new MigracionesProduccionM5Dto(
//         aplicadas.Length,
//         pendientes.Length,
//         aplicadas.LastOrDefault() ?? "<ninguna>",
//         tablaExiste,
//         "__EFMigrationsHistory");
// }
// ========================================================================

