using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AceriaData.Infrastructure.Persistence;

public sealed class DesignTimeAceriaDbContextFactory
    : IDesignTimeDbContextFactory<AceriaDbContext>
{
    public AceriaDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__AceriaDB")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;Connect Timeout=30;";

        var options = new DbContextOptionsBuilder<AceriaDbContext>()
            .UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(AceriaDbContext).Assembly.GetName().Name))
            .Options;

        return new AceriaDbContext(options);
    }
}
