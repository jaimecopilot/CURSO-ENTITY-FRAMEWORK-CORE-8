using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CargaLazyUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CargaLazyUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== LAZY LOADING (DEMOSTRACIÓN) ===");
        var ordenes = _unidad.Ordenes.ObtenerTodasSinInclude();
        var totalPlanchas = 0;
        foreach (var orden in ordenes)
            totalPlanchas += orden.Planchas.Count;
        if (ordenes.Count != 5 || totalPlanchas != 5) throw new InvalidOperationException("Lazy Loading no cargó las relaciones.");
        Console.WriteLine($"Órdenes: {ordenes.Count} | Planchas accedidas bajo demanda: {totalPlanchas}");
        Console.WriteLine("Advertencia docente: el acceso dentro de un bucle puede producir N+1 consultas.");

        /*
        // ERROR CONTROLADO M03 3.9 - LAZY LOADING CON DBCONTEXT CERRADO
        // Demuestra el Paso 9: el proxy necesita que el DbContext siga vivo.
        var fueraDeContexto = _unidad.Ordenes.ObtenerTodasSinIncludeLimpiasReto();
        _unidad.Dispose();

        // Esta línea debe fallar: la navegación aún no se ha cargado y el contexto ya está cerrado.
        var planchasFueraDeContexto = fueraDeContexto[0].Planchas.Count;
        Console.WriteLine($"No debería alcanzarse: {planchasFueraDeContexto}");
        */

        /*
        // RETO M03 3.9 - CONTAR ACCESOS DE NAVEGACION Y RIESGO N+1
        // Parte de proxies frescos sin AutoInclude y accede una vez a Planchas por cada orden.
        var ordenesReto = _unidad.Ordenes.ObtenerTodasSinIncludeLimpiasReto();
        var accesosNavegacion = 0;
        var totalReto = 0;

        foreach (var orden in ordenesReto)
        {
            accesosNavegacion++;
            totalReto += orden.Planchas.Count;
        }

        if (ordenesReto.Count != 5 || accesosNavegacion != 5 || totalReto != 5)
            throw new InvalidOperationException("Reto 3.9: accesos Lazy o cardinalidad inesperados.");

        Console.WriteLine($"Reto 3.9 OK | Órdenes: {ordenesReto.Count} | Accesos navegación: {accesosNavegacion} | Planchas: {totalReto} | Consultas potenciales: 1 + {accesosNavegacion}");
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
// public sealed class CargaLazyUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public CargaLazyUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== LAZY LOADING (DEMOSTRACIÓN) ===");
//         var ordenes = _unidad.Ordenes.ObtenerTodasSinInclude();
//         var totalPlanchas = 0;
//         foreach (var orden in ordenes)
//             totalPlanchas += orden.Planchas.Count;
//         if (ordenes.Count != 5 || totalPlanchas != 5) throw new InvalidOperationException("Lazy Loading no cargó las relaciones.");
//         Console.WriteLine($"Órdenes: {ordenes.Count} | Planchas accedidas bajo demanda: {totalPlanchas}");
//         Console.WriteLine("Advertencia docente: el acceso dentro de un bucle puede producir N+1 consultas.");
//     }
// }
// ============================================================================
