using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AceriaData.ConsoleApp;

public sealed class LoggingDiagnosticoM5Runner
{
    private readonly AceriaDbContext _context;
    private readonly ILogger<LoggingDiagnosticoM5Runner> _logger;
    private readonly TelemetryClient _telemetry;

    public LoggingDiagnosticoM5Runner(
        AceriaDbContext context,
        ILogger<LoggingDiagnosticoM5Runner> logger,
        TelemetryClient telemetry)
    {
        _context = context;
        _logger = logger;
        _telemetry = telemetry;
    }

    public async Task EjecutarAsync()
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["Modulo"] = "M5",
            ["Punto"] = "5.10"
        });

        var existentes = await _context.OrdenesFabricacion.AsNoTracking().CountAsync();
        _logger.LogInformation("Consulta diagnostica completada. Ordenes existentes: {TotalOrdenes}", existentes);

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

        _telemetry.TrackEvent(
            "AceriaData.M5.5.10",
            new Dictionary<string, string>
            {
                ["NumeroOrden"] = orden.NumeroOrden,
                ["Operacion"] = "Insert"
            },
            new Dictionary<string, double>
            {
                ["Filas"] = filas
            });
        _telemetry.Flush();
    }
}
