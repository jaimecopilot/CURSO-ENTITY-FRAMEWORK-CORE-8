using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TraduccionConsultasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public TraduccionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===");

        if (!_unidad.Ordenes.FiltroPersonalizadoNoTraducibleFallaM4("Pendiente"))
            throw new InvalidOperationException("4.7: EF Core no rechazo el filtro personalizado no traducible.");

        var cliente = _unidad.Ordenes.ContarConEvaluacionClienteExplicitaM4("Pendiente");
        if (cliente <= 0)
            throw new InvalidOperationException("4.7: la evaluacion cliente explicita no devolvio datos.");

        var sqlFuncion = _unidad.Ordenes.ObtenerSqlClienteConFuncionM4("Constructora del Norte");
        var sqlDirecto = _unidad.Ordenes.ObtenerSqlClienteDirectoM4("Constructora del Norte");

        if (!sqlFuncion.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.7: no se observa LOWER en el SQL con funcion.");
        if (sqlDirecto.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.7: la comparacion directa introdujo LOWER inesperadamente.");

        Console.WriteLine("Filtro no traducible: InvalidOperationException observada.");
        Console.WriteLine($"Evaluacion cliente explicita: {cliente} filas coincidentes.");
        Console.WriteLine("--- SQL con funcion sobre columna ---");
        Console.WriteLine(sqlFuncion);
        Console.WriteLine("--- SQL con comparacion directa ---");
        Console.WriteLine(sqlDirecto);
    }
}
