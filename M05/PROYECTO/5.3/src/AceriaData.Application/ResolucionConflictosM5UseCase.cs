using AceriaData.Application.Dtos;
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ResolucionConflictosM5UseCase
{
    private readonly IResolucionConflictosM5Repositorio _repositorio;

    public ResolucionConflictosM5UseCase(IResolucionConflictosM5Repositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public void Ejecutar()
    {
        Console.WriteLine("=== 5.3 RESOLUCIÓN DE CONFLICTOS DE CONCURRENCIA ===");

        var cliente = _repositorio.ClienteGana();
        Mostrar(cliente);
        if (!cliente.ConflictoDetectado || cliente.ClienteFinal != "Cliente B - cliente gana")
            throw new InvalidOperationException("La estrategia cliente gana no produjo el resultado esperado.");

        var bd = _repositorio.BaseDeDatosGana();
        Mostrar(bd);
        if (!bd.ConflictoDetectado || bd.ClienteFinal != "Cliente A - base gana")
            throw new InvalidOperationException("La estrategia base de datos gana no produjo el resultado esperado.");

        var personalizada = _repositorio.ResolucionPersonalizada();
        Mostrar(personalizada);
        if (personalizada.ClienteFinal != "Cliente B - merge" ||
            personalizada.EstadoFinal != "EnProceso A")
        {
            throw new InvalidOperationException("La resolución personalizada no fusionó los valores esperados.");
        }

        var notificacion = _repositorio.NotificarSinSobrescribir();
        Mostrar(notificacion);
        if (!notificacion.ConflictoDetectado ||
            notificacion.Valores.Count == 0 ||
            notificacion.ClienteFinal != "Cliente A - notificación")
        {
            throw new InvalidOperationException("La estrategia de notificación no conservó la base de datos.");
        }

        var reintento = _repositorio.ReintentoAcotado(maxIntentos: 3);
        Mostrar(reintento);
        if (reintento.Intentos != 2 || reintento.ClienteFinal != "Cliente B - reintento")
            throw new InvalidOperationException("El reintento acotado no terminó como se esperaba.");

        var eliminacion = _repositorio.DetectarFilaEliminada();
        Console.WriteLine("\n--- Fila eliminada por otro usuario ---");
        Console.WriteLine($"¿Conflicto detectado?: {eliminacion.ConflictoDetectado}");
        Console.WriteLine($"¿GetDatabaseValues confirmó que ya no existe?: {eliminacion.EntidadYaNoExiste}");
        Console.WriteLine($"¿Entrada desacoplada?: {eliminacion.EntidadDesacoplada}");
        MostrarSql(eliminacion.ComandosSql);

        if (!eliminacion.ConflictoDetectado ||
            !eliminacion.EntidadYaNoExiste ||
            !eliminacion.EntidadDesacoplada)
        {
            throw new InvalidOperationException("No se trató correctamente la eliminación concurrente.");
        }

        Console.WriteLine(
            "\nConclusión: resolver un conflicto exige una política explícita. " +
            "GetDatabaseValues permite comparar original/actual/base de datos; Reload descarta " +
            "el cambio local; actualizar OriginalValues permite un reintento consciente; " +
            "si la fila ya no existe, no se intenta eliminarla de nuevo.");
    }

    private static void Mostrar(ResolucionConflictoM5Dto r)
    {
        Console.WriteLine($"\n--- {r.Estrategia} ---");
        Console.WriteLine($"¿Conflicto detectado?: {r.ConflictoDetectado}");
        Console.WriteLine($"Cliente final: {r.ClienteFinal}");
        Console.WriteLine($"Estado final: {r.EstadoFinal}");
        Console.WriteLine($"Intentos de guardado: {r.Intentos}");

        if (r.Valores.Count > 0)
        {
            Console.WriteLine("Valores del conflicto:");
            foreach (var v in r.Valores)
            {
                Console.WriteLine(
                    $"{v.Propiedad}: original={v.ValorOriginal} | actual={v.ValorActual} | bd={v.ValorBaseDeDatos}");
            }
        }

        MostrarSql(r.ComandosSql);
    }

    private static void MostrarSql(IReadOnlyList<string> comandos)
    {
        Console.WriteLine("SQL observado:");
        foreach (var comando in comandos)
        {
            Console.WriteLine("---");
            Console.WriteLine(comando);
        }
    }

    /*
    // RETO M05 5.3 - MERGE CLIENTE LOCAL ESTADO BD OBSERVACIONES COMBINADAS
    public void EjecutarRetoMergePorPropiedad()
    {
        var r = _repositorio.ResolverMergePorPropiedad();

        if (!r.ConflictoDetectado ||
            r.ClienteFinal != "Cliente local - reto" ||
            r.EstadoFinal != "EnProceso BD" ||
            r.ObservacionesFinal != "Observacion BD | Observacion local" ||
            r.Intentos != 2)
        {
            throw new InvalidOperationException(
                "Reto 5.3: el merge por propiedad no produjo el resultado esperado.");
        }

        Console.WriteLine(
            $"Reto 5.3 OK | Cliente={r.ClienteFinal} | Estado={r.EstadoFinal} | Observaciones={r.ObservacionesFinal} | Intentos={r.Intentos}");
    }
    */

}
