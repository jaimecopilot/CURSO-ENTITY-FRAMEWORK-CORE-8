// ========================================================================
// M05 5.12 · BLOQUES C# LITERALES DEL M05_PRACTICA CANÓNICO
// Blob fuente editorial: 5c38f46a7035e7a1249775293060010bb240796b
// Copias comentadas: no alteran el estado ejecutable.
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 01
// SECTION: Paso 2: Crear el caso de uso de buenas prácticas y anti-patrones
// SOURCE TARGET: Crear el archivo src/AceriaData.Application/UseCases/BuenasPracticasAntiPatronesUseCase.cs:
// ------------------------------------------------------------------------
// using AceriaData.Application.Interfaces;
//
// namespace AceriaData.Application.UseCases;
//
// public class BuenasPracticasAntiPatronesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public BuenasPracticasAntiPatronesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
//
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== BUENAS PRÁCTICAS Y ANTI-PATRONES ===");
//
//         MostrarBuenasPracticasCicloDeVida();
//         MostrarBuenasPracticasModelado();
//         MostrarBuenasPracticasConsultas();
//         MostrarBuenasPracticasEscritura();
//         MostrarBuenasPracticasMigraciones();
//         MostrarBuenasPracticasTesting();
//         MostrarAntiPatronesHabituales();
//     }
//
//     private void MostrarBuenasPracticasCicloDeVida()
//     {
//         Console.WriteLine("\n--- Ciclo de vida del DbContext ---");
//         Console.WriteLine("1. Crear el DbContext por unidad de trabajo.");
//         Console.WriteLine("2. Liberar el DbContext con using.");
//         Console.WriteLine("3. No compartir el DbContext entre hilos.");
//         Console.WriteLine("4. No mantener el DbContext vivo durante toda la aplicación.");
//     }
//
//     private void MostrarBuenasPracticasModelado()
//     {
//         Console.WriteLine("\n--- Modelado y configuración ---");
//         Console.WriteLine("1. Usar Fluent API cuando se necesite configuración centralizada o capacidades que Data Annotations no cubren; Data Annotations también son válidas en escenarios simples.");
//         Console.WriteLine("2. Configurar claves, índices y restricciones explícitamente.");
//         Console.WriteLine("3. Configurar longitudes máximas y precisión decimal.");
//         Console.WriteLine("4. Usar filtros globales para Soft Delete.");
//     }
//
//     private void MostrarBuenasPracticasConsultas()
//     {
//         Console.WriteLine("\n--- Consultas y carga de datos ---");
//         Console.WriteLine("1. Usar proyecciones para reducir el volumen de datos.");
//         Console.WriteLine("2. Usar AsNoTracking en consultas de solo lectura.");
//         Console.WriteLine("3. Elegir Include, proyección o carga explícita según la forma de datos y el caso de uso; Include no es siempre la mejor solución.");
//         Console.WriteLine("4. Evaluar AsSplitQuery cuando varias colecciones provoquen explosión cartesiana, considerando roundtrips y consistencia.");
//         Console.WriteLine("5. Aplicar filtros y paginación en el servidor.");
//         Console.WriteLine("6. Revisar funciones sobre columnas en Where por traducción y sargabilidad; el uso de índices depende del proveedor, expresión e índice.");
//         Console.WriteLine("7. Evitar métodos .NET no traducibles dentro de Where salvo que se introduzca explícitamente una frontera de evaluación cliente.");
//     }
//
//     private void MostrarBuenasPracticasEscritura()
//     {
//         Console.WriteLine("\n--- Escritura y transacciones ---");
//         Console.WriteLine("1. Agrupar operaciones en una unidad de trabajo.");
//         Console.WriteLine("2. Usar transacciones explícitas cuando sea necesario.");
//         Console.WriteLine("3. Mantener las transacciones cortas.");
//         Console.WriteLine("4. Gestionar los conflictos de concurrencia.");
//     }
//
//     private void MostrarBuenasPracticasMigraciones()
//     {
//         Console.WriteLine("\n--- Migraciones y despliegue ---");
//         Console.WriteLine("1. Generar migraciones con nombres descriptivos.");
//         Console.WriteLine("2. No modificar migraciones ya aplicadas.");
//         Console.WriteLine("3. Usar scripts idempotentes en producción.");
//         Console.WriteLine("4. Hacer copias de seguridad antes de aplicar.");
//         Console.WriteLine("5. Preparar planes de reversión.");
//     }
//
//     private void MostrarBuenasPracticasTesting()
//     {
//         Console.WriteLine("\n--- Testing y diagnóstico ---");
//         Console.WriteLine("1. Usar SQLite en memoria para tests relacionales rápidos, documentando sus diferencias con SQL Server.");
//         Console.WriteLine("2. Usar InMemory sólo cuando sus diferencias no invaliden el comportamiento que se quiere comprobar.");
//         Console.WriteLine("3. Configurar logging con ILogger o Serilog.");
//         Console.WriteLine("4. Usar observadores de diagnóstico para detectar consultas lentas.");
//     }
//
//     private void MostrarAntiPatronesHabituales()
//     {
//         Console.WriteLine("\n--- Anti-patrones habituales ---");
//         Console.WriteLine("1. DbContext estático compartido.");
//         Console.WriteLine("2. Repositorio genérico sin valor arquitectónico o que fuerza operaciones que el dominio no necesita.");
//         Console.WriteLine("3. Exponer IQueryable a través de una frontera donde filtra detalles del proveedor o permite composición no controlada.");
//         Console.WriteLine("4. Problema N+1.");
//         Console.WriteLine("5. Over-fetching.");
//         Console.WriteLine("6. Carga Lazy sin control.");
//         Console.WriteLine("7. Materialización prematura.");
//         Console.WriteLine("8. Expresiones en Where que no se traducen o perjudican innecesariamente la sargabilidad.");
//         Console.WriteLine("9. Métodos .NET no traducibles en Where sin una frontera cliente explícita.");
//         Console.WriteLine("10. Transacciones largas.");
//         Console.WriteLine("11. Migraciones modificadas.");
//         Console.WriteLine("12. Tests que siempre pasan.");
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 02
// SECTION: Paso 3: Registrar el caso de uso en el contenedor
// SOURCE TARGET: Modificar src/AceriaData.Console/Program.cs:
// ------------------------------------------------------------------------
// services.AddScoped<BuenasPracticasAntiPatronesUseCase>();
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 03
// SECTION: Paso 4: Llamar al caso de uso desde la consola
// SOURCE TARGET: Modificar el método Main:
// ------------------------------------------------------------------------
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<BuenasPracticasAntiPatronesUseCase>();
//     useCase.Ejecutar();
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 04
// SECTION: Paso 7: Diagnosticar un error común
// SOURCE TARGET: Modificar el caso de uso para usar un DbContext estático compartido:
// ------------------------------------------------------------------------
// public class BuenasPracticasAntiPatronesUseCase
// {
//     private static readonly AceriaDbContext _contextCompartido = new AceriaDbContext();
//     // ...
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 05
// SECTION: Paso 7: Diagnosticar un error común
// SOURCE TARGET: Paso 7: Diagnosticar un error común
// ------------------------------------------------------------------------
// public class BuenasPracticasAntiPatronesUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
//
//     public BuenasPracticasAntiPatronesUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 06
// SECTION: Paso 1: Crear el DTO HallazgoAuditoriaDto:
// SOURCE TARGET: Paso 1: Crear el DTO HallazgoAuditoriaDto:
// ------------------------------------------------------------------------
// namespace AceriaData.Application.Dtos;
//
// public class HallazgoAuditoriaDto
// {
//     public string Metodo { get; set; } = string.Empty;
//     public string Practica { get; set; } = string.Empty;
//     public string Recomendacion { get; set; } = string.Empty;
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 07
// SECTION: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// SOURCE TARGET: Paso 2: Añadir el método a la interfaz IOrdenRepositorio:
// ------------------------------------------------------------------------
// List<HallazgoAuditoriaDto> AuditarBuenasPracticas();
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 08
// SECTION: Paso 3: Implementar el método en OrdenRepositorio:
// SOURCE TARGET: Paso 3: Implementar el método en OrdenRepositorio:
// ------------------------------------------------------------------------
// public List<HallazgoAuditoriaDto> AuditarBuenasPracticas()
// {
//     return new List<HallazgoAuditoriaDto>
//     {
//         new HallazgoAuditoriaDto
//         {
//             Metodo = "ObtenerResumenes",
//             Practica = "Usa AsNoTracking y proyección",
//             Recomendacion = "Correcto"
//         },
//         new HallazgoAuditoriaDto
//         {
//             Metodo = "ObtenerConPlanchasInclude",
//             Practica = "Usa Include y AsNoTracking",
//             Recomendacion = "Correcto"
//         },
//         new HallazgoAuditoriaDto
//         {
//             Metodo = "ObtenerConPlanchasYDetalleSplitQuery",
//             Practica = "Usa AsSplitQuery",
//             Recomendacion = "Correcto"
//         },
//         new HallazgoAuditoriaDto
//         {
//             Metodo = "ObtenerPorIdParaActualizar",
//             Practica = "Usa tracking para modificar",
//             Recomendacion = "Correcto"
//         }
//     };
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 09
// SECTION: Paso 4: Añadir la demostración en el caso de uso:
// SOURCE TARGET: Paso 4: Añadir la demostración en el caso de uso:
// ------------------------------------------------------------------------
// private void DemostrarAuditoria()
// {
//     Console.WriteLine("\n--- Auditoría de buenas prácticas ---");
//
//     var hallazgos = _unidad.Ordenes.AuditarBuenasPracticas();
//     foreach (var hallazgo in hallazgos)
//     {
//         Console.WriteLine($"Método: {hallazgo.Metodo} | Práctica: {hallazgo.Practica} | Recomendación: {hallazgo.Recomendacion}");
//     }
// }
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 10
// SECTION: Paso 5: Llamar al método desde Ejecutar:
// SOURCE TARGET: Paso 5: Llamar al método desde Ejecutar:
// ------------------------------------------------------------------------
// DemostrarAuditoria();
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 11
// SECTION: Paso 8: Reproducir un N+1 intencionado
// SOURCE TARGET: Paso 8: Reproducir un N+1 intencionado
// ------------------------------------------------------------------------
// SqlCommandCounterInterceptor.Instance.Reset();
//
// var ordenes = context.OrdenesFabricacion
//     .AsNoTracking()
//     .OrderBy(o => o.Id)
//     .ToList();
//
// var resultadoN1 = ordenes.Select(o => new
// {
//     o.NumeroOrden,
//     TotalPlanchas = context.PlanchasAcero.Count(p => p.OrdenId == o.Id)
// }).ToList();
//
// var comandosN1 = SqlCommandCounterInterceptor.Instance.Count;
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 12
// SECTION: Paso 9: Primera corrección con carga anticipada cuando necesitas el grafo
// SOURCE TARGET: Paso 9: Primera corrección con carga anticipada cuando necesitas el grafo
// ------------------------------------------------------------------------
// SqlCommandCounterInterceptor.Instance.Reset();
//
// var conInclude = context.OrdenesFabricacion
//     .AsNoTracking()
//     .Include(o => o.Planchas)
//     .OrderBy(o => o.Id)
//     .ToList()
//     .Select(o => new
//     {
//         o.NumeroOrden,
//         TotalPlanchas = o.Planchas.Count
//     })
//     .ToList();
//
// var comandosInclude = SqlCommandCounterInterceptor.Instance.Count;
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 13
// SECTION: Paso 10: Segunda mejora con proyección
// SOURCE TARGET: Paso 10: Segunda mejora con proyección
// ------------------------------------------------------------------------
// SqlCommandCounterInterceptor.Instance.Reset();
//
// var proyectado = context.OrdenesFabricacion
//     .AsNoTracking()
//     .OrderBy(o => o.Id)
//     .Select(o => new
//     {
//         o.NumeroOrden,
//         TotalPlanchas = o.Planchas.Count()
//     })
//     .ToList();
//
// var comandosProyeccion = SqlCommandCounterInterceptor.Instance.Count;
// ========================================================================

// CANONICAL PDF M05 5.12 - BLOCK 14
// SECTION: Paso 11: Verificar equivalencia funcional y evidencia técnica
// SOURCE TARGET: Paso 11: Verificar equivalencia funcional y evidencia técnica
// ------------------------------------------------------------------------
// Assert.Equal(
//     resultadoN1.Select(x => (x.NumeroOrden, x.TotalPlanchas)),
//     conInclude.Select(x => (x.NumeroOrden, x.TotalPlanchas)));
//
// Assert.Equal(
//     resultadoN1.Select(x => (x.NumeroOrden, x.TotalPlanchas)),
//     proyectado.Select(x => (x.NumeroOrden, x.TotalPlanchas)));
// ========================================================================

