using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AceriaData.Infrastructure.Persistence.Configurations;

public sealed class PlanchaAceroConfiguration : IEntityTypeConfiguration<PlanchaAcero>
{
    public void Configure(EntityTypeBuilder<PlanchaAcero> b)
    {
        b.ToTable("PlanchasAcero", t =>
        {
            t.HasCheckConstraint("CK_PlanchasAcero_Espesor", "[Espesor] > 0");
            t.HasCheckConstraint("CK_PlanchasAcero_Ancho", "[Ancho] > 0");
            t.HasCheckConstraint("CK_PlanchasAcero_Largo", "[Largo] > 0");
            t.HasCheckConstraint("CK_PlanchasAcero_Peso", "[Peso] > 0");
        });
        b.HasKey(x => x.Id);
        b.Property(x => x.Peso).HasPrecision(18, 3);
        b.Property(x => x.Activa).HasDefaultValue(true);
        b.HasOne(x => x.Orden).WithMany(x => x.Planchas).HasForeignKey(x => x.OrdenId).OnDelete(DeleteBehavior.Cascade).IsRequired();
        b.HasIndex(x => new { x.OrdenId, x.Activa }).HasDatabaseName("IX_PlanchasAcero_OrdenId_Activa");
        b.HasIndex(x => x.Espesor).HasFilter("[Activa] = 1").HasDatabaseName("IX_PlanchasAcero_Espesor_Activas");
        b.HasQueryFilter(x => !x.IsDeleted && x.Activa);
    }
}

// ============================================================================
// EJEMPLO DEL PASO 5.1B
// Configuración Fluent API de PlanchaAcero.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 5.1 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// 
// namespace AceriaData.Infrastructure.Persistence.Configurations;
// 
// public sealed class PlanchaAceroConfiguration : IEntityTypeConfiguration<PlanchaAcero>
// {
//     public void Configure(EntityTypeBuilder<PlanchaAcero> b)
//     {
//         b.ToTable("PlanchasAcero", t =>
//         {
//             t.HasCheckConstraint("CK_PlanchasAcero_Espesor", "[Espesor] > 0");
//             t.HasCheckConstraint("CK_PlanchasAcero_Ancho", "[Ancho] > 0");
//             t.HasCheckConstraint("CK_PlanchasAcero_Largo", "[Largo] > 0");
//             t.HasCheckConstraint("CK_PlanchasAcero_Peso", "[Peso] > 0");
//         });
//         b.HasKey(x => x.Id);
//         b.Property(x => x.Peso).HasPrecision(18, 3);
//         b.Property(x => x.Activa).HasDefaultValue(true);
//         b.HasOne(x => x.Orden).WithMany(x => x.Planchas).HasForeignKey(x => x.OrdenId).OnDelete(DeleteBehavior.Cascade).IsRequired();
//         b.HasIndex(x => new { x.OrdenId, x.Activa }).HasDatabaseName("IX_PlanchasAcero_OrdenId_Activa");
//         b.HasIndex(x => x.Espesor).HasFilter("[Activa] = 1").HasDatabaseName("IX_PlanchasAcero_Espesor_Activas");
//         b.HasQueryFilter(x => !x.IsDeleted && x.Activa);
//     }
// }
// ============================================================================
