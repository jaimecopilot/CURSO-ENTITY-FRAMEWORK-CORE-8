using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TransaccionesM5UseCase
{
    private readonly ITransaccionesM5Repositorio _repositorio;

    public TransaccionesM5UseCase(ITransaccionesM5Repositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public void Ejecutar()
    {
        Console.WriteLine("=== 5.4 TRANSACCIONES: SAVECHANGES, COMMIT, ROLLBACK Y SAVEPOINTS ===");

        var atomica = _repositorio.DemostrarAtomicidadSaveChanges();
        Mostrar(atomica);
        if (!atomica.ResultadoEsperado ||
            atomica.PrimeraOrdenExiste ||
            atomica.MarsHabilitado)
        {
            throw new InvalidOperationException(
                "La demostración de atomicidad de SaveChanges no produjo el resultado esperado.");
        }

        var commit = _repositorio.DemostrarCommitExplicito();
        Mostrar(commit);
        if (!commit.ResultadoEsperado ||
            !commit.PrimeraOrdenExiste ||
            !commit.SegundaOrdenExiste)
        {
            throw new InvalidOperationException("El Commit explícito no persistió las dos operaciones.");
        }

        var rollback = _repositorio.DemostrarRollbackExplicito();
        Mostrar(rollback);
        if (!rollback.ResultadoEsperado ||
            rollback.PrimeraOrdenExiste)
        {
            throw new InvalidOperationException("El Rollback explícito no deshizo la primera escritura.");
        }

        var savepoint = _repositorio.DemostrarRollbackASavepoint();
        Mostrar(savepoint);
        if (!savepoint.ResultadoEsperado ||
            !savepoint.PrimeraOrdenExiste ||
            savepoint.SegundaOrdenExiste)
        {
            throw new InvalidOperationException(
                "RollbackToSavepoint no conservó solo el trabajo anterior al savepoint.");
        }

        Console.WriteLine(
            "\nConclusión: un único SaveChanges es atómico; una transacción explícita " +
            "permite agrupar varias llamadas a SaveChanges; Rollback revierte toda la " +
            "transacción; RollbackToSavepoint revierte solo el trabajo posterior al punto. " +
            "En SQL Server los savepoints de EF Core requieren MARS desactivado.");
    }

    private static void Mostrar(ResultadoTransaccionM5Dto r)
    {
        Console.WriteLine($"\n--- {r.Escenario} ---");
        Console.WriteLine($"Resultado esperado: {r.ResultadoEsperado}");
        Console.WriteLine($"Primera orden existe: {r.PrimeraOrdenExiste}");
        Console.WriteLine($"Segunda orden existe: {r.SegundaOrdenExiste}");
        Console.WriteLine($"MARS habilitado: {r.MarsHabilitado}");
        Console.WriteLine("SQL observado:");

        foreach (var comando in r.ComandosSql)
        {
            Console.WriteLine("---");
            Console.WriteLine(comando);
        }
    }
}
