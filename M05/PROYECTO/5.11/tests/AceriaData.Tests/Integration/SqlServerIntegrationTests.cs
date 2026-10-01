using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AceriaData.Tests.Integration;

[Collection(SqlServerCollection.Name)]
public sealed class SqlServerIntegrationTests
{
    private readonly SqlServerDatabaseFixture _fixture;

    public SqlServerIntegrationTests(SqlServerDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task MigracionesReales_CreanHistorialYEsquemaEsperado()
    {
        await _fixture.ResetAsync();
        await using var context = new AceriaDbContext(_fixture.CreateOptions());

        var migrations = await context.Database.GetAppliedMigrationsAsync();
        Assert.Contains(migrations, m => m.EndsWith("M5_5_2_ConcurrencyTokens"));

        await using var command = context.Database.GetDbConnection().CreateCommand();
        await context.Database.OpenConnectionAsync();
        command.CommandText = """
            SELECT COUNT(*)
            FROM sys.columns c
            JOIN sys.tables t ON t.object_id = c.object_id
            WHERE t.name = N'OrdenesFabricacion' AND c.name = N'RowVersion'
            """;
        var count = Convert.ToInt32(await command.ExecuteScalarAsync());
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task RowVersionSqlServer_DetectaConflictoRealEntreDosContextos()
    {
        await _fixture.ResetAsync();

        int id;
        await using (var seed = new AceriaDbContext(_fixture.CreateOptions()))
        {
            var orden = new OrdenFabricacion
            {
                NumeroOrden = "OF-TEST-ROWVERSION",
                Cliente = "Inicial",
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow
            };
            seed.OrdenesFabricacion.Add(orden);
            await seed.SaveChangesAsync();
            id = orden.Id;
            Assert.NotEmpty(orden.RowVersion);
        }

        await using var contextA = new AceriaDbContext(_fixture.CreateOptions());
        await using var contextB = new AceriaDbContext(_fixture.CreateOptions());

        var a = await contextA.OrdenesFabricacion.SingleAsync(x => x.Id == id);
        var b = await contextB.OrdenesFabricacion.SingleAsync(x => x.Id == id);

        a.Cliente = "A";
        await contextA.SaveChangesAsync();

        b.Cliente = "B";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => contextB.SaveChangesAsync());
    }

    [Fact]
    public async Task Respawn_LimpiaDatosPeroConservaHistorialDeMigraciones()
    {
        await _fixture.ResetAsync();
        await using (var context = new AceriaDbContext(_fixture.CreateOptions()))
        {
            context.OrdenesFabricacion.Add(new OrdenFabricacion
            {
                NumeroOrden = "OF-RESPAWN",
                Cliente = "Temporal",
                Estado = "Pendiente",
                FechaCreacion = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        await _fixture.ResetAsync();

        await using var check = new AceriaDbContext(_fixture.CreateOptions());
        Assert.Equal(0, await check.OrdenesFabricacion.CountAsync());
        Assert.NotEmpty(await check.Database.GetAppliedMigrationsAsync());
    }
}
