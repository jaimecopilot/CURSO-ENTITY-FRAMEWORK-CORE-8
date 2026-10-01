using System.Data;
using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace AceriaData.Infrastructure.Repositories;

public sealed class MigracionesProduccionM5Repositorio : IMigracionesProduccionM5Repositorio
{
    public const string TablaHistorialHeredada = "__EFMigrationsHistory";

    private readonly IServiceScopeFactory _scopeFactory;

    public MigracionesProduccionM5Repositorio(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<MigracionesProduccionM5Dto> AplicarConIMigratorAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync();

        var aplicadas = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
        var pendientes = (await context.Database.GetPendingMigrationsAsync()).ToArray();
        var tablaExiste = await ExisteTablaHistorialAsync(context);

        return new MigracionesProduccionM5Dto(
            aplicadas.Length,
            pendientes.Length,
            aplicadas.LastOrDefault() ?? "<ninguna>",
            tablaExiste,
            TablaHistorialHeredada);
    }

    private static async Task<bool> ExisteTablaHistorialAsync(AceriaDbContext context)
    {
        var connection = context.Database.GetDbConnection();
        var cerrar = connection.State != ConnectionState.Open;

        if (cerrar)
            await connection.OpenAsync();

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT COUNT(*)
                FROM sys.tables
                WHERE [name] = N'__EFMigrationsHistory';
                """;

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count == 1;
        }
        finally
        {
            if (cerrar)
                await connection.CloseAsync();
        }
    }
}
