// ========================================================================
// M05 5.2 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.2 - BLOCK 01
// SECTION: Paso 3: Implementar y estudiar `ConcurrencyTokensConfiguration.cs`
// SOURCE TARGET: Paso 3: Implementar y estudiar `ConcurrencyTokensConfiguration.cs`
// ------------------------------------------------------------------------
// public sealed class OrdenFabricacionConcurrencyConfiguration : IEntityTypeConfiguration<OrdenFabricacion>
// {
//     public void Configure(EntityTypeBuilder<OrdenFabricacion> b) =>
//         b.Property(x => x.RowVersion).IsRowVersion();
// }
//
// public sealed class PlanchaAceroConcurrencyConfiguration : IEntityTypeConfiguration<PlanchaAcero>
// {
//     public void Configure(EntityTypeBuilder<PlanchaAcero> b) =>
//         b.Property(x => x.RowVersion).IsRowVersion();
// }
//
// public sealed class AleacionConcurrencyConfiguration : IEntityTypeConfiguration<Aleacion>
// {
//     public void Configure(EntityTypeBuilder<Aleacion> b) =>
//         b.Property(x => x.RowVersion).IsRowVersion();
// }
//
// public sealed class DetalleOrdenConcurrencyConfiguration : IEntityTypeConfiguration<DetalleOrden>
// {
//     public void Configure(EntityTypeBuilder<DetalleOrden> b)
//     {
//         b.Property(x => x.EstadoDetalle)
//             .IsRequired()
//             .HasMaxLength(50)
//             .HasDefaultValue("Pendiente")
//             .IsConcurrencyToken();
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.2 - BLOCK 02
// SECTION: Paso 4: Implementar y estudiar `TokensConcurrenciaM5Repositorio.cs`
// SOURCE TARGET: Paso 4: Implementar y estudiar `TokensConcurrenciaM5Repositorio.cs`
// ------------------------------------------------------------------------
// public RowVersionM5Dto DemostrarRowVersion()
// {
//     using var scopeA = _scopeFactory.CreateScope();
//     using var scopeB = _scopeFactory.CreateScope();
//     var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
//
//     var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
//     var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
//     var inicialA = Convert.ToHexString(ordenA.RowVersion);
//     var inicialB = Convert.ToHexString(ordenB.RowVersion);
//
//     SqlCommandCounterInterceptor.Instance.Reset();
//     ordenA.Cliente = "Cliente 5.2 A";
//     contextA.SaveChanges();
//     var despuesA = Convert.ToHexString(ordenA.RowVersion);
//
//     ordenB.Estado = "EnProceso";
//     var conflicto = false;
//     try
//     {
//         contextB.SaveChanges();
//     }
//     catch (DbUpdateConcurrencyException)
//     {
//         conflicto = true;
//     }
//
//     return new RowVersionM5Dto(
//         inicialA, inicialB, despuesA, conflicto,
//         SqlCommandCounterInterceptor.Instance.SnapshotCommands());
// }
// ========================================================================

// CANONICAL PDF M05 5.2 - BLOCK 03
// SECTION: Paso 5: Implementar y estudiar `TokensConcurrenciaM5Repositorio.cs`
// SOURCE TARGET: Paso 5: Implementar y estudiar `TokensConcurrenciaM5Repositorio.cs`
// ------------------------------------------------------------------------
// public IndiceRowVersionM5Dto ComprobarIndiceRowVersion()
// {
//     using var scope = _scopeFactory.CreateScope();
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     var connection = context.Database.GetDbConnection();
//     if (connection.State != ConnectionState.Open)
//         connection.Open();
//
//     using var command = connection.CreateCommand();
//     command.CommandText = """
//         SELECT COUNT(*)
//         FROM sys.indexes AS i
//         INNER JOIN sys.index_columns AS ic
//             ON i.object_id = ic.object_id AND i.index_id = ic.index_id
//         INNER JOIN sys.columns AS c
//             ON ic.object_id = c.object_id AND ic.column_id = c.column_id
//         WHERE i.object_id = OBJECT_ID(N'dbo.OrdenesFabricacion')
//           AND c.name = N'RowVersion';
//         """;
//
//     return new IndiceRowVersionM5Dto(Convert.ToInt32(command.ExecuteScalar()) > 0);
// }
// ========================================================================

