// ========================================================================
// M05 5.3 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.3 - BLOCK 01
// SECTION: Paso 3: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`
// SOURCE TARGET: Paso 3: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`
// ------------------------------------------------------------------------
// public ResolucionConflictoM5Dto ClienteGana()
// {
//     RestaurarOrden();
//     using var scopeA = _scopeFactory.CreateScope();
//     using var scopeB = _scopeFactory.CreateScope();
//     var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var a = Cargar(contextA);
//     var b = Cargar(contextB);
//
//     a.Cliente = "Cliente A - cliente gana";
//     contextA.SaveChanges();
//
//     b.Cliente = "Cliente B - cliente gana";
//     var conflicto = false;
//     var intentos = 1;
//     var valores = new List<ValorConflictoM5Dto>();
//
//     try
//     {
//         contextB.SaveChanges();
//     }
//     catch (DbUpdateConcurrencyException ex)
//     {
//         conflicto = true;
//         var entry = ex.Entries.Single();
//         var db = entry.GetDatabaseValues()
//             ?? throw new InvalidOperationException("La orden desapareció durante ClienteGana.");
//         valores = CrearValores(entry, db);
//         entry.OriginalValues.SetValues(db);
//         intentos++;
//         contextB.SaveChanges();
//     }
//
//     var final = LeerFinal();
//     return Resultado("Cliente gana", conflicto, final, intentos, valores);
// }
// ========================================================================

// CANONICAL PDF M05 5.3 - BLOCK 02
// SECTION: Paso 4: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`
// SOURCE TARGET: Paso 4: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`
// ------------------------------------------------------------------------
// public ResolucionConflictoM5Dto ResolucionPersonalizada()
// {
//     RestaurarOrden();
//     using var scopeA = _scopeFactory.CreateScope();
//     using var scopeB = _scopeFactory.CreateScope();
//     var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var a = Cargar(contextA);
//     var b = Cargar(contextB);
//
//     a.Cliente = "Cliente A - merge";
//     a.Estado = "EnProceso A";
//     contextA.SaveChanges();
//
//     b.Cliente = "Cliente B - merge";
//     b.Estado = "Completada B";
//     var intentos = 1;
//
//     try
//     {
//         contextB.SaveChanges();
//     }
//     catch (DbUpdateConcurrencyException ex)
//     {
//         var entry = ex.Entries.Single();
//         var db = entry.GetDatabaseValues()
//             ?? throw new InvalidOperationException("La orden desapareció durante la fusión.");
//         entry.OriginalValues.SetValues(db);
//         entry.CurrentValues[nameof(OrdenFabricacion.Estado)] = db[nameof(OrdenFabricacion.Estado)];
//         intentos++;
//         contextB.SaveChanges();
//     }
//
//     return Resultado("Resolución personalizada", true, LeerFinal(), intentos, []);
// }
// ========================================================================

// CANONICAL PDF M05 5.3 - BLOCK 03
// SECTION: Paso 5: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`
// SOURCE TARGET: Paso 5: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`
// ------------------------------------------------------------------------
// while (true)
// {
//     intentos++;
//     try
//     {
//         contextB.SaveChanges();
//         break;
//     }
//     catch (DbUpdateConcurrencyException ex) when (intentos < maxIntentos)
//     {
//         conflicto = true;
//         var entry = ex.Entries.Single();
//         var db = entry.GetDatabaseValues()
//             ?? throw new InvalidOperationException("La fila ya no existe.");
//         entry.OriginalValues.SetValues(db);
//     }
// }
// ========================================================================

