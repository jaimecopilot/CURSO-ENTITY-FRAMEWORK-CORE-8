using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConcurrenciaOptimistaM5UseCase
{
    private readonly IConcurrenciaOptimistaM5Repositorio _repositorio;

    public ConcurrenciaOptimistaM5UseCase(IConcurrenciaOptimistaM5Repositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public void Ejecutar()
    {
        Console.WriteLine("=== 5.1 CONCURRENCIA OPTIMISTA: CONCEPTO Y NECESIDAD ===");

        var perdida = _repositorio.DemostrarActualizacionPerdidaMismaPropiedad();

        Console.WriteLine("\n--- Dos usuarios modifican la MISMA propiedad ---");
        Console.WriteLine($"Cliente inicial: {perdida.ClienteInicial}");
        Console.WriteLine($"Usuario A guarda: {perdida.ClienteUsuarioA}");
        Console.WriteLine($"Usuario B guarda: {perdida.ClienteUsuarioB}");
        Console.WriteLine($"Cliente final en base de datos: {perdida.ClienteFinal}");
        Console.WriteLine($"¿Se perdió el cambio de A?: {perdida.CambioUsuarioAPerdido}");
        MostrarComandos(perdida.ComandosSql);

        if (!perdida.CambioUsuarioAPerdido ||
            perdida.ClienteFinal != perdida.ClienteUsuarioB)
        {
            throw new InvalidOperationException(
                "La demostración de actualización perdida no produjo el resultado esperado.");
        }

        var merge = _repositorio.DemostrarCambiosEnPropiedadesDistintas();

        Console.WriteLine("\n--- Dos usuarios modifican propiedades DISTINTAS ---");
        Console.WriteLine($"Cliente inicial: {merge.ClienteInicial}");
        Console.WriteLine($"Estado inicial: {merge.EstadoInicial}");
        Console.WriteLine($"Cliente final: {merge.ClienteFinal}");
        Console.WriteLine($"Estado final: {merge.EstadoFinal}");
        Console.WriteLine($"¿Se conservaron ambos cambios?: {merge.AmbosCambiosConservados}");
        MostrarComandos(merge.ComandosSql);

        if (!merge.AmbosCambiosConservados)
        {
            throw new InvalidOperationException(
                "EF Core no conservó los cambios independientes como se esperaba.");
        }

        Console.WriteLine(
            "\nConclusión: sin token de concurrencia, EF Core no detecta que otro usuario " +
            "haya cambiado la fila. Si ambos cambian la misma propiedad, el último guardado " +
            "puede sobrescribir al anterior. Si cambian propiedades distintas y ambas entidades " +
            "están siendo rastreadas normalmente, EF Core actualiza solo las propiedades modificadas.");
    }

    private static void MostrarComandos(IReadOnlyList<string> comandos)
    {
        Console.WriteLine("SQL observado:");
        foreach (var comando in comandos)
        {
            Console.WriteLine("---");
            Console.WriteLine(comando);
        }
    }
}
