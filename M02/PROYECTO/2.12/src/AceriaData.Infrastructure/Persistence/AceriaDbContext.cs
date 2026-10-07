using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Persistence;

public sealed class AceriaDbContext : DbContext
{
    public AceriaDbContext(DbContextOptions<AceriaDbContext> options) : base(options) { }

    public DbSet<OrdenFabricacion> OrdenesFabricacion => Set<OrdenFabricacion>();
    public DbSet<PlanchaAcero> PlanchasAcero => Set<PlanchaAcero>();
    public DbSet<Aleacion> Aleaciones => Set<Aleacion>();
    public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();
    public DbSet<DetalleOrden> DetallesOrden => Set<DetalleOrden>();
    public DbSet<CertificadoCalidad> CertificadosCalidad => Set<CertificadoCalidad>();
    public DbSet<OrdenAleacion> OrdenesAleaciones => Set<OrdenAleacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AceriaDbContext).Assembly);
    }
}

// ============================================================================
// EJEMPLO DEL PASO 5
// DbContext y aplicación de configuraciones desde Infrastructure.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 5 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// using AceriaData.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// 
// namespace AceriaData.Infrastructure.Persistence;
// 
// public sealed class AceriaDbContext : DbContext
// {
//     public AceriaDbContext(DbContextOptions<AceriaDbContext> options) : base(options) { }
// 
//     public DbSet<OrdenFabricacion> OrdenesFabricacion => Set<OrdenFabricacion>();
//     public DbSet<PlanchaAcero> PlanchasAcero => Set<PlanchaAcero>();
//     public DbSet<Aleacion> Aleaciones => Set<Aleacion>();
//     public DbSet<EstadoOrden> EstadosOrden => Set<EstadoOrden>();
//     public DbSet<DetalleOrden> DetallesOrden => Set<DetalleOrden>();
//     public DbSet<CertificadoCalidad> CertificadosCalidad => Set<CertificadoCalidad>();
//     public DbSet<OrdenAleacion> OrdenesAleaciones => Set<OrdenAleacion>();
// 
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         base.OnModelCreating(modelBuilder);
//         modelBuilder.ApplyConfigurationsFromAssembly(typeof(AceriaDbContext).Assembly);
//     }
// }
// ============================================================================
