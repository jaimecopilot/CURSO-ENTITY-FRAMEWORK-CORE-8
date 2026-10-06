using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CargaExplicitaUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CargaExplicitaUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== EXPLICIT LOADING ===");
        var orden = _unidad.Ordenes.ObtenerConCargaExplicita("OF-2024-0001");
        var filtrada = _unidad.Ordenes.ObtenerConPlanchasPesadasExplicitas("OF-2024-0002", 300m);
        if (orden is null || orden.Planchas.Count != 2 || orden.Detalle is null) throw new InvalidOperationException("Carga explícita incompleta.");
        if (filtrada is null || filtrada.Planchas.Count != 0) throw new InvalidOperationException("Query() de carga explícita inesperado.");
        Console.WriteLine($"Carga completa: {orden.Planchas.Count} planchas | Filtrada OF-0002: {filtrada.Planchas.Count}");

        /*
        // ERROR CONTROLADO M03 3.10 - EVITAR CARGA REPETIDA CON ISLOADED
        // Demuestra el Paso 9: IsLoaded permite no ejecutar Load() una segunda vez.
        var diagnostico = _unidad.Ordenes.DiagnosticarIsLoaded("OF-2024-0003");
        const string esperado = "Antes: False | Después: True | Loads ejecutados: 1 | Planchas: 1";
        if (diagnostico != esperado)
            throw new InvalidOperationException($"Error controlado 3.10 inesperado: {diagnostico}");
        Console.WriteLine($"Error controlado 3.10 OK | {diagnostico}");
        */

        /*
        // RETO M03 3.10 - QUERY FILTRADA POR PESO MINIMO
        // Demuestra el Paso 10 con otra orden y confirma que el filtro se aplica antes de Load().
        var reto = _unidad.Ordenes.ObtenerConPlanchasPesadasExplicitas("OF-2024-0005", 150m);
        if (reto is null || reto.Planchas.Count != 1)
            throw new InvalidOperationException("Reto 3.10: se esperaba una única plancha >= 150 kg en OF-2024-0005.");
        if (filtrada is null || filtrada.Planchas.Count != 0)
            throw new InvalidOperationException("Reto 3.10: OF-2024-0002 no debe tener planchas >= 300 kg.");
        Console.WriteLine("Reto 3.10 OK | OF-2024-0005 >=150kg: 1 | OF-2024-0002 >=300kg: 0");
        */
    }
}

// ============================================================================
// FRAGMENTO PDF M03 3.10 - PASO 4
// COPIA PEDAGÓGICA EXACTA DEL BLOQUE PUBLICADO EN M03_PRACTICA.
// El E2E sustituye temporalmente el archivo activo por esta copia y la compila.
// ----------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public sealed class CargaExplicitaUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public CargaExplicitaUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== EXPLICIT LOADING ===");
//         var orden = _unidad.Ordenes.ObtenerConCargaExplicita("OF-2024-0001");
//         var filtrada = _unidad.Ordenes.ObtenerConPlanchasPesadasExplicitas("OF-2024-0002", 300m);
//         if (orden is null || orden.Planchas.Count != 2 || orden.Detalle is null) throw new InvalidOperationException("Carga explícita incompleta.");
//         if (filtrada is null || filtrada.Planchas.Count != 0) throw new InvalidOperationException("Query() de carga explícita inesperado.");
//         Console.WriteLine($"Carga completa: {orden.Planchas.Count} planchas | Filtrada OF-0002: {filtrada.Planchas.Count}");
//     }
// }
// ============================================================================
