// ========================================================================
// M05 5.5 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.5 - BLOCK 01
// SECTION: Paso 3: Implementar y estudiar `TransaccionesAmbientalesM5Repositorio.cs`
// SOURCE TARGET: Paso 3: Implementar y estudiar `TransaccionesAmbientalesM5Repositorio.cs`
// ------------------------------------------------------------------------
// public async Task<TransaccionAmbientalM5Dto> DemostrarDosContextosAsync()
// {
//     var options = new TransactionOptions
//     {
//         IsolationLevel = IsolationLevel.ReadCommitted,
//         Timeout = TimeSpan.FromSeconds(30)
//     };
//
//     bool ambienteActivo;
//     bool flujoAsync;
//     bool promocion;
//     string aislamiento;
//     int ordenId;
//
//     using (var scope = new TransactionScope(
//                TransactionScopeOption.Required,
//                options,
//                TransactionScopeAsyncFlowOption.Enabled))
//     {
//         await using var connection = new SqlConnection(ObtenerConnectionString());
//         await connection.OpenAsync();
//         ambienteActivo = Transaction.Current is not null;
//         aislamiento = Transaction.Current?.IsolationLevel.ToString() ?? "<none>";
//
//         await using (var contextOrden = CrearContexto(connection))
//         {
//             var orden = CrearOrden("OF-M5-55-AMBIENT");
//             contextOrden.OrdenesFabricacion.Add(orden);
//             await contextOrden.SaveChangesAsync();
//             ordenId = orden.Id;
//         }
//
//         await Task.Yield();
//         flujoAsync = Transaction.Current is not null;
//
//         await using (var contextPlancha = CrearContexto(connection))
//         {
//             contextPlancha.PlanchasAcero.Add(new PlanchaAcero
//             {
//                 OrdenId = ordenId,
//                 Espesor = 10, Ancho = 1000, Largo = 2000, Peso = 150m
//             });
//             await contextPlancha.SaveChangesAsync();
//         }
//
//         promocion = Transaction.Current?.TransactionInformation.DistributedIdentifier != Guid.Empty;
//         scope.Complete();
//     }
//
//     return new TransaccionAmbientalM5Dto(
//         "Dos DbContext con una conexión compartida",
//         ExisteOrdenYPlancha("OF-M5-55-AMBIENT"),
//         ambienteActivo, flujoAsync, promocion, aislamiento,
//         SqlCommandCounterInterceptor.Instance.SnapshotCommands());
// }
// ========================================================================

// CANONICAL PDF M05 5.5 - BLOCK 02
// SECTION: Paso 4: Implementar y estudiar `TransaccionesAmbientalesM5Repositorio.cs`
// SOURCE TARGET: Paso 4: Implementar y estudiar `TransaccionesAmbientalesM5Repositorio.cs`
// ------------------------------------------------------------------------
// public OpcionesTransactionScopeM5Dto DemostrarOpcionesDeScope()
// {
//     string outerId;
//     bool requiredMisma;
//     bool requiresNewOtra;
//     bool suppressSinAmbiente;
//     string aislamiento;
//
//     using (var outer = new TransactionScope())
//     {
//         var outerTransaction = Transaction.Current
//             ?? throw new InvalidOperationException("No se creó la transacción ambiental.");
//         outerId = outerTransaction.TransactionInformation.LocalIdentifier;
//         aislamiento = outerTransaction.IsolationLevel.ToString();
//
//         using (var required = new TransactionScope(TransactionScopeOption.Required))
//         {
//             requiredMisma = Transaction.Current?.TransactionInformation.LocalIdentifier == outerId;
//             required.Complete();
//         }
//
//         using (var requiresNew = new TransactionScope(TransactionScopeOption.RequiresNew))
//         {
//             requiresNewOtra = Transaction.Current?.TransactionInformation.LocalIdentifier != outerId;
//             requiresNew.Complete();
//         }
//
//         using (var suppressed = new TransactionScope(TransactionScopeOption.Suppress))
//         {
//             suppressSinAmbiente = Transaction.Current is null;
//             suppressed.Complete();
//         }
//         outer.Complete();
//     }
//
//     return new OpcionesTransactionScopeM5Dto(
//         requiredMisma, requiresNewOtra, suppressSinAmbiente,
//         aislamiento, TransactionManager.DefaultTimeout.TotalSeconds);
// }
// ========================================================================

