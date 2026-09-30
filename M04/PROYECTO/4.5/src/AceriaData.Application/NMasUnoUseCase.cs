using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class NMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");
        var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();

        if (metrica.ConsultasSql != metrica.Ordenes + 1)
            throw new InvalidOperationException(
                $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");

        Console.WriteLine(
            $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");
        Console.WriteLine(
            "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado en la baseline 3.12.");
    }
}
