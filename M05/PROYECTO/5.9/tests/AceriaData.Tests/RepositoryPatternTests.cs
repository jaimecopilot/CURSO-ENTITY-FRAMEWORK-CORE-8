using AceriaData.Application.Interfaces;
using AceriaData.Application.UseCases;
using AceriaData.Domain.Entities;
using Moq;
using Xunit;

namespace AceriaData.Tests;

public sealed class RepositoryPatternTests
{
    [Fact]
    public void ObtenerPendientes_UsaLaAbstraccionSinBaseDeDatos()
    {
        var repo = new Mock<IOrdenRepositorio>();
        repo.Setup(r => r.ObtenerTodas()).Returns(
        [
            new OrdenFabricacion { NumeroOrden = "OF-1", Estado = "Pendiente" },
            new OrdenFabricacion { NumeroOrden = "OF-2", Estado = "EnProceso" },
            new OrdenFabricacion { NumeroOrden = "OF-3", Estado = "Pendiente" }
        ]);

        var servicio = new OrdenesConsultaM5Service(repo.Object);
        var pendientes = servicio.ObtenerPendientes();

        Assert.Equal(2, pendientes.Count);
        repo.Verify(r => r.ObtenerTodas(), Times.Once);
    }

    [Fact]
    public void Registrar_DelegaEnElRepositorio()
    {
        var repo = new Mock<IOrdenRepositorio>();
        var servicio = new OrdenesConsultaM5Service(repo.Object);
        var orden = new OrdenFabricacion { NumeroOrden = "OF-MOCK", Estado = "Pendiente" };

        servicio.Registrar(orden);

        repo.Verify(r => r.Agregar(orden), Times.Once);
    }

    /* // RETO M05 5.9 - OPERACION ESPECIFICA MOQ
    [Fact]
    public void ObtenerPendientesRecientesPorCliente_InvocaOperacionEspecificaUnaVez()
    {
        var repo = new Mock<IOrdenRepositorio>();
        var desde = new DateTime(2026, 1, 1);
        var esperado = new List<OrdenFabricacion>
        {
            new() { NumeroOrden = "OF-RETO-59", Cliente = "Cliente Reto", Estado = "Pendiente", FechaCreacion = desde.AddDays(1) }
        };

        repo.Setup(r => r.ObtenerPendientesRecientesPorCliente("Cliente Reto", desde))
            .Returns(esperado);

        var servicio = new OrdenesConsultaM5Service(repo.Object);
        var resultado = servicio.ObtenerPendientesRecientesPorCliente("Cliente Reto", desde);

        Assert.Same(esperado, resultado);
        repo.Verify(r => r.ObtenerPendientesRecientesPorCliente("Cliente Reto", desde), Times.Once);
    }
    */
}
