using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("AceriaDB")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB_M5_11_Api;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddAceriaInfrastructure(connectionString);

var app = builder.Build();

app.MapGet("/api/ordenes/count", async (AceriaDbContext context) =>
    Results.Ok(new { total = await context.OrdenesFabricacion.CountAsync() }));

app.MapPost("/api/ordenes", async (AceriaDbContext context, CrearOrdenRequest request) =>
{
    var orden = new AceriaData.Domain.Entities.OrdenFabricacion
    {
        NumeroOrden = request.NumeroOrden,
        Cliente = request.Cliente,
        Estado = "Pendiente",
        FechaCreacion = DateTime.UtcNow
    };
    context.OrdenesFabricacion.Add(orden);
    await context.SaveChangesAsync();
    return Results.Created($"/api/ordenes/{orden.Id}", new { orden.Id, orden.NumeroOrden });
});

app.Run();

public sealed record CrearOrdenRequest(string NumeroOrden, string Cliente);
public partial class Program { }
