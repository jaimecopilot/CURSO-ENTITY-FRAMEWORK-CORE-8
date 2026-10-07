using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AceriaData.Infrastructure.Persistence;

public sealed class AceriaDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AceriaDbContext>
{
    public AceriaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AceriaDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
        return new AceriaDbContext(options);
    }
}

// ============================================================================
// FRAGMENTO PDF M02 2.12 - PASO 8
// Fábrica de tiempo de diseño desde la nueva ubicación arquitectónica.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa y
// elimina el prefijo "// " de esta copia para reconstruir el paso 8 del PDF.
// ----------------------------------------------------------------------------
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// 
// namespace AceriaData.Infrastructure.Persistence;
// 
// public sealed class AceriaDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AceriaDbContext>
// {
//     public AceriaDbContext CreateDbContext(string[] args)
//     {
//         var options = new DbContextOptionsBuilder<AceriaDbContext>()
//             .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB;Trusted_Connection=True;TrustServerCertificate=True;")
//             .Options;
//         return new AceriaDbContext(options);
//     }
// }
// ============================================================================
