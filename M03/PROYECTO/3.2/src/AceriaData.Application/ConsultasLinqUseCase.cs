using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConsultasLinqUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");
        var enMemoria = _unidad.Ordenes.ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList();
        var consulta = _unidad.Ordenes.Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion);
        Console.WriteLine("Consulta IQueryable construida: aún no se ha materializado.");
        var enSql = consulta.ToList();
        if (enMemoria.Count != 3 || enSql.Count != 3) throw new InvalidOperationException("Comparación IEnumerable/IQueryable inesperada.");
        Console.WriteLine($"Enumerable: {enMemoria.Count} | IQueryable: {enSql.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlFundamentos());
    }
}
