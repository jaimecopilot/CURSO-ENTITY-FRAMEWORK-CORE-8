// ========================================================================
// M05 5.1 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.1 - BLOCK 01
// SECTION: Paso 3: Implementar y estudiar `ConcurrenciaOptimistaM5Repositorio.cs`
// SOURCE TARGET: Paso 3: Implementar y estudiar `ConcurrenciaOptimistaM5Repositorio.cs`
// ------------------------------------------------------------------------
// public ConcurrenciaMismaPropiedadDto DemostrarActualizacionPerdidaMismaPropiedad()
// {
//     RestaurarEstadoInicial();
//
//     using var scopeA = _scopeFactory.CreateScope();
//     using var scopeB = _scopeFactory.CreateScope();
//
//     var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
//
//     SqlCommandCounterInterceptor.Instance.Reset();
//
//     var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
//     var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
//
//     var clienteInicial = ordenA.Cliente;
//
//     ordenA.Cliente = "Cliente actualizado por A";
//     contextA.SaveChanges();
//
//     ordenB.Cliente = "Cliente actualizado por B";
//     contextB.SaveChanges();
//
//     using var scopeVerificacion = _scopeFactory.CreateScope();
//     var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var clienteFinal = contextVerificacion.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.NumeroOrden == NumeroOrden)
//         .Select(o => o.Cliente)
//         .Single();
//
//     return new ConcurrenciaMismaPropiedadDto(
//         clienteInicial,
//         ordenA.Cliente,
//         ordenB.Cliente,
//         clienteFinal,
//         clienteFinal == ordenB.Cliente && clienteFinal != ordenA.Cliente,
//         SqlCommandCounterInterceptor.Instance.SnapshotCommands());
// }
// ========================================================================

// CANONICAL PDF M05 5.1 - BLOCK 02
// SECTION: Paso 4: Implementar y estudiar `ConcurrenciaOptimistaM5Repositorio.cs`
// SOURCE TARGET: Paso 4: Implementar y estudiar `ConcurrenciaOptimistaM5Repositorio.cs`
// ------------------------------------------------------------------------
// public ConcurrenciaPropiedadesDistintasDto DemostrarCambiosEnPropiedadesDistintas()
// {
//     RestaurarEstadoInicial();
//     using var scopeA = _scopeFactory.CreateScope();
//     using var scopeB = _scopeFactory.CreateScope();
//     var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
//
//     var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
//     var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
//
//     ordenA.Cliente = "Cliente actualizado por A";
//     contextA.SaveChanges();
//     ordenB.Estado = "EnProceso";
//     contextB.SaveChanges();
//
//     using var scopeVerificacion = _scopeFactory.CreateScope();
//     var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var resultado = contextVerificacion.OrdenesFabricacion
//         .AsNoTracking()
//         .Where(o => o.NumeroOrden == NumeroOrden)
//         .Select(o => new { o.Cliente, o.Estado })
//         .Single();
//
//     return new ConcurrenciaPropiedadesDistintasDto(
//         ClienteInicial, EstadoInicial, resultado.Cliente, resultado.Estado,
//         resultado.Cliente == ordenA.Cliente && resultado.Estado == ordenB.Estado,
//         SqlCommandCounterInterceptor.Instance.SnapshotCommands());
// }
// ========================================================================

