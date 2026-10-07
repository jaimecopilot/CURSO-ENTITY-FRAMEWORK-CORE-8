// ========================================================================
// M05 5.4 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.4 - BLOCK 01
// SECTION: Paso 3: Implementar y estudiar `TransaccionesM5Repositorio.cs`
// SOURCE TARGET: Paso 3: Implementar y estudiar `TransaccionesM5Repositorio.cs`
// ------------------------------------------------------------------------
// public ResultadoTransaccionM5Dto DemostrarAtomicidadSaveChanges()
// {
//     const string numeroValido = "OF-M5-54-ATOMIC";
//     Limpiar(numeroValido);
//     using var scope = _scopeFactory.CreateScope();
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//
//     context.OrdenesFabricacion.Add(CrearOrden(numeroValido));
//     context.OrdenesFabricacion.Add(CrearOrden(NumeroExistente));
//
//     var falloEsperado = false;
//     try
//     {
//         context.SaveChanges();
//     }
//     catch (DbUpdateException)
//     {
//         falloEsperado = true;
//         context.ChangeTracker.Clear();
//     }
//
//     var existeValida = Existe(numeroValido);
//     return new ResultadoTransaccionM5Dto(
//         "Atomicidad de un único SaveChanges",
//         falloEsperado && !existeValida,
//         existeValida,
//         Existe(NumeroExistente),
//         MarsHabilitado(context),
//         SqlCommandCounterInterceptor.Instance.SnapshotCommands());
// }
// ========================================================================

// CANONICAL PDF M05 5.4 - BLOCK 02
// SECTION: Paso 4: Implementar y estudiar `TransaccionesM5Repositorio.cs`
// SOURCE TARGET: Paso 4: Implementar y estudiar `TransaccionesM5Repositorio.cs`
// ------------------------------------------------------------------------
// public ResultadoTransaccionM5Dto DemostrarRollbackASavepoint()
// {
//     const string numero1 = "OF-M5-54-SP-1";
//     const string numero2 = "OF-M5-54-SP-2";
//     const string savepoint = "AntesSegundaOrden";
//     Limpiar(numero1, numero2);
//
//     using var scope = _scopeFactory.CreateScope();
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var mars = MarsHabilitado(context);
//     if (mars)
//         throw new InvalidOperationException("5.4 requiere MultipleActiveResultSets=false.");
//
//     using (var transaction = context.Database.BeginTransaction())
//     {
//         context.OrdenesFabricacion.Add(CrearOrden(numero1));
//         context.SaveChanges();
//         transaction.CreateSavepoint(savepoint);
//
//         context.OrdenesFabricacion.Add(CrearOrden(numero2));
//         context.SaveChanges();
//         transaction.RollbackToSavepoint(savepoint);
//
//         context.ChangeTracker.Clear();
//         transaction.Commit();
//     }
//
//     return new ResultadoTransaccionM5Dto(
//         "Rollback a savepoint",
//         Existe(numero1) && !Existe(numero2),
//         Existe(numero1), Existe(numero2), mars,
//         SqlCommandCounterInterceptor.Instance.SnapshotCommands());
// }
// ========================================================================

