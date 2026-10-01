using AceriaData.Domain.Entities;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Xunit;

namespace AceriaData.Tests.Integration;

[Collection(SqlServerCollection.Name)]
public sealed class BuenasPracticasAntiPatronesM5Tests
{
    private readonly SqlServerDatabaseFixture _fixture;

    public BuenasPracticasAntiPatronesM5Tests(SqlServerDatabaseFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task RefactorNMasUnoYOverFetching_MantieneResultadoYReduceTrabajoObservable()
    {
        await _fixture.ResetAsync();

        await using (var seed = new AceriaDbContext(_fixture.CreateOptions()))
        {
            var a = NuevaOrden("OF-512-A", "Pendiente");
            a.Planchas.Add(new PlanchaAcero { Espesor = 10, Ancho = 1000, Largo = 2000, Peso = 100m });
            a.Planchas.Add(new PlanchaAcero { Espesor = 12, Ancho = 1000, Largo = 2000, Peso = 120m });

            var b = NuevaOrden("OF-512-B", "Pendiente");
            b.Planchas.Add(new PlanchaAcero { Espesor = 8, Ancho = 1000, Largo = 2000, Peso = 80m });

            seed.AddRange(a, b, NuevaOrden("OF-512-C", "EnProceso"));
            await seed.SaveChangesAsync();
        }

        await using var context = new AceriaDbContext(_fixture.CreateOptions());
        var resultado = new BuenasPracticasAntiPatronesM5Diagnostico(context).Ejecutar();

        Assert.True(resultado.NMasUno.ResultadosEquivalentes);
        Assert.True(resultado.NMasUno.ConsultasNMasUno > resultado.NMasUno.ConsultasInclude);
        Assert.True(resultado.NMasUno.ConsultasNMasUno > resultado.NMasUno.ConsultasProyeccion);
        Assert.Equal(0, resultado.NMasUno.TrackingInclude);
        Assert.Equal(0, resultado.NMasUno.TrackingProyeccion);

        Assert.True(resultado.OverFetching.ResultadosEquivalentes);
        Assert.True(resultado.OverFetching.ColumnasEntidadCompleta > resultado.OverFetching.ColumnasProyeccion);
        Assert.True(resultado.OverFetching.TrackingEntidadCompleta > 0);
        Assert.Equal(0, resultado.OverFetching.TrackingProyeccion);
        Assert.True(resultado.OverFetching.SqlProyeccionExcluyeRowVersion);

        Assert.True(resultado.Traduccion.MetodoNoTraducibleFalla);
        Assert.Equal(0, resultado.Traduccion.ComandosEmitidosAntesDelFallo);
        Assert.True(resultado.Traduccion.EvaluacionClienteExplicitaFunciona);
        Assert.Equal(1, resultado.Traduccion.ComandosEvaluacionCliente);
    }

    private static OrdenFabricacion NuevaOrden(string numero, string estado) => new()
    {
        NumeroOrden = numero,
        Cliente = "Cliente 5.12",
        Estado = estado,
        FechaCreacion = DateTime.UtcNow
    };
}
