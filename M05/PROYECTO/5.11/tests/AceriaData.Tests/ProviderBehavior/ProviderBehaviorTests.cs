using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AceriaData.Tests.ProviderBehavior;

public sealed class ProviderBehaviorTests
{
    [Fact]
    public async Task InMemory_NoImponeClaveForaneaRelacional()
    {
        var options = new DbContextOptionsBuilder<ProbeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        await using var context = new ProbeContext(options);
        context.Children.Add(new ProbeChild { ParentId = 999, Name = "huerfano" });

        var exception = await Record.ExceptionAsync(() => context.SaveChangesAsync());

        Assert.Null(exception);
    }

    [Fact]
    public async Task Sqlite_ImponeClaveForanea_CuandoElEsquemaSeCreaExplicitamente()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        await using (var command = connection.CreateCommand())
        {
            command.CommandText = """
                PRAGMA foreign_keys=ON;
                CREATE TABLE Parents (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Code TEXT NOT NULL UNIQUE
                );
                CREATE TABLE Children (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ParentId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    FOREIGN KEY (ParentId) REFERENCES Parents(Id)
                );
                """;
            await command.ExecuteNonQueryAsync();
        }

        var options = new DbContextOptionsBuilder<ProbeContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new ProbeContext(options);
        context.Children.Add(new ProbeChild { ParentId = 999, Name = "huerfano" });

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public void Sqlite_NoDebeUsarseParaValidarRowVersionDeSqlServer()
    {
        var options = new DbContextOptionsBuilder<ProbeContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var context = new ProbeContext(options);
        var property = context.Model.FindEntityType(typeof(ProbeParent))!.FindProperty(nameof(ProbeParent.Version))!;

        Assert.True(property.IsConcurrencyToken);
        Assert.Equal(ValueGenerated.OnAddOrUpdate, property.ValueGenerated);
        Assert.Equal("BLOB", property.GetColumnType());
        // La metadata expresa la intención de EF, pero SQLite no genera el rowversion de SQL Server.
        // La semántica real se verifica en SqlServerIntegrationTests.
    }

    private sealed class ProbeContext(DbContextOptions<ProbeContext> options) : DbContext(options)
    {
        public DbSet<ProbeParent> Parents => Set<ProbeParent>();
        public DbSet<ProbeChild> Children => Set<ProbeChild>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProbeParent>(b =>
            {
                b.ToTable("Parents");
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.Code).IsUnique();
                b.Property(x => x.Version).IsRowVersion().HasColumnType("BLOB");
            });

            modelBuilder.Entity<ProbeChild>(b =>
            {
                b.ToTable("Children");
                b.HasKey(x => x.Id);
                b.HasOne<ProbeParent>().WithMany().HasForeignKey(x => x.ParentId);
            });
        }
    }

    private sealed class ProbeParent
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public byte[] Version { get; set; } = Array.Empty<byte>();
    }

    private sealed class ProbeChild
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
