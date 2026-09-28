namespace AceriaData.Application.Dtos;

public sealed class OrdenResumenDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}

public sealed class OrdenConTotalesDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public int TotalPlanchas { get; set; }
    public decimal PesoTotal { get; set; }
}

public sealed class PlanchaDto
{
    public double Espesor { get; set; }
    public decimal Peso { get; set; }
}

public sealed class DetalleDto
{
    public string ComposicionQuimica { get; set; } = string.Empty;
    public double TemperaturaColada { get; set; }
}

public sealed class OrdenConPlanchasDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public List<PlanchaDto> Planchas { get; set; } = new();
}

public sealed class OrdenConDetalleDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DetalleDto? Detalle { get; set; }
}

public sealed class OrdenCompletaDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public List<PlanchaDto> Planchas { get; set; } = new();
    public DetalleDto? Detalle { get; set; }
}

public sealed class ResumenPorClienteDto
{
    public string Cliente { get; set; } = string.Empty;
    public int TotalOrdenes { get; set; }
    public DateTime FechaMasReciente { get; set; }
}

public sealed class ResumenPorEstadoDto
{
    public string Estado { get; set; } = string.Empty;
    public int TotalOrdenes { get; set; }
}

public sealed class ResumenMensualDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public int TotalOrdenes { get; set; }
}

public sealed class ResumenPorClienteConOrdenesDto
{
    public string Cliente { get; set; } = string.Empty;
    public int TotalOrdenes { get; set; }
    public List<OrdenResumenDto> Ordenes { get; set; } = new();
}

public sealed class ResumenPorClienteYEstadoDto
{
    public string Cliente { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TotalOrdenes { get; set; }
    public decimal PesoTotal { get; set; }
}

public sealed class ResumenMensualConOrdenesDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public int TotalOrdenes { get; set; }
    public List<string> NumerosOrden { get; set; } = new();
}

public sealed class OrdenJoinDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public double? Espesor { get; set; }
    public decimal? Peso { get; set; }
    public string? ComposicionQuimica { get; set; }
}

public sealed class OrdenConAleacionesDto
{
    public string NumeroOrden { get; set; } = string.Empty;
    public List<string> Aleaciones { get; set; } = new();
}
