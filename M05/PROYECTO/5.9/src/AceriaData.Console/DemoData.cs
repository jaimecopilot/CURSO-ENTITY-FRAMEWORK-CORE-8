using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.ConsoleApp;

public static class DemoData
{
    public static void Seed(AceriaDbContext context)
    {
        if (context.OrdenesFabricacion.IgnoreQueryFilters().Any()) return;

        var a36 = new Aleacion { Nombre = "ASTM A36", Codigo = "A36", PorcentajeCarbono = 0.20, PorcentajeManganeso = 0.80 };
        var s355 = new Aleacion { Nombre = "S355", Codigo = "S355", PorcentajeCarbono = 0.18, PorcentajeManganeso = 1.20 };

        var o1 = Orden("OF-2024-0001", "Constructora del Norte", "Pendiente", new DateTime(2024, 1, 15));
        o1.Planchas.Add(new PlanchaAcero { Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m });
        o1.Planchas.Add(new PlanchaAcero { Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m });
        o1.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.20%; Mn 0.80%", TemperaturaColada = 1540 };
        o1.Certificado = new CertificadoCalidad { NumeroCertificado = "CERT-0001", FechaEmision = new DateTime(2024, 1, 20), OrganismoCertificador = "Aceria QA" };
        o1.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = a36, CantidadUtilizada = 500m, FechaAsignacion = new DateTime(2024, 1, 15) });

        var o2 = Orden("OF-2024-0002", "Constructora del Sur", "Pendiente", new DateTime(2024, 2, 20));
        o2.Planchas.Add(new PlanchaAcero { Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m });
        o2.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.18%; Mn 1.20%", TemperaturaColada = 1535 };
        o2.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = s355, CantidadUtilizada = 250m, FechaAsignacion = new DateTime(2024, 2, 20) });

        var o3 = Orden("OF-2024-0003", "Constructora del Norte", "EnProceso", new DateTime(2024, 3, 10));
        o3.Planchas.Add(new PlanchaAcero { Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m });
        o3.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.19%; Mn 1.10%", TemperaturaColada = 1545 };
        o3.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = s355, CantidadUtilizada = 400m, FechaAsignacion = new DateTime(2024, 3, 10) });

        var o4 = Orden("OF-2024-0004", "Constructora del Norte", "Pendiente", new DateTime(2024, 4, 5));
        o4.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.17%; Mn 0.90%", TemperaturaColada = 1538 };

        var o5 = Orden("OF-2024-0005", "Constructora del Este", "Completada", new DateTime(2024, 5, 12));
        o5.Planchas.Add(new PlanchaAcero { Espesor = 9.0, Ancho = 1100, Largo = 2100, Peso = 200.0m });
        o5.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = a36, CantidadUtilizada = 180m, FechaAsignacion = new DateTime(2024, 5, 12) });

        context.AddRange(a36, s355, o1, o2, o3, o4, o5);
        context.SaveChanges();

        var extras = Enumerable.Range(6, 15)
            .Select(i => Orden(
                $"OF-2024-{i:0000}",
                i % 3 == 0 ? "Constructora del Norte" :
                i % 3 == 1 ? "Constructora del Sur" : "Constructora del Este",
                i % 4 == 0 ? "EnProceso" : "Pendiente",
                new DateTime(2024, 6, 1).AddDays(i)))
            .ToList();

        context.OrdenesFabricacion.AddRange(extras);
        context.SaveChanges();
    }

    private static OrdenFabricacion Orden(string numero, string cliente, string estado, DateTime fecha) => new()
    {
        NumeroOrden = numero, Cliente = cliente, Estado = estado, FechaCreacion = fecha
    };
}
