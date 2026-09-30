namespace AceriaData.Application.Dtos;

public sealed class TrackingMetricaDto
{
    public int Filas { get; set; }
    public int EntidadesRastreadas { get; set; }
    public string Sql { get; set; } = string.Empty;
}
