using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class IdentityResolutionUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public IdentityResolutionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===");
        var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();
        var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();

        if (sin.Filas <= sin.ClavesUnicas)
            throw new InvalidOperationException("4.3: el dataset no contiene una entidad relacionada repetida.");
        if (sin.InstanciasUnicas != sin.Filas)
            throw new InvalidOperationException("4.3: AsNoTracking resolvio identidades cuando no debia.");
        if (con.InstanciasUnicas != con.ClavesUnicas)
            throw new InvalidOperationException("4.3: AsNoTrackingWithIdentityResolution no deduplico por clave.");

        Console.WriteLine($"AsNoTracking: filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias={sin.InstanciasUnicas}");
        Console.WriteLine($"IdentityResolution: filas={con.Filas}, claves={con.ClavesUnicas}, instancias={con.InstanciasUnicas}");
    }

    /*
    // ERROR CONTROLADO M04 4.3 - PLANCHA NO DEMUESTRA IDENTITY RESOLUTION
    public void EjecutarErrorEntidadNoRepetida()
    {
        var sin = _unidad.Ordenes.MedirPlanchasSinResolucionM4();
        var con = _unidad.Ordenes.MedirPlanchasConResolucionM4();

        if (sin.Filas == 0 || sin.Filas != sin.ClavesUnicas || sin.InstanciasUnicas != sin.ClavesUnicas)
            throw new InvalidOperationException("4.3 error controlado: el contraejemplo de planchas no es único por clave.");
        if (con.Filas != sin.Filas || con.ClavesUnicas != sin.ClavesUnicas || con.InstanciasUnicas != sin.InstanciasUnicas)
            throw new InvalidOperationException("4.3 error controlado: IdentityResolution cambió un conjunto sin claves repetidas.");

        Console.WriteLine(
            $"Error controlado 4.3 OK | Plancha no demuestra resolución | filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias sin={sin.InstanciasUnicas}, con={con.InstanciasUnicas}");
    }
    */

    /*
    // RETO M04 4.3 - COMPARAR REFERENCIAS DE ALEACION COMPARTIDA
    public void EjecutarRetoReferencias()
    {
        var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();
        var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();

        var duplicaReferencias = sin.InstanciasUnicas > sin.ClavesUnicas;
        var reutilizaReferencias = con.InstanciasUnicas == con.ClavesUnicas;

        if (sin.Filas != 4 || sin.ClavesUnicas != 2 || sin.InstanciasUnicas != 4)
            throw new InvalidOperationException("Reto 4.3: se esperaban 4 relaciones, 2 claves y 4 referencias sin resolución.");
        if (con.Filas != 4 || con.ClavesUnicas != 2 || con.InstanciasUnicas != 2)
            throw new InvalidOperationException("Reto 4.3: se esperaban 4 relaciones, 2 claves y 2 referencias con resolución.");
        if (!duplicaReferencias || !reutilizaReferencias)
            throw new InvalidOperationException("Reto 4.3: la comparación por referencia no refleja la resolución de identidad.");

        Console.WriteLine(
            $"Reto 4.3 OK | 4 relaciones / 2 aleaciones | sin={sin.InstanciasUnicas} referencias | con={con.InstanciasUnicas} referencias");
    }
    */

}


// FRAGMENTO PDF M04 4.3 - PASO 5
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public sealed class IdentityResolutionUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//     public IdentityResolutionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===");
//         var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();
//         var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();
//
//         if (sin.Filas <= sin.ClavesUnicas)
//             throw new InvalidOperationException("4.3: el dataset no contiene una entidad relacionada repetida.");
//         if (sin.InstanciasUnicas != sin.Filas)
//             throw new InvalidOperationException("4.3: AsNoTracking resolvio identidades cuando no debia.");
//         if (con.InstanciasUnicas != con.ClavesUnicas)
//             throw new InvalidOperationException("4.3: AsNoTrackingWithIdentityResolution no deduplico por clave.");
//
//         Console.WriteLine($"AsNoTracking: filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias={sin.InstanciasUnicas}");
//         Console.WriteLine($"IdentityResolution: filas={con.Filas}, claves={con.ClavesUnicas}, instancias={con.InstanciasUnicas}");
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 03
// SECTION: Paso 4: Crear el caso de uso de resolución de identidad
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/ResolucionIdentidadUseCase.cs:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// using System.Diagnostics;
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class ResolucionIdentidadUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public ResolucionIdentidadUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== ASNOTRACKING VS ASNOTRACKINGWITHIDENTITYRESOLUTION ===");
//
//         DemostrarAsNoTracking();
//         DemostrarConResolucionIdentidad();
//         CompararRendimiento();
//     }
//
//     private void DemostrarAsNoTracking()
//     {
//         Console.WriteLine("\n--- AsNoTracking ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerConPlanchasAsNoTracking();
//         var instancias = _unidad.Ordenes.ContarPlanchasInstanciadas(ordenes);
//
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         Console.WriteLine($"Instancias de planchas: {instancias}");
//     }
//
//     private void DemostrarConResolucionIdentidad()
//     {
//         Console.WriteLine("\n--- AsNoTrackingWithIdentityResolution ---");
//
//         var ordenes = _unidad.Ordenes.ObtenerConPlanchasConResolucionIdentidad();
//         var instancias = _unidad.Ordenes.ContarPlanchasInstanciadas(ordenes);
//
//         Console.WriteLine($"Órdenes: {ordenes.Count}");
//         Console.WriteLine($"Instancias de planchas: {instancias}");
//     }
//
//     private void CompararRendimiento()
//     {
//         Console.WriteLine("\n--- Comparación de rendimiento ---");
//
//         var cronometroNoTracking = Stopwatch.StartNew();
//         var ordenesNoTracking = _unidad.Ordenes.ObtenerConPlanchasAsNoTracking();
//         cronometroNoTracking.Stop();
//
//         var cronometroResolucion = Stopwatch.StartNew();
//         var ordenesResolucion = _unidad.Ordenes.ObtenerConPlanchasConResolucionIdentidad();
//         cronometroResolucion.Stop();
//
//         Console.WriteLine($"AsNoTracking: {cronometroNoTracking.ElapsedMilliseconds} ms | Órdenes: {ordenesNoTracking.Count}");
//         Console.WriteLine($"AsNoTrackingWithIdentityResolution: {cronometroResolucion.ElapsedMilliseconds} ms | Órdenes: {ordenesResolucion.Count}");
//     }
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 07
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Modificar el método ContarPlanchasInstanciadas para usar HashSet<PlanchaAcero> sin ReferenceEqualityComparer:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// public int ContarPlanchasInstanciadas(List<OrdenFabricacion> ordenes)
// {
//     var instancias = new HashSet<PlanchaAcero>();
//     foreach (var orden in ordenes)
//     {
//         foreach (var plancha in orden.Planchas)
//         {
//             instancias.Add(plancha);
//         }
//     }
//     return instancias.Count;
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 08
// SECTION: Paso 10: Diagnosticar un error común
// SOURCE TARGET: Paso 10: Diagnosticar un error común
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// var instancias = new HashSet<PlanchaAcero>(ReferenceEqualityComparer.Instance);
// Resultado esperado con la solución: el HashSet compara por referencia y cuenta las instancias correctamente.
//
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 11
// SECTION: Paso 3: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 3: Añadir la demostración en el caso de uso:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// private void DemostrarDosColecciones()
// {
//     Console.WriteLine("\n--- Dos colecciones: AsNoTracking ---");
//     var ordenes1 = _unidad.Ordenes.ObtenerConDosColeccionesAsNoTracking();
//     var aleaciones1 = new HashSet<Aleacion>(ReferenceEqualityComparer.Instance);
//     foreach (var orden in ordenes1)
//     {
//         foreach (var oa in orden.OrdenesAleaciones)
//         {
//             aleaciones1.Add(oa.Aleacion);
//         }
//     }
//     Console.WriteLine($"Instancias de aleaciones: {aleaciones1.Count}");
//
//     Console.WriteLine("\n--- Dos colecciones: AsNoTrackingWithIdentityResolution ---");
//     var ordenes2 = _unidad.Ordenes.ObtenerConDosColeccionesConResolucionIdentidad();
//     var aleaciones2 = new HashSet<Aleacion>(ReferenceEqualityComparer.Instance);
//     foreach (var orden in ordenes2)
//     {
//         foreach (var oa in orden.OrdenesAleaciones)
//         {
//             aleaciones2.Add(oa.Aleacion);
//         }
//     }
//     Console.WriteLine($"Instancias de aleaciones: {aleaciones2.Count}");
// }
// ========================================================================

// CANONICAL INLINE M04 4.3 - BLOCK 12
// SECTION: Paso 4: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 4: Llamar al método desde Ejecutar:
// ACTIVATION: fragmento/copia literal del paso canónico; activar en una copia temporal según el paso.
// ------------------------------------------------------------------------
// DemostrarDosColecciones();
// ========================================================================
