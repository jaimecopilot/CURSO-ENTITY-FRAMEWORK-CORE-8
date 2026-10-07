// ========================================================================
// M05 5.8 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.8 - BLOCK 01
// SECTION: Paso 3: Añadir la entidad DetalleOrden en la rama feature
// SOURCE TARGET: Crear el archivo src/AceriaData.Domain/Entities/DetalleOrden.cs:
// ------------------------------------------------------------------------
// namespace AceriaData.Domain.Entities;
//
// public class DetalleOrden
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public string ComposicionQuimica { get; set; } = string.Empty;
//     public double TemperaturaColada { get; set; }
//     public string? Notas { get; set; }
//
//     public virtual OrdenFabricacion Orden { get; set; } = null!;
// }
// ========================================================================

// CANONICAL PDF M05 5.8 - BLOCK 02
// SECTION: Paso 4: Añadir el DbSet de DetalleOrden al DbContext
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Persistence/AceriaDbContext.cs:
// ------------------------------------------------------------------------
// public DbSet<DetalleOrden> DetallesOrden { get; set; } = null!;
// ========================================================================

// CANONICAL PDF M05 5.8 - BLOCK 03
// SECTION: Paso 9: Añadir un índice en la rama bugfix
// SOURCE TARGET: Modificar src/AceriaData.Infrastructure/Persistence/Configurations/OrdenFabricacionConfiguration.cs para añadir un índice:
// ------------------------------------------------------------------------
// public void Configure(EntityTypeBuilder<OrdenFabricacion> builder)
// {
//     // ... configuración existente ...
//
//     builder.HasIndex(o => o.NumeroOrden)
//         .IsUnique()
//         .HasDatabaseName("IX_OrdenesFabricacion_NumeroOrden");
// }
// ========================================================================

