using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TransaccionesAmbientalesM5UseCase
{
    private readonly ITransaccionesAmbientalesM5Repositorio _repositorio;

    public TransaccionesAmbientalesM5UseCase(ITransaccionesAmbientalesM5Repositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task EjecutarAsync()
    {
        Console.WriteLine("=== 5.5 TRANSACCIONES AMBIENTALES Y BUENAS PRÁCTICAS ===");

        var dosContextos = await _repositorio.DemostrarDosContextosAsync();
        Mostrar(dosContextos);

        if (!dosContextos.Persistido ||
            !dosContextos.TransaccionAmbientalActiva ||
            !dosContextos.FlujoAsyncConservado ||
            dosContextos.PromocionDistribuida)
        {
            throw new InvalidOperationException(
                "La demostración ambiental con dos DbContext no produjo el resultado esperado.");
        }

        var rollback = await _repositorio.DemostrarRollbackSinCompleteAsync();
        Mostrar(rollback);

        if (rollback.Persistido)
        {
            throw new InvalidOperationException(
                "Una TransactionScope sin Complete no debería persistir la operación.");
        }

        var opciones = _repositorio.DemostrarOpcionesDeScope();
        Console.WriteLine("\n--- Required, RequiresNew y Suppress ---");
        Console.WriteLine($"Required reutiliza la transacción: {opciones.RequiredReutilizaTransaccion}");
        Console.WriteLine($"RequiresNew crea otra transacción: {opciones.RequiresNewCreaOtra}");
        Console.WriteLine($"Suppress elimina la transacción ambiental: {opciones.SuppressEliminaAmbiente}");
        Console.WriteLine($"Aislamiento predeterminado observado: {opciones.AislamientoPredeterminado}");
        Console.WriteLine($"Timeout predeterminado del runtime: {opciones.TimeoutPredeterminadoSegundos:F0} s");

        if (!opciones.RequiredReutilizaTransaccion ||
            !opciones.RequiresNewCreaOtra ||
            !opciones.SuppressEliminaAmbiente)
        {
            throw new InvalidOperationException("Las opciones de TransactionScope no se comportaron como se esperaba.");
        }

        var externo = _repositorio.DemostrarRecursoExternoNoTransaccional();
        Console.WriteLine("\n--- Recurso externo no transaccional ---");
        Console.WriteLine($"Fila de base de datos persistida: {externo.FilaBaseDeDatosPersistida}");
        Console.WriteLine($"Efecto externo permanece: {externo.EfectoExternoPermanece}");

        if (externo.FilaBaseDeDatosPersistida || !externo.EfectoExternoPermanece)
        {
            throw new InvalidOperationException(
                "No se demostró correctamente que un recurso externo no enlistado no se revierte.");
        }

        var readCommitted = _repositorio.DemostrarReadCommitted();
        Mostrar(readCommitted);

        var snapshot = _repositorio.DemostrarSnapshot();
        Mostrar(snapshot);

        if (!readCommitted.OperacionPersistida ||
            !snapshot.OperacionPersistida ||
            readCommitted.Observado != "ReadCommitted" ||
            snapshot.Observado != "Snapshot")
        {
            throw new InvalidOperationException("Los niveles de aislamiento no se observaron correctamente.");
        }

        Console.WriteLine(
            "\nConclusión: TransactionScope coordina recursos que participan realmente en la " +
            "transacción. Required reutiliza el ambiente, RequiresNew crea otro y Suppress lo " +
            "suprime. ReadCommitted evita lecturas sucias; Snapshot usa versionado cuando la " +
            "base de datos lo permite. Un servicio externo normal no se revierte por magia.");
    }

    private static void Mostrar(TransaccionAmbientalM5Dto r)
    {
        Console.WriteLine($"\n--- {r.Escenario} ---");
        Console.WriteLine($"Persistido: {r.Persistido}");
        Console.WriteLine($"Transacción ambiental activa: {r.TransaccionAmbientalActiva}");
        Console.WriteLine($"Flujo async conservado: {r.FlujoAsyncConservado}");
        Console.WriteLine($"Promoción distribuida: {r.PromocionDistribuida}");
        Console.WriteLine($"Nivel de aislamiento: {r.NivelAislamiento}");
        Console.WriteLine("SQL observado:");

        foreach (var comando in r.ComandosSql)
        {
            Console.WriteLine("---");
            Console.WriteLine(comando);
        }
    }

    private static void Mostrar(AislamientoM5Dto r)
    {
        Console.WriteLine($"\n--- Aislamiento {r.Solicitado} ---");
        Console.WriteLine($"Observado: {r.Observado}");
        Console.WriteLine($"Operación persistida: {r.OperacionPersistida}");
    }
}
