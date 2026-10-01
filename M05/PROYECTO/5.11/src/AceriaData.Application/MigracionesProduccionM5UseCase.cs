using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class MigracionesProduccionM5UseCase
{
    private readonly IMigracionesProduccionM5Repositorio _repositorio;

    public MigracionesProduccionM5UseCase(IMigracionesProduccionM5Repositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task EjecutarAsync()
    {
        Console.WriteLine("=== 5.6 MIGRACIONES EN PRODUCCIÓN: ESTRATEGIAS Y DESPLIEGUE ===");

        var resultado = await _repositorio.AplicarConIMigratorAsync();

        Console.WriteLine($"Migraciones aplicadas: {resultado.MigracionesAplicadas}");
        Console.WriteLine($"Migraciones pendientes después de IMigrator: {resultado.MigracionesPendientes}");
        Console.WriteLine($"Última migración aplicada: {resultado.UltimaMigracion}");
        Console.WriteLine($"Tabla de historial: {resultado.TablaHistorial}");
        Console.WriteLine($"Tabla de historial heredada existe: {resultado.TablaHistorialHeredadaExiste}");

        if (resultado.MigracionesAplicadas == 0 ||
            resultado.MigracionesPendientes != 0 ||
            resultado.UltimaMigracion != "20260930203405_M5_5_2_ConcurrencyTokens" ||
            !resultado.TablaHistorialHeredadaExiste)
        {
            throw new InvalidOperationException(
                "La validación programática de migraciones de 5.6 no produjo el resultado esperado.");
        }

        Console.WriteLine(
            "\nConclusión: IMigrator permite un control programático explícito, pero el " +
            "despliegue de producción debe elegir deliberadamente su estrategia. Para SQL " +
            "revisable se genera un script; para automatización se genera un migration bundle. " +
            "Una tabla de historial ya usada no se renombra sin migrar también su contenido.");
    }
}
