using System.Diagnostics;
using AceriaData.ConsoleApp.Diagnostics;
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AceriaData.ConsoleApp;

public sealed class LoggingDiagnosticoM5Runner
{
    private static readonly ActivitySource ActivitySource =
        new(AzureMonitorOpenTelemetry.ActivitySourceName);

    private readonly AceriaDbContext _context;
    private readonly ILogger<LoggingDiagnosticoM5Runner> _logger;

    public LoggingDiagnosticoM5Runner(
        AceriaDbContext context,
        ILogger<LoggingDiagnosticoM5Runner> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task EjecutarAsync()
    {
        using var activity = ActivitySource.StartActivity(
            "M5.5.10.LoggingDiagnostico",
            ActivityKind.Internal);

        activity?.SetTag("curso.modulo", "M5");
        activity?.SetTag("curso.punto", "5.10");

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["Modulo"] = "M5",
            ["Punto"] = "5.10"
        });

        var existentes = await _context.OrdenesFabricacion
            .AsNoTracking()
            .CountAsync();

        _logger.LogInformation(
            "Consulta diagnostica completada. Ordenes existentes: {TotalOrdenes}",
            existentes);

        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-LOG-510",
            Cliente = "Cliente observabilidad",
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow
        };

        _context.OrdenesFabricacion.Add(orden);
        var filas = await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Orden {NumeroOrden} del cliente {Cliente} persistida. Filas: {Filas}",
            orden.NumeroOrden,
            orden.Cliente,
            filas);

        activity?.SetTag("aceriadata.numero_orden", orden.NumeroOrden);
        activity?.SetTag("aceriadata.operacion", "Insert");
        activity?.SetTag("aceriadata.filas", filas);
    }
}
