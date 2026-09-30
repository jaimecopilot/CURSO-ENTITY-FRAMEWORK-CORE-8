namespace AceriaData.Application.Dtos;

public sealed class TrackingMetricaDto
{
    public int Filas { get; set; }
    public int EntidadesRastreadas { get; set; }
    public string Sql { get; set; } = string.Empty;
}

public sealed class IdentityResolutionMetricaDto
{
    public int Filas { get; set; }
    public int ClavesUnicas { get; set; }
    public int InstanciasUnicas { get; set; }
}

public sealed class NMasUnoMetricaDto
{
    public int Ordenes { get; set; }
    public int ConsultasSql { get; set; }
    public int Planchas { get; set; }
}

public sealed class SolucionNMasUnoMetricaDto
{
    public int Ordenes { get; set; }
    public int ConsultasSql { get; set; }
    public int ElementosRelacionados { get; set; }
}

public sealed class SplitQueryMetricaDto
{
    public int Ordenes { get; set; }
    public int ConsultasSql { get; set; }
    public int Planchas { get; set; }
    public int RelacionesAleacion { get; set; }
}

public sealed class OrdenPaginaDto
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public sealed class PaginaOrdenesDto
{
    public List<OrdenPaginaDto> Elementos { get; set; } = new();
    public string Sql { get; set; } = string.Empty;
}

public sealed class DiagnosticoRendimientoDto
{
    public int Filas { get; set; }
    public int ConsultasSql { get; set; }
    public int EntidadesRastreadas { get; set; }
    public long Ticks { get; set; }
    public string Sql { get; set; } = string.Empty;
}
