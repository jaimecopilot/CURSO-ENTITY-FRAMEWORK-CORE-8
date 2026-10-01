using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Xunit;

namespace AceriaData.Tests.Integration;

public sealed class SqlServerDatabaseFixture : IAsyncLifetime
{
    private readonly string _databaseName = "AceriaDB_M5_11_Tests_" + Guid.NewGuid().ToString("N")[..8];
    private Respawner? _respawner;

    public string ConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;";

    public DbContextOptions<AceriaDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<AceriaDbContext>()
            .UseSqlServer(ConnectionString, sql => sql.MigrationsAssembly(typeof(AceriaDbContext).Assembly.GetName().Name))
            .AddInterceptors(SqlCommandCounterInterceptor.Instance)
            .Options;

    public async Task InitializeAsync()
    {
        await using var context = new AceriaDbContext(CreateOptions());
        await context.Database.MigrateAsync();

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = [new Respawn.Graph.Table("__EFMigrationsHistory")]
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner is null) throw new InvalidOperationException("Respawn no inicializado.");
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        SqlConnection.ClearAllPools();
        var master = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
        await using var connection = new SqlConnection(master);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = $"""
            IF DB_ID(N'{_databaseName}') IS NOT NULL
            BEGIN
                ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_databaseName}];
            END
            """;
        await command.ExecuteNonQueryAsync();
    }
}

[CollectionDefinition(Name)]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerDatabaseFixture>
{
    public const string Name = "SqlServer M5.11";
}
