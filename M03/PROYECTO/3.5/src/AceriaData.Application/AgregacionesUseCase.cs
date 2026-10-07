using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class AgregacionesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public AgregacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== AGREGACIONES ===");
        var total = _unidad.Ordenes.ContarOrdenes();
        var pendientes = _unidad.Ordenes.ContarOrdenesPorEstado("Pendiente");
        var existe = _unidad.Ordenes.ExisteAlgunaOrden();
        var todasConEstado = _unidad.Ordenes.TodasLasOrdenesTienenEstado();
        var peso = _unidad.Ordenes.ObtenerPesoTotalDePlanchas();
        var promedio = _unidad.Ordenes.ObtenerPesoPromedioDePlanchas();
        var minimo = _unidad.Ordenes.ObtenerPesoMinimoDePlanchas();
        var maximo = _unidad.Ordenes.ObtenerPesoMaximoDePlanchas();
        var porCliente = _unidad.Ordenes.ObtenerResumenPorCliente();
        var porEstado = _unidad.Ordenes.ObtenerResumenPorEstado();
        var mensual = _unidad.Ordenes.ObtenerResumenMensual();
        if (total != 5 || pendientes != 3 || !existe || !todasConEstado || peso != 1426.9m || promedio != 285.38m || minimo != 125.6m || maximo != 450.0m || porCliente.Count != 3 || porEstado.Count != 3 || mensual.Count != 5) throw new InvalidOperationException("Agregaciones inesperadas.");
        Console.WriteLine($"Órdenes: {total} | Pendientes: {pendientes} | Peso: {peso:N1} kg | Promedio: {promedio:N2} kg | Min: {minimo:N1} kg | Max: {maximo:N1} kg");

        /*
        // INSPECCION M03 3.5 - SQL AGREGADOS
        // Activa este bloque junto con los métodos pedagógicos del puerto y repositorio.
        var sqlAgregados = _unidad.Ordenes.ObtenerSqlAgregadosReto();
        var sqlMensual = _unidad.Ordenes.ObtenerSqlResumenMensualReto();

        Console.WriteLine("SQL_AGREGADOS_INICIO");
        Console.WriteLine(sqlAgregados);
        Console.WriteLine("SQL_AGREGADOS_FIN");
        Console.WriteLine("SQL_MENSUAL_INICIO");
        Console.WriteLine(sqlMensual);
        Console.WriteLine("SQL_MENSUAL_FIN");
        */

        /*
        // ERROR CONTROLADO M03 3.5 - AGREGACION VACIA SIN ESTRATEGIA
        // Contrasta el promedio no anulable sobre un conjunto vacío con la estrategia
        // nullable + coalescencia usada por el checkpoint.
        var promedioSeguroVacio = _unidad.Ordenes.ObtenerPesoPromedioVacioSeguroReto();
        Exception? errorAgregacionVacia = null;

        try
        {
            _ = _unidad.Ordenes.ObtenerPesoPromedioVacioSinEstrategiaReto();
        }
        catch (Exception ex)
        {
            errorAgregacionVacia = ex;
        }

        if (promedioSeguroVacio != 0m)
            throw new InvalidOperationException("Error controlado 3.5: la estrategia nullable debe devolver 0 para el conjunto vacío.");

        if (errorAgregacionVacia is null)
            throw new InvalidOperationException("Error controlado 3.5: la agregación no anulable sobre el conjunto vacío no produjo el fallo esperado.");

        Console.WriteLine($"Error controlado 3.5 OK | Seguro: {promedioSeguroVacio} | Sin estrategia: {errorAgregacionVacia.GetType().Name}");
        */

        /*
        // RETO M03 3.5 - RESUMEN MENSUAL Y SQL DE AGREGADOS
        // Valida el resumen mensual y contrasta las agregaciones del servidor.
        var mensualReto = _unidad.Ordenes.ObtenerResumenMensual();
        var norteReto = _unidad.Ordenes.ObtenerResumenPorCliente()
            .Single(x => x.Cliente == "Constructora del Norte");
        var pendienteReto = _unidad.Ordenes.ObtenerResumenPorEstado()
            .Single(x => x.Estado == "Pendiente");

        if (mensualReto.Count != 5 ||
            mensualReto.Any(x => x.Anio != 2024 || x.Mes < 1 || x.Mes > 5 || x.TotalOrdenes != 1))
            throw new InvalidOperationException("Reto 3.5: el resumen mensual no coincide con el dataset.");

        if (norteReto.TotalOrdenes != 3 || pendienteReto.TotalOrdenes != 3)
            throw new InvalidOperationException("Reto 3.5: los resúmenes por cliente/estado no coinciden con el dataset.");

        var sqlAgregadosReto = _unidad.Ordenes.ObtenerSqlAgregadosReto();
        var sqlMensualReto = _unidad.Ordenes.ObtenerSqlResumenMensualReto();

        Console.WriteLine($"Reto 3.5 OK | Meses: {mensualReto.Count} | Norte: {norteReto.TotalOrdenes} | Pendiente: {pendienteReto.TotalOrdenes}");
        Console.WriteLine("RETO_SQL_AGREGADOS_INICIO");
        Console.WriteLine(sqlAgregadosReto);
        Console.WriteLine("RETO_SQL_AGREGADOS_FIN");
        Console.WriteLine("RETO_SQL_MENSUAL_INICIO");
        Console.WriteLine(sqlMensualReto);
        Console.WriteLine("RETO_SQL_MENSUAL_FIN");
        */
    }
}

// ============================================================================
// EJEMPLO DEL PASO 4
// COPIA COMENTADA DEL BLOQUE DE LA PRÁCTICA PARA QUE PUEDAS PROBARLO.
// Para utilizarla, comenta temporalmente la implementación activa equivalente y descomenta esta copia en una rama o copia de trabajo.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class AgregacionesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public AgregacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== AGREGACIONES ===");
//         var total = _unidad.Ordenes.ContarOrdenes();
//         var pendientes = _unidad.Ordenes.ContarOrdenesPorEstado("Pendiente");
//         var existe = _unidad.Ordenes.ExisteAlgunaOrden();
//         var todasConEstado = _unidad.Ordenes.TodasLasOrdenesTienenEstado();
//         var peso = _unidad.Ordenes.ObtenerPesoTotalDePlanchas();
//         var promedio = _unidad.Ordenes.ObtenerPesoPromedioDePlanchas();
//         var minimo = _unidad.Ordenes.ObtenerPesoMinimoDePlanchas();
//         var maximo = _unidad.Ordenes.ObtenerPesoMaximoDePlanchas();
//         var porCliente = _unidad.Ordenes.ObtenerResumenPorCliente();
//         var porEstado = _unidad.Ordenes.ObtenerResumenPorEstado();
//         var mensual = _unidad.Ordenes.ObtenerResumenMensual();
//         if (total != 5 || pendientes != 3 || !existe || !todasConEstado || peso != 1426.9m || promedio != 285.38m || minimo != 125.6m || maximo != 450.0m || porCliente.Count != 3 || porEstado.Count != 3 || mensual.Count != 5) throw new InvalidOperationException("Agregaciones inesperadas.");
//         Console.WriteLine($"Órdenes: {total} | Pendientes: {pendientes} | Peso: {peso:N1} kg | Promedio: {promedio:N2} kg | Min: {minimo:N1} kg | Max: {maximo:N1} kg");
//     }
// }
// ============================================================================
