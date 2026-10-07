using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class AgrupacionesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public AgrupacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== AGRUPACIONES CON PROYECCIÓN ===");
        var porCliente = _unidad.Ordenes.ObtenerResumenPorClienteConOrdenes();
        var porClienteEstado = _unidad.Ordenes.ObtenerResumenPorClienteYEstado();
        var having = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();
        var mensual = _unidad.Ordenes.ObtenerResumenMensualConOrdenes();
        var norte = porCliente.Single(x => x.Cliente == "Constructora del Norte");
        if (norte.TotalOrdenes != 3 || norte.Ordenes.Count != 3 || porClienteEstado.Count != 4 || having.Count != 1 || mensual.Count != 5) throw new InvalidOperationException("Agrupaciones inesperadas.");
        Console.WriteLine($"Norte: {norte.TotalOrdenes} órdenes | HAVING: {having.Count} grupo");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlAgrupacionClienteEstado());

        /*
        // RETO M03 3.6 - HAVING PARA GRUPOS CON MAS DE UNA ORDEN
        // Demuestra el Paso 10 con el único grupo cliente/estado repetido del dataset.
        var gruposReto = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();

        if (gruposReto.Count != 1 ||
            gruposReto[0].Cliente != "Constructora del Norte" ||
            gruposReto[0].Estado != "Pendiente" ||
            gruposReto[0].TotalOrdenes != 2)
            throw new InvalidOperationException("Reto 3.6: el grupo HAVING no coincide con el dataset.");

        var sqlHavingReto = _unidad.Ordenes.ObtenerSqlAgrupacionClienteEstadoConHavingReto();

        Console.WriteLine($"Reto 3.6 OK | {gruposReto[0].Cliente} | {gruposReto[0].Estado} | Órdenes: {gruposReto[0].TotalOrdenes}");
        Console.WriteLine("RETO_HAVING_SQL_INICIO");
        Console.WriteLine(sqlHavingReto);
        Console.WriteLine("RETO_HAVING_SQL_FIN");
        */
    }
}

// ============================================================================
// FRAGMENTO PDF M03 3.6 - PASO 4
// COPIA PEDAGÓGICA EXACTA DEL BLOQUE PUBLICADO EN M03_PRACTICA.
// El E2E sustituye temporalmente el archivo activo por esta copia y la compila.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class AgrupacionesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public AgrupacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== AGRUPACIONES CON PROYECCIÓN ===");
//         var porCliente = _unidad.Ordenes.ObtenerResumenPorClienteConOrdenes();
//         var porClienteEstado = _unidad.Ordenes.ObtenerResumenPorClienteYEstado();
//         var having = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();
//         var mensual = _unidad.Ordenes.ObtenerResumenMensualConOrdenes();
//         var norte = porCliente.Single(x => x.Cliente == "Constructora del Norte");
//         if (norte.TotalOrdenes != 3 || norte.Ordenes.Count != 3 || porClienteEstado.Count != 4 || having.Count != 1 || mensual.Count != 5) throw new InvalidOperationException("Agrupaciones inesperadas.");
//         Console.WriteLine($"Norte: {norte.TotalOrdenes} órdenes | HAVING: {having.Count} grupo");
//         Console.WriteLine(_unidad.Ordenes.ObtenerSqlAgrupacionClienteEstado());
//     }
// }
// ============================================================================
