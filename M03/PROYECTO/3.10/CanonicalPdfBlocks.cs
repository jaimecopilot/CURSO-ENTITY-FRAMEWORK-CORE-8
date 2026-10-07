// ============================================================================
// M03 3.10 · BLOQUES C# DE LA PRÁCTICA CANÓNICA
// Fuente: M03_PRACTICA reconstruida desde la fuente canónica del usuario.
// Todos los fragmentos siguientes permanecen comentados deliberadamente.
// El estado ejecutable del checkpoint se mantiene en los archivos normales.
// ============================================================================
// ===== CANONICAL PDF M03 3.10 BLOCK 1 =====
// Paso: Paso 2: Añadir los métodos de carga Explicit a la interfaz del repositorio
// Sección: Paso 2: Añadir los métodos de carga Explicit a la interfaz del repositorio
// Ruta indicada por la práctica: src/AceriaData.Application/Interfaces/IOrdenRepositorio.cs
// using AceriaData.Application.Dtos;
// using AceriaData.Domain.Entities;
// 
// namespace AceriaData.Application.Interfaces;
// 
// public interface IOrdenRepositorio
// {
//     // ... métodos existentes ...
// 
//     OrdenFabricacion? CargarPlanchasExplicitamente(int ordenId);
//     OrdenFabricacion? CargarDetalleExplicitamente(int ordenId);
//     OrdenFabricacion? CargarPlanchasActivasExplicitamente(int ordenId);
//     OrdenFabricacion? CargarPlanchasYDetalleExplicitamente(int ordenId);
// 
//     void Agregar(OrdenFabricacion orden);
//     void Eliminar(OrdenFabricacion orden);
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 1 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 2 =====
// Paso: Paso 3: Implementar los métodos de carga Explicit en el repositorio
// Sección: Paso 3: Implementar los métodos de carga Explicit en el repositorio
// Ruta indicada por la práctica: src/AceriaData.Infrastructure/Repositories/OrdenRepositorio.cs
// public OrdenFabricacion? CargarPlanchasExplicitamente(int ordenId)
// {
//     var orden = _context.OrdenesFabricacion.FirstOrDefault(o => o.Id == ordenId);
//     if (orden is null)
//     {
//         return null;
//     }
// 
//     _context.Entry(orden).Collection(o => o.Planchas).Load();
//     return orden;
// }
// 
// public OrdenFabricacion? CargarDetalleExplicitamente(int ordenId)
// {
//     var orden = _context.OrdenesFabricacion.FirstOrDefault(o => o.Id == ordenId);
//     if (orden is null)
//     {
//         return null;
//     }
// 
//     _context.Entry(orden).Reference(o => o.Detalle).Load();
//     return orden;
// }
// 
// public OrdenFabricacion? CargarPlanchasActivasExplicitamente(int ordenId)
// {
//     var orden = _context.OrdenesFabricacion.FirstOrDefault(o => o.Id == ordenId);
//     if (orden is null)
//     {
//         return null;
//     }
// 
//     _context.Entry(orden).Collection(o => o.Planchas).Query().Where(p => p.Activa).Load();
//     return orden;
// }
// 
// public OrdenFabricacion? CargarPlanchasYDetalleExplicitamente(int ordenId)
// {
//     var orden = _context.OrdenesFabricacion.FirstOrDefault(o => o.Id == ordenId);
//     if (orden is null)
//     {
//         return null;
//     }
// 
//     var entry = _context.Entry(orden);
// 
//     if (!entry.Collection(o => o.Planchas).IsLoaded)
//     {
//         entry.Collection(o => o.Planchas).Load();
//     }
// 
//     if (!entry.Reference(o => o.Detalle).IsLoaded)
//     {
//         entry.Reference(o => o.Detalle).Load();
//     }
// 
//     return orden;
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 2 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 3 =====
// Paso: Paso 4: Crear el caso de uso de carga Explicit
// Sección: Paso 4: Crear el caso de uso de carga Explicit
// Ruta indicada por la práctica: src/AceriaData.Application/UseCases/CargaExplicitUseCase.cs
// using AceriaData.Application.Interfaces;
// 
// namespace AceriaData.Application.UseCases;
// 
// public class CargaExplicitUseCase
// {
//     private readonly IUnidadDeTrabajo _unidad;
// 
//     public CargaExplicitUseCase(IUnidadDeTrabajo unidad)
//     {
//         _unidad = unidad;
//     }
// 
//     public void Ejecutar()
//     {
//         Console.WriteLine("=== CARGA EXPLICIT ===");
// 
//         DemostrarCargaReferencia();
//         DemostrarCargaColeccion();
//         DemostrarCargaConFiltro();
//         DemostrarCargaMultipleConIsLoaded();
//     }
// 
//     private void DemostrarCargaReferencia()
//     {
//         Console.WriteLine("\n--- Carga Explicit de referencia (detalle) ---");
// 
//         var orden = _unidad.Ordenes.CargarDetalleExplicitamente(1);
//         if (orden is not null)
//         {
//             Console.WriteLine($"  Orden: {orden.NumeroOrden} | Cliente: {orden.Cliente}");
//             var detalle = orden.Detalle == null ? "Sin detalle" : $"{orden.Detalle.ComposicionQuimica} | {orden.Detalle.TemperaturaColada}°C";
//             Console.WriteLine($"  Detalle: {detalle}");
//         }
//     }
// 
//     private void DemostrarCargaColeccion()
//     {
//         Console.WriteLine("\n--- Carga Explicit de colección (planchas) ---");
// 
//         var orden = _unidad.Ordenes.CargarPlanchasExplicitamente(1);
//         if (orden is not null)
//         {
//             Console.WriteLine($"  Orden: {orden.NumeroOrden} | Planchas: {orden.Planchas.Count}");
//             foreach (var plancha in orden.Planchas)
//             {
//                 Console.WriteLine($"    Plancha {plancha.Id} | Espesor: {plancha.Espesor} | Peso: {plancha.Peso} kg");
//             }
//         }
//     }
// 
//     private void DemostrarCargaConFiltro()
//     {
//         Console.WriteLine("\n--- Carga Explicit con filtro (planchas activas) ---");
// 
//         var orden = _unidad.Ordenes.CargarPlanchasActivasExplicitamente(1);
//         if (orden is not null)
//         {
//             Console.WriteLine($"  Orden: {orden.NumeroOrden} | Planchas activas: {orden.Planchas.Count}");
//             foreach (var plancha in orden.Planchas)
//             {
//                 Console.WriteLine($"    Plancha {plancha.Id} | Espesor: {plancha.Espesor} | Activa: {plancha.Activa}");
//             }
//         }
//     }
// 
//     private void DemostrarCargaMultipleConIsLoaded()
//     {
//         Console.WriteLine("\n--- Carga Explicit de múltiples propiedades con IsLoaded ---");
// 
//         var orden = _unidad.Ordenes.CargarPlanchasYDetalleExplicitamente(1);
//         if (orden is not null)
//         {
//             Console.WriteLine($"  Orden: {orden.NumeroOrden} | Planchas: {orden.Planchas.Count}");
//             var detalle = orden.Detalle == null ? "Sin detalle" : orden.Detalle.ComposicionQuimica;
//             Console.WriteLine($"  Detalle: {detalle}");
//         }
//     }
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 3 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 4 =====
// Paso: Paso 5: Registrar el caso de uso en el contenedor
// Sección: Paso 5: Registrar el caso de uso en el contenedor
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// services.AddScoped<CargaExplicitUseCase>();
// ===== END CANONICAL PDF M03 3.10 BLOCK 4 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 5 =====
// Paso: Paso 6: Llamar al caso de uso desde la consola
// Sección: Paso 6: Llamar al caso de uso desde la consola
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var useCase = scope.ServiceProvider.GetRequiredService<CargaExplicitUseCase>();
//     useCase.Ejecutar();
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 5 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 6 =====
// Paso: Paso 7: Insertar datos de prueba con planchas y detalle
// Sección: Paso 7: Insertar datos de prueba con planchas y detalle
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// using (var scope = _provider.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
//     context.Database.EnsureDeleted();
//     context.Database.EnsureCreated();
// 
//     var orden1 = new OrdenFabricacion { NumeroOrden = "OF-2024-0001", Cliente = "Constructora del Norte", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 1, 15) };
//     var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2024-0002", Cliente = "Constructora del Sur", Estado = "Pendiente", FechaCreacion = new DateTime(2024, 2, 20) };
//     context.OrdenesFabricacion.AddRange(orden1, orden2);
//     context.SaveChanges();
// 
//     var plancha1 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m, Activa = true };
//     var plancha2 = new PlanchaAcero { OrdenId = orden1.Id, Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m, Activa = false };
//     var plancha3 = new PlanchaAcero { OrdenId = orden2.Id, Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m, Activa = true };
//     context.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
// 
//     var detalle1 = new DetalleOrden { OrdenId = orden1.Id, ComposicionQuimica = "C: 0.45%, Mn: 0.75%", TemperaturaColada = 1550.5 };
//     context.DetallesOrden.Add(detalle1);
// 
//     context.SaveChanges();
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 6 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 7 =====
// Paso: Paso 9: Analizar la salida
// Sección: Paso 9: Analizar la salida
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// var entry = _context.Entry(orden);
// 
// if (!entry.Collection(o => o.Planchas).IsLoaded)
// {
//     entry.Collection(o => o.Planchas).Load();
// }
// 
// if (!entry.Reference(o => o.Detalle).IsLoaded)
// {
//     entry.Reference(o => o.Detalle).Load();
// }
// Resultado esperado con la solución: el código solo ejecuta consultas adicionales si la propiedad no estaba cargada previamente.
// 
// Errores comunes del ejercicio
// Error	Causa	Solución
// Consulta innecesaria	Se llamó a Load sin comprobar IsLoaded	Comprobar IsLoaded antes de Load
// Filtro no aplicado	Se aplicó el filtro fuera del Query	Aplicar el filtro dentro del Query
// NullReferenceException	La entidad principal es null	Comprobar antes de llamar a Load
// Referencia circular	Se carga una entidad que referencia a la principal	Usar DTOs o evitar cargar la referencia inversa
// N+1	Se llama a Load en un bucle	Usar Include en su lugar
// Carga Lazy mezclada	Se mezcla carga Explicit con carga Lazy	Elegir una técnica y usarla de forma consistente
// Reto resuelto: Carga Explicit condicional con filtro y ordenación
// Reto: Crear un método en el repositorio que cargue las planchas de una orden de forma explícita, aplicando un filtro por espesor mínimo y una ordenación por peso descendente. Añadir el método a la interfaz, la implementación y una demostración en el caso de uso.
// 
// Solución paso a paso:
// 
// Paso 1: Añadir el método a la interfaz IOrdenRepositorio:
// 
// csharp
// OrdenFabricacion? CargarPlanchasFiltradasYOrdenadasExplicitamente(int ordenId, double espesorMinimo);
// ===== END CANONICAL PDF M03 3.10 BLOCK 7 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 8 =====
// Paso: Paso 2: Implementar el método en OrdenRepositorio:
// Sección: Paso 2: Implementar el método en OrdenRepositorio:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// public OrdenFabricacion? CargarPlanchasFiltradasYOrdenadasExplicitamente(int ordenId, double espesorMinimo)
// {
//     var orden = _context.OrdenesFabricacion.FirstOrDefault(o => o.Id == ordenId);
//     if (orden is null)
//     {
//         return null;
//     }
// 
//     _context.Entry(orden)
//         .Collection(o => o.Planchas)
//         .Query()
//         .Where(p => p.Espesor >= espesorMinimo)
//         .OrderByDescending(p => p.Peso)
//         .Load();
// 
//     return orden;
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 8 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 9 =====
// Paso: Paso 3: Añadir la demostración en el caso de uso:
// Sección: Paso 3: Añadir la demostración en el caso de uso:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// private void DemostrarCargaFiltradaYOrdenada()
// {
//     Console.WriteLine("\n--- Carga Explicit con filtro y ordenación ---");
// 
//     var orden = _unidad.Ordenes.CargarPlanchasFiltradasYOrdenadasExplicitamente(1, 11.0);
//     if (orden is not null)
//     {
//         Console.WriteLine($"  Orden: {orden.NumeroOrden} | Planchas con espesor >= 11.0: {orden.Planchas.Count}");
//         foreach (var plancha in orden.Planchas)
//         {
//             Console.WriteLine($"    Plancha {plancha.Id} | Espesor: {plancha.Espesor} | Peso: {plancha.Peso} kg");
//         }
//     }
// }
// ===== END CANONICAL PDF M03 3.10 BLOCK 9 =====
// ===== CANONICAL PDF M03 3.10 BLOCK 10 =====
// Paso: Paso 4: Llamar al método desde Ejecutar:
// Sección: Paso 4: Llamar al método desde Ejecutar:
// Ruta indicada por la práctica: src/AceriaData.Console/Program.cs
// DemostrarCargaFiltradaYOrdenada();
// Paso 5: Ejecutar dotnet run y verificar que las planchas se muestran filtradas por espesor y ordenadas por peso descendente.
// ===== END CANONICAL PDF M03 3.10 BLOCK 10 =====
