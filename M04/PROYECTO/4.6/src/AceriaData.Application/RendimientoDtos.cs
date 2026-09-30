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
