# Curso Profesional de Entity Framework Core 8

# Módulo 4 — Prácticas: Optimización y rendimiento

**Autor: JAIME GALLO**

Cada práctica trabaja sobre un estado completo y ejecutable de AceriaData. La secuencia es acumulativa desde M03/PROYECTO/3.12.

## Punto 4.1 — Análisis del SQL generado: ToQueryString y logging

### Contexto del proyecto

Este punto continúa M03/PROYECTO/3.12. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

ToQueryString inspecciona la representación SQL sin materializar; el logging muestra los comandos realmente ejecutados. Son herramientas complementarias: una muestra la consulta prevista y la otra la ejecución real.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.1
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento41.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `ObtenerSqlConIncludeM4`, `ObtenerSqlConProyeccionM4`, `ObtenerSqlPendientesOrdenadasM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public string ObtenerSqlPendientesOrdenadasM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion)
        .ThenBy(o => o.Id)
        .ToQueryString();

    public string ObtenerSqlConIncludeM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Include(o => o.Planchas)
        .Where(o => o.Cliente == "Constructora del Norte")
        .OrderBy(o => o.Id)
        .ToQueryString();

    public string ObtenerSqlConProyeccionM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion)
        .ThenBy(o => o.Id)
        .Select(o => new { o.NumeroOrden, o.Cliente })
        .ToQueryString();
}
```

#### Explicación línea a línea — Rendimiento41.cs

Línea 1: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    public string ObtenerSqlPendientesOrdenadasM4() => _context.OrdenesFabricacion` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 8: `        .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 9: `        .Where(o => o.Estado == "Pendiente")` → Añade el predicado de filtrado a la forma de consulta.

Línea 10: `        .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 11: `        .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 12: `        .ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 14: `    public string ObtenerSqlConIncludeM4() => _context.OrdenesFabricacion` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 15: `        .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 16: `        .Include(o => o.Planchas)` → Define la navegación relacionada que debe cargarse.

Línea 17: `        .Where(o => o.Cliente == "Constructora del Norte")` → Añade el predicado de filtrado a la forma de consulta.

Línea 18: `        .OrderBy(o => o.Id)` → Forma parte del orden determinista.

Línea 19: `        .ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 21: `    public string ObtenerSqlConProyeccionM4() => _context.OrdenesFabricacion` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 22: `        .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 23: `        .Where(o => o.Estado == "Pendiente")` → Añade el predicado de filtrado a la forma de consulta.

Línea 24: `        .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 25: `        .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 26: `        .Select(o => new { o.NumeroOrden, o.Cliente })` → Proyecta la forma de resultado y controla datos materializados.

Línea 27: `        .ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 28: `}` → Delimita el bloque sintáctico asociado.


Archivo complementario: src/AceriaData.Infrastructure/DependencyInjection.cs

```csharp
using AceriaData.Application.Interfaces;
using AceriaData.Infrastructure.Persistence;
using AceriaData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AceriaData.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAceriaInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AceriaDbContext>(o => o
            .UseSqlServer(connectionString)
            .EnableDetailedErrors()
            .LogTo(
                Console.WriteLine,
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information));
        services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        return services;
    }
}
```

#### Explicación línea a línea — DependencyInjection.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure.Repositories;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Logging;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 8: `namespace AceriaData.Infrastructure;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 10: `public static class DependencyInjection` → Continúa la implementación del punto con esta expresión: public static class DependencyInjection

Línea 11: `{` → Delimita el bloque sintáctico asociado.

Línea 12: `    public static IServiceCollection AddAceriaInfrastructure(this IServiceCollection services, string connectionString)` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 13: `    {` → Delimita el bloque sintáctico asociado.

Línea 14: `        services.AddDbContext<AceriaDbContext>(o => o` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 15: `            .UseSqlServer(connectionString)` → Continúa la composición fluida invocando UseSqlServer sobre el resultado de la línea anterior.

Línea 16: `            .EnableDetailedErrors()` → Continúa la composición fluida invocando EnableDetailedErrors sobre el resultado de la línea anterior.

Línea 17: `            .LogTo(` → Continúa la composición fluida invocando LogTo sobre el resultado de la línea anterior.

Línea 18: `                Console.WriteLine,` → Publica evidencia observable en la consola.

Línea 19: `                new[] { DbLoggerCategory.Database.Command.Name },` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 20: `                LogLevel.Information));` → Continúa la implementación del punto con esta expresión: LogLevel.Information));

Línea 21: `        services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `        return services;` → Devuelve el resultado calculado al llamador.

Línea 24: `    }` → Delimita el bloque sintáctico asociado.

Línea 25: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class AnalisisSqlUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public AnalisisSqlUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.1 ANALISIS DEL SQL GENERADO ===");
        var pendientes = _unidad.Ordenes.ObtenerSqlPendientesOrdenadasM4();
        var include = _unidad.Ordenes.ObtenerSqlConIncludeM4();
        var proyeccion = _unidad.Ordenes.ObtenerSqlConProyeccionM4();

        if (!pendientes.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.1: el SQL de pendientes no contiene filtro.");
        if (!include.Contains("JOIN", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.1: el SQL con Include no contiene JOIN.");
        if (proyeccion.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.1: la proyeccion recupera columnas no solicitadas.");

        Console.WriteLine("--- SQL pendientes ---");
        Console.WriteLine(pendientes);
        Console.WriteLine("--- SQL Include ---");
        Console.WriteLine(include);
        Console.WriteLine("--- SQL proyeccion ---");
        Console.WriteLine(proyeccion);
    }
}
```

#### Explicación línea a línea — AnalisisSqlUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class AnalisisSqlUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public AnalisisSqlUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.1 ANALISIS DEL SQL GENERADO ===");` → Publica evidencia observable en la consola.

Línea 13: `        var pendientes = _unidad.Ordenes.ObtenerSqlPendientesOrdenadasM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var include = _unidad.Ordenes.ObtenerSqlConIncludeM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        var proyeccion = _unidad.Ordenes.ObtenerSqlConProyeccionM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 17: `        if (!pendientes.Contains("WHERE", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 18: `            throw new InvalidOperationException("4.1: el SQL de pendientes no contiene filtro.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 19: `        if (!include.Contains("JOIN", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 20: `            throw new InvalidOperationException("4.1: el SQL con Include no contiene JOIN.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 21: `        if (proyeccion.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 22: `            throw new InvalidOperationException("4.1: la proyeccion recupera columnas no solicitadas.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 24: `        Console.WriteLine("--- SQL pendientes ---");` → Publica evidencia observable en la consola.

Línea 25: `        Console.WriteLine(pendientes);` → Publica evidencia observable en la consola.

Línea 26: `        Console.WriteLine("--- SQL Include ---");` → Publica evidencia observable en la consola.

Línea 27: `        Console.WriteLine(include);` → Publica evidencia observable en la consola.

Línea 28: `        Console.WriteLine("--- SQL proyeccion ---");` → Publica evidencia observable en la consola.

Línea 29: `        Console.WriteLine(proyeccion);` → Publica evidencia observable en la consola.

Línea 30: `    }` → Delimita el bloque sintáctico asociado.

Línea 31: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateOnBuild = true,
    ValidateScopes = true
});
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<AnalisisSqlUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.1 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa tipos o extensiones requeridos por esta implementación.

Línea 24: `{` → Delimita el bloque sintáctico asociado.

Línea 25: `    ValidateOnBuild = true,` → Ordena validar el grafo de dependencias al construir el proveedor de servicios.

Línea 26: `    ValidateScopes = true` → Activa la comprobación de ciclos de vida Scoped para detectar resoluciones incorrectas.

Línea 27: `});` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 28: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 30: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 31: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 32: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 33: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 34: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 36: `var useCase = scope.ServiceProvider.GetRequiredService<AnalisisSqlUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 37: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 39: `Console.WriteLine("4.1 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.1 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Comparar el SQL de una entidad completa con el de una proyección y justificar qué columnas sobran.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.1 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- ToQueryString antes de materializar y logging de comandos SQL.
- Consultas con Where, OrderBy, Select e Include, incluyendo el filtro global de Soft Delete.
- Análisis de múltiples Include como origen potencial de multiplicación de filas.

### Reto de ampliación

Construye mentalmente una consulta con dos colecciones incluidas y anticipa cómo crecerían las filas; compruébalo después en 4.8.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No materializar con ToList antes de pedir ToQueryString.
- No exponer IQueryable desde Application.
- No resolver servicios Scoped desde el proveedor raíz.

### Analogía operativa

ToQueryString es el plano previo; el logging es el registro de lo que realmente pasó por la línea.

### Resultado esperado

Al finalizar el punto 4.1, el proyecto debe compilar, ejecutarse y terminar con 4.1 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.2 y continúa directamente desde este proyecto.

---

## Punto 4.2 — Tracking y No Tracking

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.1. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

Tracking y NoTracking normalmente no cambian el SELECT: cambian sobre todo materialización y ChangeTracker. Un DTO puro sin entidades no se rastrea; una proyección que contenga entidades sí puede mantener tracking de esas entidades.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.2
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento42.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `MedirConsultaConTrackingM4`, `MedirConsultaSinTrackingM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public TrackingMetricaDto MedirConsultaConTrackingM4()
    {
        _context.ChangeTracker.Clear();
        var consulta = _context.OrdenesFabricacion
            .AsTracking()
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }

    public TrackingMetricaDto MedirConsultaSinTrackingM4()
    {
        _context.ChangeTracker.Clear();
        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id);
        var sql = consulta.ToQueryString();
        var filas = consulta.ToList().Count;
        return new TrackingMetricaDto
        {
            Filas = filas,
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql
        };
    }
}
```

#### Explicación línea a línea — Rendimiento42.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public TrackingMetricaDto MedirConsultaConTrackingM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 9: `    {` → Delimita el bloque sintáctico asociado.

Línea 10: `        _context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 11: `        var consulta = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 12: `            .AsTracking()` → Fuerza tracking para hacer observable el ChangeTracker.

Línea 13: `            .OrderBy(o => o.Id);` → Forma parte del orden determinista.

Línea 14: `        var sql = consulta.ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 15: `        var filas = consulta.ToList().Count;` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 16: `        return new TrackingMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 17: `        {` → Delimita el bloque sintáctico asociado.

Línea 18: `            Filas = filas,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 19: `            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 20: `            Sql = sql` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 21: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 22: `    }` → Delimita el bloque sintáctico asociado.

Línea 24: `    public TrackingMetricaDto MedirConsultaSinTrackingM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 25: `    {` → Delimita el bloque sintáctico asociado.

Línea 26: `        _context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 27: `        var consulta = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 28: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 29: `            .OrderBy(o => o.Id);` → Forma parte del orden determinista.

Línea 30: `        var sql = consulta.ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 31: `        var filas = consulta.ToList().Count;` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 32: `        return new TrackingMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 33: `        {` → Delimita el bloque sintáctico asociado.

Línea 34: `            Filas = filas,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 35: `            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 36: `            Sql = sql` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 37: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 38: `    }` → Delimita el bloque sintáctico asociado.

Línea 39: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TrackingUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public TrackingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.2 TRACKING Y NO TRACKING ===");
        var con = _unidad.Ordenes.MedirConsultaConTrackingM4();
        var sin = _unidad.Ordenes.MedirConsultaSinTrackingM4();

        if (con.Filas == 0 || con.EntidadesRastreadas != con.Filas)
            throw new InvalidOperationException("4.2: tracking no produjo el numero esperado de entradas.");
        if (sin.Filas != con.Filas || sin.EntidadesRastreadas != 0)
            throw new InvalidOperationException("4.2: AsNoTracking dejo entidades rastreadas.");

        Console.WriteLine($"Con tracking: filas={con.Filas}, rastreadas={con.EntidadesRastreadas}");
        Console.WriteLine($"Sin tracking: filas={sin.Filas}, rastreadas={sin.EntidadesRastreadas}");
        Console.WriteLine("El SQL puede ser equivalente; la diferencia relevante esta en la materializacion y el ChangeTracker.");
    }
}
```

#### Explicación línea a línea — TrackingUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class TrackingUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public TrackingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.2 TRACKING Y NO TRACKING ===");` → Publica evidencia observable en la consola.

Línea 13: `        var con = _unidad.Ordenes.MedirConsultaConTrackingM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var sin = _unidad.Ordenes.MedirConsultaSinTrackingM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        if (con.Filas == 0 || con.EntidadesRastreadas != con.Filas)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 17: `            throw new InvalidOperationException("4.2: tracking no produjo el numero esperado de entradas.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 18: `        if (sin.Filas != con.Filas || sin.EntidadesRastreadas != 0)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException("4.2: AsNoTracking dejo entidades rastreadas.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 21: `        Console.WriteLine($"Con tracking: filas={con.Filas}, rastreadas={con.EntidadesRastreadas}");` → Publica evidencia observable en la consola.

Línea 22: `        Console.WriteLine($"Sin tracking: filas={sin.Filas}, rastreadas={sin.EntidadesRastreadas}");` → Publica evidencia observable en la consola.

Línea 23: `        Console.WriteLine("El SQL puede ser equivalente; la diferencia relevante esta en la materializacion y el ChangeTracker.");` → Publica evidencia observable en la consola.

Línea 24: `    }` → Delimita el bloque sintáctico asociado.

Línea 25: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<TrackingUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.2 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 25: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 27: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 28: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 29: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 30: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 31: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 33: `var useCase = scope.ServiceProvider.GetRequiredService<TrackingUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 34: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 36: `Console.WriteLine("4.2 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.2 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Explicar por qué dos consultas con SQL parecido pueden tener distinto coste de materialización.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.2 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Tracking, AsTracking, AsNoTracking y coste del ChangeTracker.
- Conteo de entidades rastreadas y comparación aislada entre consultas.
- Tracking de grafos con entidades relacionadas.

### Reto de ampliación

Carga un grafo con relaciones con y sin tracking y razona qué entidades quedarían registradas.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No interpretar SQL idéntico como coste idéntico de materialización.
- No reutilizar estado previo del ChangeTracker al medir.
- No registrar DbContext como Singleton.

### Analogía operativa

Tracking es mantener una ficha viva de cada pieza; NoTracking es leerla sin abrir expediente.

### Resultado esperado

Al finalizar el punto 4.2, el proyecto debe compilar, ejecutarse y terminar con 4.2 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.3 y continúa directamente desde este proyecto.

---

## Punto 4.3 — AsNoTracking y AsNoTrackingWithIdentityResolution

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.2. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

La resolución de identidad solo se demuestra si la misma clave aparece repetida. AceriaData usa Aleacion porque una misma aleación está relacionada con varias órdenes; PlanchaAcero pertenece a una sola orden y no es una evidencia válida.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.3
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento43.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `MedirNoTrackingConResolucionM4`, `MedirNoTrackingSinResolucionM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public IdentityResolutionMetricaDto MedirNoTrackingSinResolucionM4()
    {
        _context.ChangeTracker.Clear();
        var entidades = _context.OrdenesAleaciones
            .AsNoTracking()
            .OrderBy(oa => oa.OrdenFabricacionId)
            .Select(oa => oa.Aleacion)
            .ToList();

        return new IdentityResolutionMetricaDto
        {
            Filas = entidades.Count,
            ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),
            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
        };
    }

    public IdentityResolutionMetricaDto MedirNoTrackingConResolucionM4()
    {
        _context.ChangeTracker.Clear();
        var entidades = _context.OrdenesAleaciones
            .AsNoTrackingWithIdentityResolution()
            .OrderBy(oa => oa.OrdenFabricacionId)
            .Select(oa => oa.Aleacion)
            .ToList();

        return new IdentityResolutionMetricaDto
        {
            Filas = entidades.Count,
            ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),
            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()
        };
    }
}
```

#### Explicación línea a línea — Rendimiento43.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public IdentityResolutionMetricaDto MedirNoTrackingSinResolucionM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 9: `    {` → Delimita el bloque sintáctico asociado.

Línea 10: `        _context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 11: `        var entidades = _context.OrdenesAleaciones` → Calcula y conserva el resultado que será validado o mostrado.

Línea 12: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 13: `            .OrderBy(oa => oa.OrdenFabricacionId)` → Forma parte del orden determinista.

Línea 14: `            .Select(oa => oa.Aleacion)` → Proyecta la forma de resultado y controla datos materializados.

Línea 15: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 17: `        return new IdentityResolutionMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 18: `        {` → Delimita el bloque sintáctico asociado.

Línea 19: `            Filas = entidades.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 20: `            ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),` → Proyecta la forma de resultado y controla datos materializados.

Línea 21: `            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 22: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 23: `    }` → Delimita el bloque sintáctico asociado.

Línea 25: `    public IdentityResolutionMetricaDto MedirNoTrackingConResolucionM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 26: `    {` → Delimita el bloque sintáctico asociado.

Línea 27: `        _context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 28: `        var entidades = _context.OrdenesAleaciones` → Calcula y conserva el resultado que será validado o mostrado.

Línea 29: `            .AsNoTrackingWithIdentityResolution()` → Activa NoTracking con resolución temporal de identidad.

Línea 30: `            .OrderBy(oa => oa.OrdenFabricacionId)` → Forma parte del orden determinista.

Línea 31: `            .Select(oa => oa.Aleacion)` → Proyecta la forma de resultado y controla datos materializados.

Línea 32: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 34: `        return new IdentityResolutionMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 35: `        {` → Delimita el bloque sintáctico asociado.

Línea 36: `            Filas = entidades.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 37: `            ClavesUnicas = entidades.Select(a => a.Id).Distinct().Count(),` → Proyecta la forma de resultado y controla datos materializados.

Línea 38: `            InstanciasUnicas = entidades.Distinct(ReferenceEqualityComparer.Instance).Count()` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 39: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 40: `    }` → Delimita el bloque sintáctico asociado.

Línea 41: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
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
}
```

#### Explicación línea a línea — IdentityResolutionUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class IdentityResolutionUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public IdentityResolutionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.3 NO TRACKING E IDENTITY RESOLUTION ===");` → Publica evidencia observable en la consola.

Línea 13: `        var sin = _unidad.Ordenes.MedirNoTrackingSinResolucionM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var con = _unidad.Ordenes.MedirNoTrackingConResolucionM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        if (sin.Filas <= sin.ClavesUnicas)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 17: `            throw new InvalidOperationException("4.3: el dataset no contiene una entidad relacionada repetida.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 18: `        if (sin.InstanciasUnicas != sin.Filas)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException("4.3: AsNoTracking resolvio identidades cuando no debia.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 20: `        if (con.InstanciasUnicas != con.ClavesUnicas)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 21: `            throw new InvalidOperationException("4.3: AsNoTrackingWithIdentityResolution no deduplico por clave.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 23: `        Console.WriteLine($"AsNoTracking: filas={sin.Filas}, claves={sin.ClavesUnicas}, instancias={sin.InstanciasUnicas}");` → Publica evidencia observable en la consola.

Línea 24: `        Console.WriteLine($"IdentityResolution: filas={con.Filas}, claves={con.ClavesUnicas}, instancias={con.InstanciasUnicas}");` → Publica evidencia observable en la consola.

Línea 25: `    }` → Delimita el bloque sintáctico asociado.

Línea 26: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<IdentityResolutionUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.3 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 26: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 28: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 29: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 30: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 31: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 32: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 34: `var useCase = scope.ServiceProvider.GetRequiredService<IdentityResolutionUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 35: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 37: `Console.WriteLine("4.3 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.3 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Predecir cuántas instancias habrá cuando cuatro relaciones apunten a dos aleaciones distintas.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.3 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- AsNoTracking frente a AsNoTrackingWithIdentityResolution.
- Conteo por referencia usando ReferenceEqualityComparer.
- Escenario donde una misma clave aparece varias veces en el resultado.

### Reto de ampliación

Compara por referencia las instancias de una aleación compartida con y sin Identity Resolution.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No usar una entidad que nunca puede repetirse para demostrar resolución de identidad.
- No confundir igualdad de clave con igualdad de referencia.
- No dejar tracking previo activo durante la comparación.

### Analogía operativa

La resolución de identidad evita crear dos fichas físicas para la misma clave dentro de una consulta.

### Resultado esperado

Al finalizar el punto 4.3, el proyecto debe compilar, ejecutarse y terminar con 4.3 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.4 y continúa directamente desde este proyecto.

---

## Punto 4.4 — Problema N+1: identificación y causas

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.3. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

En AceriaData, Lazy Loading está desactivado. Para estudiar N+1 se provoca de forma explícita: una consulta para órdenes y una adicional por orden. Un interceptor cuenta los DbCommand reales.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.4
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento44.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `EjecutarNMasUnoM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public NMasUnoMetricaDto EjecutarNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();

        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, o.NumeroOrden })
            .ToList();

        var totalPlanchas = 0;
        foreach (var orden in ordenes)
        {
            totalPlanchas += _context.PlanchasAcero
                .AsNoTracking()
                .Count(p => p.OrdenId == orden.Id);
        }

        return new NMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = totalPlanchas
        };
    }
}
```

#### Explicación línea a línea — Rendimiento44.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public NMasUnoMetricaDto EjecutarNMasUnoM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 9: `    {` → Delimita el bloque sintáctico asociado.

Línea 10: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 12: `        var ordenes = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 13: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 14: `            .OrderBy(o => o.Id)` → Forma parte del orden determinista.

Línea 15: `            .Select(o => new { o.Id, o.NumeroOrden })` → Proyecta la forma de resultado y controla datos materializados.

Línea 16: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 18: `        var totalPlanchas = 0;` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `        foreach (var orden in ordenes)` → Recorre los elementos materializados para observar o validar cada resultado.

Línea 20: `        {` → Delimita el bloque sintáctico asociado.

Línea 21: `            totalPlanchas += _context.PlanchasAcero` → Continúa la implementación del punto con esta expresión: totalPlanchas += _context.PlanchasAcero

Línea 22: `                .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 23: `                .Count(p => p.OrdenId == orden.Id);` → Continúa la composición fluida invocando Count sobre el resultado de la línea anterior.

Línea 24: `        }` → Delimita el bloque sintáctico asociado.

Línea 26: `        return new NMasUnoMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 27: `        {` → Delimita el bloque sintáctico asociado.

Línea 28: `            Ordenes = ordenes.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 29: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 30: `            Planchas = totalPlanchas` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 31: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 32: `    }` → Delimita el bloque sintáctico asociado.

Línea 33: `}` → Delimita el bloque sintáctico asociado.


Archivo complementario: src/AceriaData.Infrastructure/SqlCommandCounterInterceptor.cs

```csharp
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AceriaData.Infrastructure;

public sealed class SqlCommandCounterInterceptor : DbCommandInterceptor
{
    public static SqlCommandCounterInterceptor Instance { get; } = new();

    private long _count;
    public long Count => Interlocked.Read(ref _count);

    public void Reset() => Interlocked.Exchange(ref _count, 0);

    private void Increment() => Interlocked.Increment(ref _count);

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Increment();
        return result;
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        Increment();
        return result;
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        Increment();
        return result;
    }
}
```

#### Explicación línea a línea — SqlCommandCounterInterceptor.cs

Línea 1: `using System.Data.Common;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore.Diagnostics;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed class SqlCommandCounterInterceptor : DbCommandInterceptor` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public static SqlCommandCounterInterceptor Instance { get; } = new();` → Mide comandos SQL reales ejecutados.

Línea 10: `    private long _count;` → Continúa la implementación del punto con esta expresión: private long _count;

Línea 11: `    public long Count => Interlocked.Read(ref _count);` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 13: `    public void Reset() => Interlocked.Exchange(ref _count, 0);` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 15: `    private void Increment() => Interlocked.Increment(ref _count);` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 17: `    public override InterceptionResult<DbDataReader> ReaderExecuting(` → Continúa la implementación del punto con esta expresión: public override InterceptionResult<DbDataReader> ReaderExecuting(

Línea 18: `        DbCommand command,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 19: `        CommandEventData eventData,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 20: `        InterceptionResult<DbDataReader> result)` → Continúa la implementación del punto con esta expresión: InterceptionResult<DbDataReader> result)

Línea 21: `    {` → Delimita el bloque sintáctico asociado.

Línea 22: `        Increment();` → Continúa la implementación del punto con esta expresión: Increment();

Línea 23: `        return result;` → Devuelve el resultado calculado al llamador.

Línea 24: `    }` → Delimita el bloque sintáctico asociado.

Línea 26: `    public override InterceptionResult<object> ScalarExecuting(` → Continúa la implementación del punto con esta expresión: public override InterceptionResult<object> ScalarExecuting(

Línea 27: `        DbCommand command,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 28: `        CommandEventData eventData,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 29: `        InterceptionResult<object> result)` → Continúa la implementación del punto con esta expresión: InterceptionResult<object> result)

Línea 30: `    {` → Delimita el bloque sintáctico asociado.

Línea 31: `        Increment();` → Continúa la implementación del punto con esta expresión: Increment();

Línea 32: `        return result;` → Devuelve el resultado calculado al llamador.

Línea 33: `    }` → Delimita el bloque sintáctico asociado.

Línea 35: `    public override InterceptionResult<int> NonQueryExecuting(` → Continúa la implementación del punto con esta expresión: public override InterceptionResult<int> NonQueryExecuting(

Línea 36: `        DbCommand command,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 37: `        CommandEventData eventData,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 38: `        InterceptionResult<int> result)` → Continúa la implementación del punto con esta expresión: InterceptionResult<int> result)

Línea 39: `    {` → Delimita el bloque sintáctico asociado.

Línea 40: `        Increment();` → Continúa la implementación del punto con esta expresión: Increment();

Línea 41: `        return result;` → Devuelve el resultado calculado al llamador.

Línea 42: `    }` → Delimita el bloque sintáctico asociado.

Línea 43: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class NMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");
        var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();

        if (metrica.ConsultasSql != metrica.Ordenes + 1)
            throw new InvalidOperationException(
                $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");

        Console.WriteLine(
            $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");
        Console.WriteLine(
            "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.");
    }
}
```

#### Explicación línea a línea — NMasUnoUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class NMasUnoUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public NMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.4 PROBLEMA N+1 ===");` → Publica evidencia observable en la consola.

Línea 13: `        var metrica = _unidad.Ordenes.EjecutarNMasUnoM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        if (metrica.ConsultasSql != metrica.Ordenes + 1)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 16: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 17: `                $"4.4: se esperaban N+1 consultas; obtenidas {metrica.ConsultasSql} para N={metrica.Ordenes}.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 19: `        Console.WriteLine(` → Publica evidencia observable en la consola.

Línea 20: `            $"Ordenes={metrica.Ordenes} | Planchas={metrica.Planchas} | Consultas SQL={metrica.ConsultasSql}");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 21: `        Console.WriteLine(` → Publica evidencia observable en la consola.

Línea 22: `            "La demostracion genera N+1 de forma explicita; Lazy Loading permanece desactivado.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 23: `    }` → Delimita el bloque sintáctico asociado.

Línea 24: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<NMasUnoUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.4 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 27: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 30: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 32: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 33: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<NMasUnoUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 36: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 38: `Console.WriteLine("4.4 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.4 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Calcular y después medir cuántos comandos se producen para N órdenes.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.4 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Identificación de N+1, sus causas y relación con navegaciones.
- Conteo real de comandos SQL y comparación con una alternativa sin N+1.
- Variantes conceptuales con Lazy Loading, consultas en bucle, FirstOrDefault y proyecciones.

### Reto de ampliación

Provoca N+1 al consultar detalle por orden y compáralo conceptualmente con una carga anticipada o proyección.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No asumir que acceder a una navegación ejecutará SQL cuando Lazy Loading está desactivado.
- No inferir N+1 por intuición: contar comandos reales.
- No mezclar estado previo del contexto en la medición.

### Analogía operativa

N+1 es pedir una lista y volver a la ventanilla una vez por cada elemento.

### Resultado esperado

Al finalizar el punto 4.4, el proyecto debe compilar, ejecutarse y terminar con 4.4 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.5 y continúa directamente desde este proyecto.

---

## Punto 4.5 — Solución a N+1: Include, proyecciones y Split Queries

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.4. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

No existe una solución universal al N+1. Include sirve para grafos; una proyección cuando solo se necesitan campos concretos; SplitQuery puede reducir explosión cartesiana con varias colecciones a costa de más roundtrips.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.5
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento45.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `EjecutarIncludeContraNMasUnoM4`, `EjecutarProyeccionContraNMasUnoM4`, `EjecutarSplitQueryContraNMasUnoM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public SolucionNMasUnoMetricaDto EjecutarIncludeContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .Include(o => o.Planchas)
            .OrderBy(o => o.Id)
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count)
        };
    }

    public SolucionNMasUnoMetricaDto EjecutarProyeccionContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Select(o => new { o.Id, TotalPlanchas = o.Planchas.Count })
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.TotalPlanchas)
        };
    }

    public SolucionNMasUnoMetricaDto EjecutarSplitQueryContraNMasUnoM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = _context.OrdenesFabricacion
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Planchas)
            .Include(o => o.OrdenesAleaciones)
                .ThenInclude(oa => oa.Aleacion)
            .AsSplitQuery()
            .OrderBy(o => o.Id)
            .ToList();

        return new SolucionNMasUnoMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count + o.OrdenesAleaciones.Count)
        };
    }
}
```

#### Explicación línea a línea — Rendimiento45.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public SolucionNMasUnoMetricaDto EjecutarIncludeContraNMasUnoM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 9: `    {` → Delimita el bloque sintáctico asociado.

Línea 10: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 11: `        var ordenes = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 12: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 13: `            .Include(o => o.Planchas)` → Define la navegación relacionada que debe cargarse.

Línea 14: `            .OrderBy(o => o.Id)` → Forma parte del orden determinista.

Línea 15: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 17: `        return new SolucionNMasUnoMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 18: `        {` → Delimita el bloque sintáctico asociado.

Línea 19: `            Ordenes = ordenes.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 20: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 21: `            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count)` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 22: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 23: `    }` → Delimita el bloque sintáctico asociado.

Línea 25: `    public SolucionNMasUnoMetricaDto EjecutarProyeccionContraNMasUnoM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 26: `    {` → Delimita el bloque sintáctico asociado.

Línea 27: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 28: `        var ordenes = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 29: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 30: `            .OrderBy(o => o.Id)` → Forma parte del orden determinista.

Línea 31: `            .Select(o => new { o.Id, TotalPlanchas = o.Planchas.Count })` → Proyecta la forma de resultado y controla datos materializados.

Línea 32: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 34: `        return new SolucionNMasUnoMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 35: `        {` → Delimita el bloque sintáctico asociado.

Línea 36: `            Ordenes = ordenes.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 37: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 38: `            ElementosRelacionados = ordenes.Sum(o => o.TotalPlanchas)` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 39: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 40: `    }` → Delimita el bloque sintáctico asociado.

Línea 42: `    public SolucionNMasUnoMetricaDto EjecutarSplitQueryContraNMasUnoM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 43: `    {` → Delimita el bloque sintáctico asociado.

Línea 44: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 45: `        var ordenes = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 46: `            .AsNoTrackingWithIdentityResolution()` → Activa NoTracking con resolución temporal de identidad.

Línea 47: `            .Include(o => o.Planchas)` → Define la navegación relacionada que debe cargarse.

Línea 48: `            .Include(o => o.OrdenesAleaciones)` → Define la navegación relacionada que debe cargarse.

Línea 49: `                .ThenInclude(oa => oa.Aleacion)` → Define la navegación relacionada que debe cargarse.

Línea 50: `            .AsSplitQuery()` → Divide la carga relacionada en varios comandos SQL.

Línea 51: `            .OrderBy(o => o.Id)` → Forma parte del orden determinista.

Línea 52: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 54: `        return new SolucionNMasUnoMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 55: `        {` → Delimita el bloque sintáctico asociado.

Línea 56: `            Ordenes = ordenes.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 57: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 58: `            ElementosRelacionados = ordenes.Sum(o => o.Planchas.Count + o.OrdenesAleaciones.Count)` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 59: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 60: `    }` → Delimita el bloque sintáctico asociado.

Línea 61: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SolucionesNMasUnoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");
        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();
        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();
        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();

        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)
            throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");

        Console.WriteLine($"Include: {include.ConsultasSql} consulta.");
        Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");
    }
}
```

#### Explicación línea a línea — SolucionesNMasUnoUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class SolucionesNMasUnoUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public SolucionesNMasUnoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.5 SOLUCIONES AL N+1 ===");` → Publica evidencia observable en la consola.

Línea 13: `        var include = _unidad.Ordenes.EjecutarIncludeContraNMasUnoM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var proyeccion = _unidad.Ordenes.EjecutarProyeccionContraNMasUnoM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        var split = _unidad.Ordenes.EjecutarSplitQueryContraNMasUnoM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 17: `        if (include.ConsultasSql != 1 || proyeccion.ConsultasSql != 1)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 18: `            throw new InvalidOperationException("4.5: Include/proyeccion no redujeron la carga a una consulta.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 19: `        if (split.ConsultasSql != 3)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 20: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 21: `                $"4.5: se esperaban 3 consultas en SplitQuery con dos colecciones; obtenidas {split.ConsultasSql}.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 23: `        Console.WriteLine($"Include: {include.ConsultasSql} consulta.");` → Publica evidencia observable en la consola.

Línea 24: `        Console.WriteLine($"Proyeccion: {proyeccion.ConsultasSql} consulta.");` → Publica evidencia observable en la consola.

Línea 25: `        Console.WriteLine($"SplitQuery: {split.ConsultasSql} consultas para evitar explosion cartesiana con dos colecciones.");` → Publica evidencia observable en la consola.

Línea 26: `    }` → Delimita el bloque sintáctico asociado.

Línea 27: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<SolucionesNMasUnoUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.5 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 28: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 30: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 31: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 32: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 33: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 34: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 36: `var useCase = scope.ServiceProvider.GetRequiredService<SolucionesNMasUnoUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 37: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 39: `Console.WriteLine("4.5 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.5 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Elegir entre Include, proyección o SplitQuery para tres escenarios y justificar el coste dominante.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.5 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Include y ThenInclude para cargar grafos.
- Proyecciones para obtener solo los datos necesarios.
- AsSplitQuery como alternativa cuando existen varias colecciones.

### Reto de ampliación

Combina Include, ThenInclude, Identity Resolution y SplitQuery en un grafo con planchas y aleaciones y justifica el número de comandos.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No aplicar SplitQuery por defecto sin observar la forma del grafo.
- No comparar tiempos sin aislar tracking y dataset.
- No confundir evitar N+1 con garantizar una única consulta.

### Analogía operativa

Optimizar N+1 es decidir si conviene traer el expediente completo, un resumen o varios lotes coordinados.

### Resultado esperado

Al finalizar el punto 4.5, el proyecto debe compilar, ejecutarse y terminar con 4.5 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.6 y continúa directamente desde este proyecto.

---

## Punto 4.6 — Over-fetching: causas y soluciones

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.5. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

Over-fetching se diagnostica observando la forma real del SELECT. La práctica compara igual cardinalidad con entidad completa frente a proyección DTO.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.6
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento46.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `ObtenerPendientesEntidadCompletaM4`, `ObtenerPendientesProyectadasM4`, `ObtenerSqlPendientesEntidadCompletaM4`, `ObtenerSqlPendientesProyectadasM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private IQueryable<OrdenFabricacion> PendientesM4() => _context.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.Estado == "Pendiente")
        .OrderBy(o => o.FechaCreacion)
        .ThenBy(o => o.Id);

    public List<OrdenFabricacion> ObtenerPendientesEntidadCompletaM4() =>
        PendientesM4().ToList();

    public List<OrdenResumenDto> ObtenerPendientesProyectadasM4() =>
        PendientesM4()
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .ToList();

    public string ObtenerSqlPendientesEntidadCompletaM4() =>
        PendientesM4().ToQueryString();

    public string ObtenerSqlPendientesProyectadasM4() =>
        PendientesM4()
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .ToQueryString();
}
```

#### Explicación línea a línea — Rendimiento46.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Domain.Entities;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 7: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 8: `{` → Delimita el bloque sintáctico asociado.

Línea 9: `    private IQueryable<OrdenFabricacion> PendientesM4() => _context.OrdenesFabricacion` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 10: `        .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 11: `        .Where(o => o.Estado == "Pendiente")` → Añade el predicado de filtrado a la forma de consulta.

Línea 12: `        .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 13: `        .ThenBy(o => o.Id);` → Forma parte del orden determinista.

Línea 15: `    public List<OrdenFabricacion> ObtenerPendientesEntidadCompletaM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 16: `        PendientesM4().ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 18: `    public List<OrdenResumenDto> ObtenerPendientesProyectadasM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 19: `        PendientesM4()` → Continúa la implementación del punto con esta expresión: PendientesM4()

Línea 20: `            .Select(o => new OrdenResumenDto` → Proyecta la forma de resultado y controla datos materializados.

Línea 21: `            {` → Delimita el bloque sintáctico asociado.

Línea 22: `                NumeroOrden = o.NumeroOrden,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 23: `                Cliente = o.Cliente,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 24: `                Estado = o.Estado,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 25: `                FechaCreacion = o.FechaCreacion` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 26: `            })` → Continúa la implementación del punto con esta expresión: })

Línea 27: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 29: `    public string ObtenerSqlPendientesEntidadCompletaM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 30: `        PendientesM4().ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 32: `    public string ObtenerSqlPendientesProyectadasM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 33: `        PendientesM4()` → Continúa la implementación del punto con esta expresión: PendientesM4()

Línea 34: `            .Select(o => new OrdenResumenDto` → Proyecta la forma de resultado y controla datos materializados.

Línea 35: `            {` → Delimita el bloque sintáctico asociado.

Línea 36: `                NumeroOrden = o.NumeroOrden,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 37: `                Cliente = o.Cliente,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 38: `                Estado = o.Estado,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 39: `                FechaCreacion = o.FechaCreacion` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 40: `            })` → Continúa la implementación del punto con esta expresión: })

Línea 41: `            .ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 42: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class OverFetchingUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public OverFetchingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.6 OVER-FETCHING ===");
        var completas = _unidad.Ordenes.ObtenerPendientesEntidadCompletaM4();
        var proyectadas = _unidad.Ordenes.ObtenerPendientesProyectadasM4();
        var sqlCompleto = _unidad.Ordenes.ObtenerSqlPendientesEntidadCompletaM4();
        var sqlProyectado = _unidad.Ordenes.ObtenerSqlPendientesProyectadasM4();

        if (completas.Count != proyectadas.Count)
            throw new InvalidOperationException("4.6: la proyeccion cambio la cardinalidad.");
        if (!sqlCompleto.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.6: el SQL completo no evidencia columnas extra.");
        if (sqlProyectado.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.6: la proyeccion sigue recuperando Observaciones.");

        Console.WriteLine($"Filas equivalentes: {completas.Count}");
        Console.WriteLine("--- SQL entidad completa ---");
        Console.WriteLine(sqlCompleto);
        Console.WriteLine("--- SQL proyeccion ---");
        Console.WriteLine(sqlProyectado);
    }
}
```

#### Explicación línea a línea — OverFetchingUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class OverFetchingUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public OverFetchingUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.6 OVER-FETCHING ===");` → Publica evidencia observable en la consola.

Línea 13: `        var completas = _unidad.Ordenes.ObtenerPendientesEntidadCompletaM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var proyectadas = _unidad.Ordenes.ObtenerPendientesProyectadasM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        var sqlCompleto = _unidad.Ordenes.ObtenerSqlPendientesEntidadCompletaM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        var sqlProyectado = _unidad.Ordenes.ObtenerSqlPendientesProyectadasM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 18: `        if (completas.Count != proyectadas.Count)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException("4.6: la proyeccion cambio la cardinalidad.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 20: `        if (!sqlCompleto.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 21: `            throw new InvalidOperationException("4.6: el SQL completo no evidencia columnas extra.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 22: `        if (sqlProyectado.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 23: `            throw new InvalidOperationException("4.6: la proyeccion sigue recuperando Observaciones.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 25: `        Console.WriteLine($"Filas equivalentes: {completas.Count}");` → Publica evidencia observable en la consola.

Línea 26: `        Console.WriteLine("--- SQL entidad completa ---");` → Publica evidencia observable en la consola.

Línea 27: `        Console.WriteLine(sqlCompleto);` → Publica evidencia observable en la consola.

Línea 28: `        Console.WriteLine("--- SQL proyeccion ---");` → Publica evidencia observable en la consola.

Línea 29: `        Console.WriteLine(sqlProyectado);` → Publica evidencia observable en la consola.

Línea 30: `    }` → Delimita el bloque sintáctico asociado.

Línea 31: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<OverFetchingUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.6 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 28: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 29: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 31: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 32: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 33: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 34: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 35: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 37: `var useCase = scope.ServiceProvider.GetRequiredService<OverFetchingUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 38: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 40: `Console.WriteLine("4.6 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.6 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Identificar en el SQL qué columnas desaparecen al proyectar y relacionarlo con transferencia y materialización.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.6 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Over-fetching de columnas y de filas.
- Proyecciones, filtros y paginación para reducir datos transferidos.
- Inspección del SQL para comparar entidad completa frente a shape reducido.

### Reto de ampliación

Compara el SELECT de entidad completa y proyección y relaciona las columnas eliminadas con transferencia y materialización.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No aplicar Skip sin un OrderBy determinista.
- No materializar antes de terminar filtros y proyecciones.
- No medir solo tiempo cuando el objetivo es demostrar volumen de datos.

### Analogía operativa

Over-fetching es mover un palé entero cuando la siguiente estación solo necesita cuatro piezas.

### Resultado esperado

Al finalizar el punto 4.6, el proyecto debe compilar, ejecutarse y terminar con 4.6 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.7 y continúa directamente desde este proyecto.

---

## Punto 4.7 — Consultas ineficientes: traducción y frontera cliente/servidor

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.6. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

En EF Core 8 un predicado no traducible dentro de Where no se evalúa silenciosamente en cliente: falla. La evaluación cliente exige una frontera explícita como AsEnumerable. Las funciones sobre columnas pueden perjudicar sargabilidad y deben medirse.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.7
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento47.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `ContarConEvaluacionClienteExplicitaM4`, `FiltroPersonalizadoNoTraducibleFallaM4`, `ObtenerSqlClienteConFuncionM4`, `ObtenerSqlClienteDirectoM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private static bool EstadoCoincideM4(string actual, string buscado) =>
        string.Equals(actual, buscado, StringComparison.OrdinalIgnoreCase);

    public bool FiltroPersonalizadoNoTraducibleFallaM4(string estado)
    {
        try
        {
            _ = _context.OrdenesFabricacion
                .AsNoTracking()
                .Where(o => EstadoCoincideM4(o.Estado, estado))
                .ToList();
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    public int ContarConEvaluacionClienteExplicitaM4(string estado) =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .AsEnumerable()
            .Count(o => EstadoCoincideM4(o.Estado, estado));

    public string ObtenerSqlClienteConFuncionM4(string cliente)
    {
        var normalizado = cliente.ToLowerInvariant();
        return _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Cliente.ToLower() == normalizado)
            .ToQueryString();
    }

    public string ObtenerSqlClienteDirectoM4(string cliente) =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Cliente == cliente)
            .ToQueryString();
}
```

#### Explicación línea a línea — Rendimiento47.cs

Línea 1: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private static bool EstadoCoincideM4(string actual, string buscado) =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 8: `        string.Equals(actual, buscado, StringComparison.OrdinalIgnoreCase);` → Continúa la implementación del punto con esta expresión: string.Equals(actual, buscado, StringComparison.OrdinalIgnoreCase);

Línea 10: `    public bool FiltroPersonalizadoNoTraducibleFallaM4(string estado)` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        try` → Abre el bloque protegido cuya excepción forma parte de la evidencia del escenario.

Línea 13: `        {` → Delimita el bloque sintáctico asociado.

Línea 14: `            _ = _context.OrdenesFabricacion` → Fuerza la ejecución y descarta el valor porque en este bloque interesa medir el coste de la operación.

Línea 15: `                .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 16: `                .Where(o => EstadoCoincideM4(o.Estado, estado))` → Añade el predicado de filtrado a la forma de consulta.

Línea 17: `                .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 18: `            return false;` → Devuelve el resultado calculado al llamador.

Línea 19: `        }` → Delimita el bloque sintáctico asociado.

Línea 20: `        catch (InvalidOperationException)` → Captura explícitamente la excepción esperada para distinguir el fallo de traducción.

Línea 21: `        {` → Delimita el bloque sintáctico asociado.

Línea 22: `            return true;` → Devuelve el resultado calculado al llamador.

Línea 23: `        }` → Delimita el bloque sintáctico asociado.

Línea 24: `    }` → Delimita el bloque sintáctico asociado.

Línea 26: `    public int ContarConEvaluacionClienteExplicitaM4(string estado) =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 27: `        _context.OrdenesFabricacion` → Continúa la implementación del punto con esta expresión: _context.OrdenesFabricacion

Línea 28: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 29: `            .AsEnumerable()` → Establece explícitamente la frontera hacia LINQ to Objects.

Línea 30: `            .Count(o => EstadoCoincideM4(o.Estado, estado));` → Continúa la composición fluida invocando Count sobre el resultado de la línea anterior.

Línea 32: `    public string ObtenerSqlClienteConFuncionM4(string cliente)` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 33: `    {` → Delimita el bloque sintáctico asociado.

Línea 34: `        var normalizado = cliente.ToLowerInvariant();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 35: `        return _context.OrdenesFabricacion` → Devuelve el resultado calculado al llamador.

Línea 36: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 37: `            .Where(o => o.Cliente.ToLower() == normalizado)` → Añade el predicado de filtrado a la forma de consulta.

Línea 38: `            .ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 39: `    }` → Delimita el bloque sintáctico asociado.

Línea 41: `    public string ObtenerSqlClienteDirectoM4(string cliente) =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 42: `        _context.OrdenesFabricacion` → Continúa la implementación del punto con esta expresión: _context.OrdenesFabricacion

Línea 43: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 44: `            .Where(o => o.Cliente == cliente)` → Añade el predicado de filtrado a la forma de consulta.

Línea 45: `            .ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 46: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class TraduccionConsultasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public TraduccionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===");

        if (!_unidad.Ordenes.FiltroPersonalizadoNoTraducibleFallaM4("Pendiente"))
            throw new InvalidOperationException("4.7: EF Core no rechazo el filtro personalizado no traducible.");

        var cliente = _unidad.Ordenes.ContarConEvaluacionClienteExplicitaM4("Pendiente");
        if (cliente <= 0)
            throw new InvalidOperationException("4.7: la evaluacion cliente explicita no devolvio datos.");

        var sqlFuncion = _unidad.Ordenes.ObtenerSqlClienteConFuncionM4("Constructora del Norte");
        var sqlDirecto = _unidad.Ordenes.ObtenerSqlClienteDirectoM4("Constructora del Norte");

        if (!sqlFuncion.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.7: no se observa LOWER en el SQL con funcion.");
        if (sqlDirecto.Contains("LOWER", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("4.7: la comparacion directa introdujo LOWER inesperadamente.");

        Console.WriteLine("Filtro no traducible: InvalidOperationException observada.");
        Console.WriteLine($"Evaluacion cliente explicita: {cliente} filas coincidentes.");
        Console.WriteLine("--- SQL con funcion sobre columna ---");
        Console.WriteLine(sqlFuncion);
        Console.WriteLine("--- SQL con comparacion directa ---");
        Console.WriteLine(sqlDirecto);
    }
}
```

#### Explicación línea a línea — TraduccionConsultasUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class TraduccionConsultasUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public TraduccionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.7 TRADUCCION Y FRONTERA CLIENTE/SERVIDOR ===");` → Publica evidencia observable en la consola.

Línea 14: `        if (!_unidad.Ordenes.FiltroPersonalizadoNoTraducibleFallaM4("Pendiente"))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 15: `            throw new InvalidOperationException("4.7: EF Core no rechazo el filtro personalizado no traducible.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 17: `        var cliente = _unidad.Ordenes.ContarConEvaluacionClienteExplicitaM4("Pendiente");` → Calcula y conserva el resultado que será validado o mostrado.

Línea 18: `        if (cliente <= 0)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException("4.7: la evaluacion cliente explicita no devolvio datos.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 21: `        var sqlFuncion = _unidad.Ordenes.ObtenerSqlClienteConFuncionM4("Constructora del Norte");` → Calcula y conserva el resultado que será validado o mostrado.

Línea 22: `        var sqlDirecto = _unidad.Ordenes.ObtenerSqlClienteDirectoM4("Constructora del Norte");` → Calcula y conserva el resultado que será validado o mostrado.

Línea 24: `        if (!sqlFuncion.Contains("LOWER", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 25: `            throw new InvalidOperationException("4.7: no se observa LOWER en el SQL con funcion.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 26: `        if (sqlDirecto.Contains("LOWER", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 27: `            throw new InvalidOperationException("4.7: la comparacion directa introdujo LOWER inesperadamente.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 29: `        Console.WriteLine("Filtro no traducible: InvalidOperationException observada.");` → Publica evidencia observable en la consola.

Línea 30: `        Console.WriteLine($"Evaluacion cliente explicita: {cliente} filas coincidentes.");` → Publica evidencia observable en la consola.

Línea 31: `        Console.WriteLine("--- SQL con funcion sobre columna ---");` → Publica evidencia observable en la consola.

Línea 32: `        Console.WriteLine(sqlFuncion);` → Publica evidencia observable en la consola.

Línea 33: `        Console.WriteLine("--- SQL con comparacion directa ---");` → Publica evidencia observable en la consola.

Línea 34: `        Console.WriteLine(sqlDirecto);` → Publica evidencia observable en la consola.

Línea 35: `    }` → Delimita el bloque sintáctico asociado.

Línea 36: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<TraduccionConsultasUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.7 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `services.AddScoped<TraduccionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 29: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 30: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 32: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 33: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 34: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 35: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 36: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 38: `var useCase = scope.ServiceProvider.GetRequiredService<TraduccionConsultasUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 39: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 41: `Console.WriteLine("4.7 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.7 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Comparar el SQL con LOWER(columna) frente a comparación directa y explicar qué debe medirse en SQL Server.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.7 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Filtros no traducibles y frontera cliente/servidor.
- Funciones aplicadas a columnas y posible pérdida de sargabilidad.
- Reescritura de expresiones y uso de collation cuando corresponda.

### Reto de ampliación

Reescribe una validación de formato para usar operaciones traducibles y explica qué parte debe seguir ejecutándose en SQL.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No afirmar que un Where no traducible se ejecuta automáticamente en memoria.
- No aplicar ToLower a la columna sin analizar el impacto sobre el índice.
- No ocultar una frontera cliente implícita: hacerla explícita.

### Analogía operativa

Una frontera cliente explícita es sacar las piezas de la máquina y continuar manualmente: se puede hacer, pero debe ser consciente.

### Resultado esperado

Al finalizar el punto 4.7, el proyecto debe compilar, ejecutarse y terminar con 4.7 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.8 y continúa directamente desde este proyecto.

---

## Punto 4.8 — Split Queries: cuándo y cómo usarlas

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.7. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

SplitQuery ejecuta varios comandos y puede evitar explosión cartesiana. No implica una transacción independiente por subconsulta. Sin aislamiento adecuado puede no existir una instantánea consistente frente a cambios concurrentes.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.8
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento48.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `MedirSingleQueryM4`, `MedirSplitQueryM4`, `ObtenerSqlSingleQueryM4`, `ObtenerSqlSplitQueryM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using AceriaData.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private IQueryable<OrdenFabricacion> ConsultaDosColeccionesM4() =>
        _context.OrdenesFabricacion
            .AsNoTrackingWithIdentityResolution()
            .Include(o => o.Planchas)
            .Include(o => o.OrdenesAleaciones)
                .ThenInclude(oa => oa.Aleacion)
            .OrderBy(o => o.Id);

    public SplitQueryMetricaDto MedirSingleQueryM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().AsSingleQuery().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }

    public SplitQueryMetricaDto MedirSplitQueryM4()
    {
        SqlCommandCounterInterceptor.Instance.Reset();
        var ordenes = ConsultaDosColeccionesM4().AsSplitQuery().ToList();

        return new SplitQueryMetricaDto
        {
            Ordenes = ordenes.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            Planchas = ordenes.Sum(o => o.Planchas.Count),
            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)
        };
    }

    public string ObtenerSqlSingleQueryM4() =>
        ConsultaDosColeccionesM4().AsSingleQuery().ToQueryString();

    public string ObtenerSqlSplitQueryM4() =>
        ConsultaDosColeccionesM4().AsSplitQuery().ToQueryString();
}
```

#### Explicación línea a línea — Rendimiento48.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Domain.Entities;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 7: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 8: `{` → Delimita el bloque sintáctico asociado.

Línea 9: `    private IQueryable<OrdenFabricacion> ConsultaDosColeccionesM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 10: `        _context.OrdenesFabricacion` → Continúa la implementación del punto con esta expresión: _context.OrdenesFabricacion

Línea 11: `            .AsNoTrackingWithIdentityResolution()` → Activa NoTracking con resolución temporal de identidad.

Línea 12: `            .Include(o => o.Planchas)` → Define la navegación relacionada que debe cargarse.

Línea 13: `            .Include(o => o.OrdenesAleaciones)` → Define la navegación relacionada que debe cargarse.

Línea 14: `                .ThenInclude(oa => oa.Aleacion)` → Define la navegación relacionada que debe cargarse.

Línea 15: `            .OrderBy(o => o.Id);` → Forma parte del orden determinista.

Línea 17: `    public SplitQueryMetricaDto MedirSingleQueryM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 18: `    {` → Delimita el bloque sintáctico asociado.

Línea 19: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 20: `        var ordenes = ConsultaDosColeccionesM4().AsSingleQuery().ToList();` → Fuerza un único comando para la comparación.

Línea 22: `        return new SplitQueryMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 23: `        {` → Delimita el bloque sintáctico asociado.

Línea 24: `            Ordenes = ordenes.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 25: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 26: `            Planchas = ordenes.Sum(o => o.Planchas.Count),` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 27: `            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 28: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 29: `    }` → Delimita el bloque sintáctico asociado.

Línea 31: `    public SplitQueryMetricaDto MedirSplitQueryM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 32: `    {` → Delimita el bloque sintáctico asociado.

Línea 33: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 34: `        var ordenes = ConsultaDosColeccionesM4().AsSplitQuery().ToList();` → Divide la carga relacionada en varios comandos SQL.

Línea 36: `        return new SplitQueryMetricaDto` → Devuelve el resultado calculado al llamador.

Línea 37: `        {` → Delimita el bloque sintáctico asociado.

Línea 38: `            Ordenes = ordenes.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 39: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 40: `            Planchas = ordenes.Sum(o => o.Planchas.Count),` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 41: `            RelacionesAleacion = ordenes.Sum(o => o.OrdenesAleaciones.Count)` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 42: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 43: `    }` → Delimita el bloque sintáctico asociado.

Línea 45: `    public string ObtenerSqlSingleQueryM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 46: `        ConsultaDosColeccionesM4().AsSingleQuery().ToQueryString();` → Fuerza un único comando para la comparación.

Línea 48: `    public string ObtenerSqlSplitQueryM4() =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 49: `        ConsultaDosColeccionesM4().AsSplitQuery().ToQueryString();` → Divide la carga relacionada en varios comandos SQL.

Línea 50: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class SplitQueriesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");
        var single = _unidad.Ordenes.MedirSingleQueryM4();
        var split = _unidad.Ordenes.MedirSplitQueryM4();

        if (single.ConsultasSql != 1)
            throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");
        if (split.ConsultasSql != 3)
            throw new InvalidOperationException(
                $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");
        if (single.Ordenes != split.Ordenes ||
            single.Planchas != split.Planchas ||
            single.RelacionesAleacion != split.RelacionesAleacion)
            throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");

        Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");
        Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");
        Console.WriteLine("--- ToQueryString SingleQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());
        Console.WriteLine("--- ToQueryString SplitQuery ---");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());
    }
}
```

#### Explicación línea a línea — SplitQueriesUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class SplitQueriesUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public SplitQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.8 SINGLE QUERY VS SPLIT QUERY ===");` → Publica evidencia observable en la consola.

Línea 13: `        var single = _unidad.Ordenes.MedirSingleQueryM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var split = _unidad.Ordenes.MedirSplitQueryM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        if (single.ConsultasSql != 1)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 17: `            throw new InvalidOperationException($"4.8: SingleQuery ejecuto {single.ConsultasSql} comandos.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 18: `        if (split.ConsultasSql != 3)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 20: `                $"4.8: SplitQuery ejecuto {split.ConsultasSql} comandos; se esperaban 3.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 21: `        if (single.Ordenes != split.Ordenes ||` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 22: `            single.Planchas != split.Planchas ||` → Continúa una condición compuesta usada para validar la equivalencia del resultado.

Línea 23: `            single.RelacionesAleacion != split.RelacionesAleacion)` → Continúa la implementación del punto con esta expresión: single.RelacionesAleacion != split.RelacionesAleacion)

Línea 24: `            throw new InvalidOperationException("4.8: SingleQuery y SplitQuery no materializaron el mismo grafo.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 26: `        Console.WriteLine($"SingleQuery: {single.ConsultasSql} comando SQL.");` → Publica evidencia observable en la consola.

Línea 27: `        Console.WriteLine($"SplitQuery: {split.ConsultasSql} comandos SQL.");` → Publica evidencia observable en la consola.

Línea 28: `        Console.WriteLine("--- ToQueryString SingleQuery ---");` → Publica evidencia observable en la consola.

Línea 29: `        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSingleQueryM4());` → Publica evidencia observable en la consola.

Línea 30: `        Console.WriteLine("--- ToQueryString SplitQuery ---");` → Publica evidencia observable en la consola.

Línea 31: `        Console.WriteLine(_unidad.Ordenes.ObtenerSqlSplitQueryM4());` → Publica evidencia observable en la consola.

Línea 32: `    }` → Delimita el bloque sintáctico asociado.

Línea 33: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();
services.AddScoped<SplitQueriesUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<SplitQueriesUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.8 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `services.AddScoped<TraduccionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 28: `services.AddScoped<SplitQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 30: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 31: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 33: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 34: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 35: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 36: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 37: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 39: `var useCase = scope.ServiceProvider.GetRequiredService<SplitQueriesUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 40: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 42: `Console.WriteLine("4.8 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.8 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Explicar por qué dos colecciones multiplican filas en SingleQuery y por qué SplitQuery intercambia volumen por roundtrips.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.8 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- AsSingleQuery frente a AsSplitQuery con varias colecciones.
- Explosión cartesiana, duplicación de datos y roundtrips.
- Coherencia entre varios comandos y configuración global de Split Queries.

### Reto de ampliación

Analiza cómo cambiaría el comportamiento si SplitQuery fuera global y qué advertencias querrías convertir en señal de diagnóstico.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No afirmar que cada subconsulta de SplitQuery crea su propia transacción.
- No afirmar que una sola colección nunca puede beneficiarse; evaluar volumen y roundtrips.
- No comparar Single/Split con grafos distintos.

### Analogía operativa

SingleQuery mezcla lotes en una hoja grande; SplitQuery los trae por separado y los ensambla por claves.

### Resultado esperado

Al finalizar el punto 4.8, el proyecto debe compilar, ejecutarse y terminar con 4.8 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.9 y continúa directamente desde este proyecto.

---

## Punto 4.9 — Compiled Queries

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.8. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

EF Core ya cachea consultas por forma. EF.CompileQuery evita parte del trabajo de búsqueda y preparación de EF; no almacena el plan de ejecución de SQL Server. Debe medirse en hot paths y no se exige ganar una microprueba aislada.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.9
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento49.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `ObtenerPorEstadoCompiladoM4`, `ObtenerPorEstadoNormalM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Domain.Entities;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    private static readonly Func<AceriaDbContext, string, IEnumerable<OrdenFabricacion>>
        ConsultaCompiladaPorEstadoM4 =
            EF.CompileQuery(
                (AceriaDbContext context, string estado) =>
                    context.OrdenesFabricacion
                        .AsNoTracking()
                        .Where(o => o.Estado == estado)
                        .OrderBy(o => o.FechaCreacion)
                        .ThenBy(o => o.Id)
                        .Select(o => o));

    public List<OrdenFabricacion> ObtenerPorEstadoNormalM4(string estado) =>
        _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => o.Estado == estado)
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .ToList();

    public List<OrdenFabricacion> ObtenerPorEstadoCompiladoM4(string estado) =>
        ConsultaCompiladaPorEstadoM4(_context, estado).ToList();
}
```

#### Explicación línea a línea — Rendimiento49.cs

Línea 1: `using AceriaData.Domain.Entities;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 7: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 8: `{` → Delimita el bloque sintáctico asociado.

Línea 9: `    private static readonly Func<AceriaDbContext, string, IEnumerable<OrdenFabricacion>>` → Declara un campo de solo lectura que conserva una dependencia o delegado reutilizable.

Línea 10: `        ConsultaCompiladaPorEstadoM4 =` → Continúa la implementación del punto con esta expresión: ConsultaCompiladaPorEstadoM4 =

Línea 11: `            EF.CompileQuery(` → Prepara un delegado de compiled query de EF.

Línea 12: `                (AceriaDbContext context, string estado) =>` → Define la expresión lambda que EF Core o el caso de uso empleará en esta operación.

Línea 13: `                    context.OrdenesFabricacion` → Continúa la implementación del punto con esta expresión: context.OrdenesFabricacion

Línea 14: `                        .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 15: `                        .Where(o => o.Estado == estado)` → Añade el predicado de filtrado a la forma de consulta.

Línea 16: `                        .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 17: `                        .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 18: `                        .Select(o => o));` → Proyecta la forma de resultado y controla datos materializados.

Línea 20: `    public List<OrdenFabricacion> ObtenerPorEstadoNormalM4(string estado) =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 21: `        _context.OrdenesFabricacion` → Continúa la implementación del punto con esta expresión: _context.OrdenesFabricacion

Línea 22: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 23: `            .Where(o => o.Estado == estado)` → Añade el predicado de filtrado a la forma de consulta.

Línea 24: `            .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 25: `            .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 26: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 28: `    public List<OrdenFabricacion> ObtenerPorEstadoCompiladoM4(string estado) =>` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 29: `        ConsultaCompiladaPorEstadoM4(_context, estado).ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 30: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using System.Diagnostics;
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CompiledQueriesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CompiledQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.9 COMPILED QUERIES ===");

        var normal = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");
        var compilada = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");

        if (!normal.Select(o => o.Id).SequenceEqual(compilada.Select(o => o.Id)))
            throw new InvalidOperationException("4.9: consulta normal y compilada no son equivalentes.");

        var swNormal = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
            _ = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");
        swNormal.Stop();

        var swCompilada = Stopwatch.StartNew();
        for (var i = 0; i < 20; i++)
            _ = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");
        swCompilada.Stop();

        Console.WriteLine($"Normal: {swNormal.ElapsedTicks} ticks | Compilada: {swCompilada.ElapsedTicks} ticks");
        Console.WriteLine(
            "Medicion observacional: no se exige que la compiled query gane en un dataset pequeno; se valida equivalencia y reutilizacion.");
    }
}
```

#### Explicación línea a línea — CompiledQueriesUseCase.cs

Línea 1: `using System.Diagnostics;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed class CompiledQueriesUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 9: `    public CompiledQueriesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 11: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 12: `    {` → Delimita el bloque sintáctico asociado.

Línea 13: `        Console.WriteLine("=== 4.9 COMPILED QUERIES ===");` → Publica evidencia observable en la consola.

Línea 15: `        var normal = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        var compilada = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");` → Calcula y conserva el resultado que será validado o mostrado.

Línea 18: `        if (!normal.Select(o => o.Id).SequenceEqual(compilada.Select(o => o.Id)))` → Proyecta la forma de resultado y controla datos materializados.

Línea 19: `            throw new InvalidOperationException("4.9: consulta normal y compilada no son equivalentes.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 21: `        var swNormal = Stopwatch.StartNew();` → Participa en la medición temporal observacional.

Línea 22: `        for (var i = 0; i < 20; i++)` → Repite la operación para obtener una medición observacional sobre varias ejecuciones.

Línea 23: `            _ = _unidad.Ordenes.ObtenerPorEstadoNormalM4("Pendiente");` → Fuerza la ejecución y descarta el valor porque en este bloque interesa medir el coste de la operación.

Línea 24: `        swNormal.Stop();` → Detiene el cronómetro inmediatamente después del bloque que se está midiendo.

Línea 26: `        var swCompilada = Stopwatch.StartNew();` → Participa en la medición temporal observacional.

Línea 27: `        for (var i = 0; i < 20; i++)` → Repite la operación para obtener una medición observacional sobre varias ejecuciones.

Línea 28: `            _ = _unidad.Ordenes.ObtenerPorEstadoCompiladoM4("Pendiente");` → Fuerza la ejecución y descarta el valor porque en este bloque interesa medir el coste de la operación.

Línea 29: `        swCompilada.Stop();` → Detiene el cronómetro inmediatamente después del bloque que se está midiendo.

Línea 31: `        Console.WriteLine($"Normal: {swNormal.ElapsedTicks} ticks | Compilada: {swCompilada.ElapsedTicks} ticks");` → Publica evidencia observable en la consola.

Línea 32: `        Console.WriteLine(` → Publica evidencia observable en la consola.

Línea 33: `            "Medicion observacional: no se exige que la compiled query gane en un dataset pequeno; se valida equivalencia y reutilizacion.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 34: `    }` → Delimita el bloque sintáctico asociado.

Línea 35: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();
services.AddScoped<SplitQueriesUseCase>();
services.AddScoped<CompiledQueriesUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<CompiledQueriesUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.9 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `services.AddScoped<TraduccionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 28: `services.AddScoped<SplitQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 29: `services.AddScoped<CompiledQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 31: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 32: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 34: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 35: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 36: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 37: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 38: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 40: `var useCase = scope.ServiceProvider.GetRequiredService<CompiledQueriesUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 41: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 43: `Console.WriteLine("4.9 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.9 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Justificar cuándo el coste evitado por CompileQuery puede importar frente a red y base de datos.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.9 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- EF.CompileQuery y EF.CompileAsyncQuery, parámetros y proyecciones.
- Caché interna de consultas de EF Core y coste que realmente evita una compiled query.
- Medición en hot paths sin prometer una mejora universal.

### Reto de ampliación

Diseña una compiled query proyectada y explica qué coste de EF evita frente al coste de red y SQL Server.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No compilar el delegado en cada llamada.
- No afirmar que EF.CompileQuery almacena el plan de ejecución de SQL Server.
- No usar un umbral de tiempo como condición de éxito de la práctica.

### Analogía operativa

CompiledQuery guarda una ruta de preparación en EF; no reserva una vía dentro de SQL Server.

### Resultado esperado

Al finalizar el punto 4.9, el proyecto debe compilar, ejecutarse y terminar con 4.9 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.10 y continúa directamente desde este proyecto.

---

## Punto 4.10 — Paginación eficiente: Skip/Take y keyset pagination

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.9. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

Toda paginación necesita orden totalmente determinista. AceriaData ordena por FechaCreacion e Id; keyset usa ambos valores como cursor. Offset es válido para saltos arbitrarios pero puede encarecerse con offsets altos.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.10
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento410.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `ObtenerPaginaKeysetM4`, `ObtenerPaginaOffsetM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public PaginaOrdenesDto ObtenerPaginaOffsetM4(int pagina, int tamano)
    {
        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));
        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));

        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            });

        return new PaginaOrdenesDto
        {
            Sql = consulta.ToQueryString(),
            Elementos = consulta.ToList()
        };
    }

    public PaginaOrdenesDto ObtenerPaginaKeysetM4(
        DateTime ultimaFecha,
        int ultimoId,
        int tamano)
    {
        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));

        var consulta = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o =>
                o.FechaCreacion > ultimaFecha ||
                (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Take(tamano)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            });

        return new PaginaOrdenesDto
        {
            Sql = consulta.ToQueryString(),
            Elementos = consulta.ToList()
        };
    }
}
```

#### Explicación línea a línea — Rendimiento410.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public PaginaOrdenesDto ObtenerPaginaOffsetM4(int pagina, int tamano)` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 9: `    {` → Delimita el bloque sintáctico asociado.

Línea 10: `        if (pagina < 1) throw new ArgumentOutOfRangeException(nameof(pagina));` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 11: `        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 13: `        var consulta = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 15: `            .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 16: `            .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 17: `            .Skip((pagina - 1) * tamano)` → Aplica el desplazamiento de la paginación offset.

Línea 18: `            .Take(tamano)` → Limita el número máximo de elementos.

Línea 19: `            .Select(o => new OrdenPaginaDto` → Proyecta la forma de resultado y controla datos materializados.

Línea 20: `            {` → Delimita el bloque sintáctico asociado.

Línea 21: `                Id = o.Id,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 22: `                NumeroOrden = o.NumeroOrden,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 23: `                Cliente = o.Cliente,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 24: `                Estado = o.Estado,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 25: `                FechaCreacion = o.FechaCreacion` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 26: `            });` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 28: `        return new PaginaOrdenesDto` → Devuelve el resultado calculado al llamador.

Línea 29: `        {` → Delimita el bloque sintáctico asociado.

Línea 30: `            Sql = consulta.ToQueryString(),` → Obtiene la representación SQL sin materializar la consulta.

Línea 31: `            Elementos = consulta.ToList()` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 32: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 33: `    }` → Delimita el bloque sintáctico asociado.

Línea 35: `    public PaginaOrdenesDto ObtenerPaginaKeysetM4(` → Continúa la implementación del punto con esta expresión: public PaginaOrdenesDto ObtenerPaginaKeysetM4(

Línea 36: `        DateTime ultimaFecha,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 37: `        int ultimoId,` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 38: `        int tamano)` → Continúa la implementación del punto con esta expresión: int tamano)

Línea 39: `    {` → Delimita el bloque sintáctico asociado.

Línea 40: `        if (tamano < 1) throw new ArgumentOutOfRangeException(nameof(tamano));` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 42: `        var consulta = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 43: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 44: `            .Where(o =>` → Añade el predicado de filtrado a la forma de consulta.

Línea 45: `                o.FechaCreacion > ultimaFecha ||` → Continúa una condición compuesta usada para validar la equivalencia del resultado.

Línea 46: `                (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))` → Continúa la implementación del punto con esta expresión: (o.FechaCreacion == ultimaFecha && o.Id > ultimoId))

Línea 47: `            .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 48: `            .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 49: `            .Take(tamano)` → Limita el número máximo de elementos.

Línea 50: `            .Select(o => new OrdenPaginaDto` → Proyecta la forma de resultado y controla datos materializados.

Línea 51: `            {` → Delimita el bloque sintáctico asociado.

Línea 52: `                Id = o.Id,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 53: `                NumeroOrden = o.NumeroOrden,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 54: `                Cliente = o.Cliente,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 55: `                Estado = o.Estado,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 56: `                FechaCreacion = o.FechaCreacion` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 57: `            });` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 59: `        return new PaginaOrdenesDto` → Devuelve el resultado calculado al llamador.

Línea 60: `        {` → Delimita el bloque sintáctico asociado.

Línea 61: `            Sql = consulta.ToQueryString(),` → Obtiene la representación SQL sin materializar la consulta.

Línea 62: `            Elementos = consulta.ToList()` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 63: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 64: `    }` → Delimita el bloque sintáctico asociado.

Línea 65: `}` → Delimita el bloque sintáctico asociado.


Archivo complementario: src/AceriaData.Console/DemoData.cs

```csharp
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
```

#### Explicación línea a línea — DemoData.cs

Línea 1: `using AceriaData.Domain.Entities;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `namespace AceriaData.ConsoleApp;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 7: `public static class DemoData` → Continúa la implementación del punto con esta expresión: public static class DemoData

Línea 8: `{` → Delimita el bloque sintáctico asociado.

Línea 9: `    public static void Seed(AceriaDbContext context)` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 10: `    {` → Delimita el bloque sintáctico asociado.

Línea 11: `        if (context.OrdenesFabricacion.IgnoreQueryFilters().Any()) return;` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 13: `        var a36 = new Aleacion { Nombre = "ASTM A36", Codigo = "A36", PorcentajeCarbono = 0.20, PorcentajeManganeso = 0.80 };` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `        var s355 = new Aleacion { Nombre = "S355", Codigo = "S355", PorcentajeCarbono = 0.18, PorcentajeManganeso = 1.20 };` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        var o1 = Orden("OF-2024-0001", "Constructora del Norte", "Pendiente", new DateTime(2024, 1, 15));` → Calcula y conserva el resultado que será validado o mostrado.

Línea 17: `        o1.Planchas.Add(new PlanchaAcero { Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m });` → Continúa la implementación del punto con esta expresión: o1.Planchas.Add(new PlanchaAcero { Espesor = 10.5, Ancho = 1500, Largo = 3000, Peso = 370.5m });

Línea 18: `        o1.Planchas.Add(new PlanchaAcero { Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m });` → Continúa la implementación del punto con esta expresión: o1.Planchas.Add(new PlanchaAcero { Espesor = 12.0, Ancho = 1200, Largo = 2500, Peso = 280.8m });

Línea 19: `        o1.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.20%; Mn 0.80%", TemperaturaColada = 1540 };` → Continúa la implementación del punto con esta expresión: o1.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.20%; Mn 0.80%", TemperaturaColada = 1540 };

Línea 20: `        o1.Certificado = new CertificadoCalidad { NumeroCertificado = "CERT-0001", FechaEmision = new DateTime(2024, 1, 20), OrganismoCertificador = "Aceria QA" };` → Continúa la implementación del punto con esta expresión: o1.Certificado = new CertificadoCalidad { NumeroCertificado = "CERT-0001", FechaEmision = new DateTime(2024, 1, 20), OrganismoCertificador = "Aceria QA" };

Línea 21: `        o1.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = a36, CantidadUtilizada = 500m, FechaAsignacion = new DateTime(2024, 1, 15) });` → Continúa la implementación del punto con esta expresión: o1.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = a36, CantidadUtilizada = 500m, FechaAsignacion = new DateTime(2024, 1, 15) });

Línea 23: `        var o2 = Orden("OF-2024-0002", "Constructora del Sur", "Pendiente", new DateTime(2024, 2, 20));` → Calcula y conserva el resultado que será validado o mostrado.

Línea 24: `        o2.Planchas.Add(new PlanchaAcero { Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m });` → Continúa la implementación del punto con esta expresión: o2.Planchas.Add(new PlanchaAcero { Espesor = 8.0, Ancho = 1000, Largo = 2000, Peso = 125.6m });

Línea 25: `        o2.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.18%; Mn 1.20%", TemperaturaColada = 1535 };` → Continúa la implementación del punto con esta expresión: o2.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.18%; Mn 1.20%", TemperaturaColada = 1535 };

Línea 26: `        o2.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = s355, CantidadUtilizada = 250m, FechaAsignacion = new DateTime(2024, 2, 20) });` → Continúa la implementación del punto con esta expresión: o2.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = s355, CantidadUtilizada = 250m, FechaAsignacion = new DateTime(2024, 2, 20) });

Línea 28: `        var o3 = Orden("OF-2024-0003", "Constructora del Norte", "EnProceso", new DateTime(2024, 3, 10));` → Calcula y conserva el resultado que será validado o mostrado.

Línea 29: `        o3.Planchas.Add(new PlanchaAcero { Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m });` → Continúa la implementación del punto con esta expresión: o3.Planchas.Add(new PlanchaAcero { Espesor = 15.0, Ancho = 1800, Largo = 3500, Peso = 450.0m });

Línea 30: `        o3.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.19%; Mn 1.10%", TemperaturaColada = 1545 };` → Continúa la implementación del punto con esta expresión: o3.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.19%; Mn 1.10%", TemperaturaColada = 1545 };

Línea 31: `        o3.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = s355, CantidadUtilizada = 400m, FechaAsignacion = new DateTime(2024, 3, 10) });` → Continúa la implementación del punto con esta expresión: o3.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = s355, CantidadUtilizada = 400m, FechaAsignacion = new DateTime(2024, 3, 10) });

Línea 33: `        var o4 = Orden("OF-2024-0004", "Constructora del Norte", "Pendiente", new DateTime(2024, 4, 5));` → Calcula y conserva el resultado que será validado o mostrado.

Línea 34: `        o4.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.17%; Mn 0.90%", TemperaturaColada = 1538 };` → Continúa la implementación del punto con esta expresión: o4.Detalle = new DetalleOrden { ComposicionQuimica = "C 0.17%; Mn 0.90%", TemperaturaColada = 1538 };

Línea 36: `        var o5 = Orden("OF-2024-0005", "Constructora del Este", "Completada", new DateTime(2024, 5, 12));` → Calcula y conserva el resultado que será validado o mostrado.

Línea 37: `        o5.Planchas.Add(new PlanchaAcero { Espesor = 9.0, Ancho = 1100, Largo = 2100, Peso = 200.0m });` → Continúa la implementación del punto con esta expresión: o5.Planchas.Add(new PlanchaAcero { Espesor = 9.0, Ancho = 1100, Largo = 2100, Peso = 200.0m });

Línea 38: `        o5.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = a36, CantidadUtilizada = 180m, FechaAsignacion = new DateTime(2024, 5, 12) });` → Continúa la implementación del punto con esta expresión: o5.OrdenesAleaciones.Add(new OrdenAleacion { Aleacion = a36, CantidadUtilizada = 180m, FechaAsignacion = new DateTime(2024, 5, 12) });

Línea 40: `        context.AddRange(a36, s355, o1, o2, o3, o4, o5);` → Continúa la implementación del punto con esta expresión: context.AddRange(a36, s355, o1, o2, o3, o4, o5);

Línea 41: `        context.SaveChanges();` → Continúa la implementación del punto con esta expresión: context.SaveChanges();

Línea 43: `        var extras = Enumerable.Range(6, 15)` → Calcula y conserva el resultado que será validado o mostrado.

Línea 44: `            .Select(i => Orden(` → Proyecta la forma de resultado y controla datos materializados.

Línea 45: `                $"OF-2024-{i:0000}",` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 46: `                i % 3 == 0 ? "Constructora del Norte" :` → Continúa la implementación del punto con esta expresión: i % 3 == 0 ? "Constructora del Norte" :

Línea 47: `                i % 3 == 1 ? "Constructora del Sur" : "Constructora del Este",` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 48: `                i % 4 == 0 ? "EnProceso" : "Pendiente",` → Aporta un argumento o componente intermedio a la construcción multilínea en curso.

Línea 49: `                new DateTime(2024, 6, 1).AddDays(i)))` → Crea la instancia concreta que se devolverá o utilizará como resultado.

Línea 50: `            .ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 52: `        context.OrdenesFabricacion.AddRange(extras);` → Continúa la implementación del punto con esta expresión: context.OrdenesFabricacion.AddRange(extras);

Línea 53: `        context.SaveChanges();` → Continúa la implementación del punto con esta expresión: context.SaveChanges();

Línea 54: `    }` → Delimita el bloque sintáctico asociado.

Línea 56: `    private static OrdenFabricacion Orden(string numero, string cliente, string estado, DateTime fecha) => new()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 57: `    {` → Delimita el bloque sintáctico asociado.

Línea 58: `        NumeroOrden = numero, Cliente = cliente, Estado = estado, FechaCreacion = fecha` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 59: `    };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 60: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class PaginacionUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public PaginacionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.10 PAGINACION ===");

        var offset = _unidad.Ordenes.ObtenerPaginaOffsetM4(2, 5);
        var primeraKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(DateTime.MinValue, 0, 5);
        var cursor = primeraKeyset.Elementos.Last();
        var segundaKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(
            cursor.FechaCreacion,
            cursor.Id,
            5);

        if (offset.Elementos.Count != 5 ||
            primeraKeyset.Elementos.Count != 5 ||
            segundaKeyset.Elementos.Count != 5)
            throw new InvalidOperationException("4.10: paginacion no devolvio el tamano esperado.");

        if (primeraKeyset.Elementos
            .Select(x => x.Id)
            .Intersect(segundaKeyset.Elementos.Select(x => x.Id))
            .Any())
            throw new InvalidOperationException("4.10: keyset repitio filas entre paginas.");

        Console.WriteLine("--- OFFSET ---");
        Console.WriteLine(offset.Sql);
        Console.WriteLine("--- KEYSET ---");
        Console.WriteLine(segundaKeyset.Sql);
    }
}
```

#### Explicación línea a línea — PaginacionUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class PaginacionUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public PaginacionUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.10 PAGINACION ===");` → Publica evidencia observable en la consola.

Línea 14: `        var offset = _unidad.Ordenes.ObtenerPaginaOffsetM4(2, 5);` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        var primeraKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(DateTime.MinValue, 0, 5);` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `        var cursor = primeraKeyset.Elementos.Last();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 17: `        var segundaKeyset = _unidad.Ordenes.ObtenerPaginaKeysetM4(` → Calcula y conserva el resultado que será validado o mostrado.

Línea 18: `            cursor.FechaCreacion,` → Pasa la fecha del cursor anterior como primera componente del seek compuesto.

Línea 19: `            cursor.Id,` → Pasa el Id del cursor anterior como desempate determinista del seek.

Línea 20: `            5);` → Continúa la implementación del punto con esta expresión: 5);

Línea 22: `        if (offset.Elementos.Count != 5 ||` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 23: `            primeraKeyset.Elementos.Count != 5 ||` → Continúa una condición compuesta usada para validar la equivalencia del resultado.

Línea 24: `            segundaKeyset.Elementos.Count != 5)` → Continúa la implementación del punto con esta expresión: segundaKeyset.Elementos.Count != 5)

Línea 25: `            throw new InvalidOperationException("4.10: paginacion no devolvio el tamano esperado.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 27: `        if (primeraKeyset.Elementos` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 28: `            .Select(x => x.Id)` → Proyecta la forma de resultado y controla datos materializados.

Línea 29: `            .Intersect(segundaKeyset.Elementos.Select(x => x.Id))` → Proyecta la forma de resultado y controla datos materializados.

Línea 30: `            .Any())` → Comprueba si existe alguna coincidencia sin materializar toda la secuencia.

Línea 31: `            throw new InvalidOperationException("4.10: keyset repitio filas entre paginas.");` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 33: `        Console.WriteLine("--- OFFSET ---");` → Publica evidencia observable en la consola.

Línea 34: `        Console.WriteLine(offset.Sql);` → Publica evidencia observable en la consola.

Línea 35: `        Console.WriteLine("--- KEYSET ---");` → Publica evidencia observable en la consola.

Línea 36: `        Console.WriteLine(segundaKeyset.Sql);` → Publica evidencia observable en la consola.

Línea 37: `    }` → Delimita el bloque sintáctico asociado.

Línea 38: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();
services.AddScoped<SplitQueriesUseCase>();
services.AddScoped<CompiledQueriesUseCase>();
services.AddScoped<PaginacionUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<PaginacionUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.10 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `services.AddScoped<TraduccionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 28: `services.AddScoped<SplitQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 29: `services.AddScoped<CompiledQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 30: `services.AddScoped<PaginacionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 32: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 33: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 35: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 36: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 37: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 38: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 39: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 41: `var useCase = scope.ServiceProvider.GetRequiredService<PaginacionUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 42: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 44: `Console.WriteLine("4.10 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.10 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Construir la condición seek para un orden compuesto FechaCreacion + Id.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.10 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Offset pagination con Skip/Take.
- Keyset pagination con orden totalmente determinista.
- Filtro, proyección y dirección de paginación.

### Reto de ampliación

Añade mentalmente un filtro de estado a la paginación y conserva el mismo orden compuesto para no saltar ni repetir filas.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No paginar sin OrderBy.
- No usar una clave de ordenación no única como cursor único.
- No dejar tracking activo para listados paginados de solo lectura.

### Analogía operativa

Offset cuenta cajas desde el principio; keyset continúa desde la etiqueta exacta de la última caja vista.

### Resultado esperado

Al finalizar el punto 4.10, el proyecto debe compilar, ejecutarse y terminar con 4.10 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.11 y continúa directamente desde este proyecto.

---

## Punto 4.11 — Diagnóstico con logs, métricas y herramientas

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.10. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

Un tiempo aislado no prueba rendimiento. El diagnóstico reproducible combina SQL, comandos, filas, tracking y tiempo, y usa TagWith para correlación.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.11
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento411.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `DiagnosticarPendientesM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using System.Diagnostics;
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public DiagnosticoRendimientoDto DiagnosticarPendientesM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consulta = _context.OrdenesFabricacion
            .TagWith("M4.11-DIAGNOSTICO")
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Select(o => new OrdenResumenDto
            {
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .Take(10);

        var sql = consulta.ToQueryString();

        var sw = Stopwatch.StartNew();
        var filas = consulta.ToList();
        sw.Stop();

        return new DiagnosticoRendimientoDto
        {
            Filas = filas.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Ticks = sw.ElapsedTicks,
            Sql = sql
        };
    }
}
```

#### Explicación línea a línea — Rendimiento411.cs

Línea 1: `using System.Diagnostics;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 7: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 8: `{` → Delimita el bloque sintáctico asociado.

Línea 9: `    public DiagnosticoRendimientoDto DiagnosticarPendientesM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 10: `    {` → Delimita el bloque sintáctico asociado.

Línea 11: `        _context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 12: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 14: `        var consulta = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `            .TagWith("M4.11-DIAGNOSTICO")` → Etiqueta el SQL para correlacionarlo con logs.

Línea 16: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 17: `            .Where(o => o.Estado == "Pendiente")` → Añade el predicado de filtrado a la forma de consulta.

Línea 18: `            .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 19: `            .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 20: `            .Select(o => new OrdenResumenDto` → Proyecta la forma de resultado y controla datos materializados.

Línea 21: `            {` → Delimita el bloque sintáctico asociado.

Línea 22: `                NumeroOrden = o.NumeroOrden,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 23: `                Cliente = o.Cliente,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 24: `                Estado = o.Estado,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 25: `                FechaCreacion = o.FechaCreacion` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 26: `            })` → Continúa la implementación del punto con esta expresión: })

Línea 27: `            .Take(10);` → Limita el número máximo de elementos.

Línea 29: `        var sql = consulta.ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 31: `        var sw = Stopwatch.StartNew();` → Participa en la medición temporal observacional.

Línea 32: `        var filas = consulta.ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 33: `        sw.Stop();` → Detiene el cronómetro inmediatamente después del bloque que se está midiendo.

Línea 35: `        return new DiagnosticoRendimientoDto` → Devuelve el resultado calculado al llamador.

Línea 36: `        {` → Delimita el bloque sintáctico asociado.

Línea 37: `            Filas = filas.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 38: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 39: `            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 40: `            Ticks = sw.ElapsedTicks,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 41: `            Sql = sql` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 42: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 43: `    }` → Delimita el bloque sintáctico asociado.

Línea 44: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class DiagnosticoRendimientoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public DiagnosticoRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.11 DIAGNOSTICO DE RENDIMIENTO ===");
        var d = _unidad.Ordenes.DiagnosticarPendientesM4();

        if (d.Filas == 0 || d.ConsultasSql != 1 || d.EntidadesRastreadas != 0)
            throw new InvalidOperationException(
                "4.11: las metricas observables no coinciden con la consulta optimizada.");
        if (!d.Sql.Contains("M4.11-DIAGNOSTICO", StringComparison.Ordinal))
            throw new InvalidOperationException("4.11: falta TagWith en el SQL de diagnostico.");

        Console.WriteLine(
            $"Filas={d.Filas} | SQL commands={d.ConsultasSql} | Tracking={d.EntidadesRastreadas} | Ticks={d.Ticks}");
        Console.WriteLine(d.Sql);
    }
}
```

#### Explicación línea a línea — DiagnosticoRendimientoUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class DiagnosticoRendimientoUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public DiagnosticoRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.11 DIAGNOSTICO DE RENDIMIENTO ===");` → Publica evidencia observable en la consola.

Línea 13: `        var d = _unidad.Ordenes.DiagnosticarPendientesM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        if (d.Filas == 0 || d.ConsultasSql != 1 || d.EntidadesRastreadas != 0)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 16: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 17: `                "4.11: las metricas observables no coinciden con la consulta optimizada.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 18: `        if (!d.Sql.Contains("M4.11-DIAGNOSTICO", StringComparison.Ordinal))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException("4.11: falta TagWith en el SQL de diagnostico.");` → Etiqueta el SQL para correlacionarlo con logs.

Línea 21: `        Console.WriteLine(` → Publica evidencia observable en la consola.

Línea 22: `            $"Filas={d.Filas} | SQL commands={d.ConsultasSql} | Tracking={d.EntidadesRastreadas} | Ticks={d.Ticks}");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 23: `        Console.WriteLine(d.Sql);` → Publica evidencia observable en la consola.

Línea 24: `    }` → Delimita el bloque sintáctico asociado.

Línea 25: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();
services.AddScoped<SplitQueriesUseCase>();
services.AddScoped<CompiledQueriesUseCase>();
services.AddScoped<PaginacionUseCase>();
services.AddScoped<DiagnosticoRendimientoUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<DiagnosticoRendimientoUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.11 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `services.AddScoped<TraduccionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 28: `services.AddScoped<SplitQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 29: `services.AddScoped<CompiledQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 30: `services.AddScoped<PaginacionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 31: `services.AddScoped<DiagnosticoRendimientoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 33: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 34: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 36: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 37: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 38: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 39: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 40: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 42: `var useCase = scope.ServiceProvider.GetRequiredService<DiagnosticoRendimientoUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 43: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 45: `Console.WriteLine("4.11 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.11 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Definir qué métrica distinguiría roundtrips de materialización.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.11 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- LogTo, categorías, niveles, ILoggerFactory, SensitiveDataLogging, DetailedErrors y ConfigureWarnings.
- Tiempo, número de comandos, filas y tracking como métricas observables.
- DiagnosticSource/DiagnosticListener y detección de consultas lentas.

### Reto de ampliación

Diseña un observador de consultas lentas con un umbral configurable y explica qué aporta frente al interceptor de conteo.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No incrementar contadores manualmente dentro del repositorio.
- No habilitar SensitiveDataLogging indiscriminadamente en producción.
- No usar una única métrica temporal como diagnóstico completo.

### Analogía operativa

Diagnosticar es instrumentar la línea antes de cambiar la máquina.

### Resultado esperado

Al finalizar el punto 4.11, el proyecto debe compilar, ejecutarse y terminar con 4.11 OK.

### Conexión con el siguiente punto

El siguiente estado es 4.12 y continúa directamente desde este proyecto.

---

## Punto 4.12 — Estrategias de optimización y checklist de rendimiento

### Contexto del proyecto

Este punto continúa M04/PROYECTO/4.11. Conserva solución, capas, filtros, índices y migraciones; M4 no crea migraciones vacías.

### Objetivo práctico

El checklist no impone una clasificación universal. Primero se define la forma necesaria, después se observa SQL, roundtrips, materialización y tracking, y solo entonces se eligen o descartan técnicas.

### Paso 1: Abrir el proyecto del punto

```powershell
cd M04/PROYECTO/4.12
dotnet restore AceriaData.sln
```

### Paso 2: Comprobar migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

Debe seguir apareciendo M2_2_12_Architecture.

### Paso 3: Revisar los cambios de este punto

El archivo principal es src/AceriaData.Infrastructure/Repositories/Rendimiento412.cs. El proyecto conserva el estado anterior y añade únicamente los cambios necesarios para este punto.

**Métodos incorporados en este punto:** `EjecutarChecklistFinalM4`.
**Métodos retirados en este punto:** ninguno.

Los métodos nuevos se ejercen desde el caso de uso del punto y se comprobarán al ejecutar la aplicación.

### Paso 4: Implementar y estudiar Infrastructure

```csharp
using AceriaData.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AceriaData.Infrastructure.Repositories;

public sealed partial class OrdenRepositorio
{
    public ChecklistRendimientoDto EjecutarChecklistFinalM4()
    {
        _context.ChangeTracker.Clear();
        SqlCommandCounterInterceptor.Instance.Reset();

        var consulta = _context.OrdenesFabricacion
            .TagWith("M4.12-CHECKLIST-FINAL")
            .AsNoTracking()
            .Where(o => o.Estado == "Pendiente")
            .OrderBy(o => o.FechaCreacion)
            .ThenBy(o => o.Id)
            .Select(o => new OrdenPaginaDto
            {
                Id = o.Id,
                NumeroOrden = o.NumeroOrden,
                Cliente = o.Cliente,
                Estado = o.Estado,
                FechaCreacion = o.FechaCreacion
            })
            .Take(5);

        var sql = consulta.ToQueryString();
        var filas = consulta.ToList();

        return new ChecklistRendimientoDto
        {
            Filas = filas.Count,
            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),
            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),
            Sql = sql,
            DecisionCompiledQuery =
                "No se aplica: esta consulta final no se ha demostrado como hot path; medir antes de introducir complejidad.",
            DecisionLoading =
                "Proyeccion escalar: no se cargan navegaciones, por lo que Include/SplitQuery no aportan valor en esta consulta."
        };
    }
}
```

#### Explicación línea a línea — Rendimiento412.cs

Línea 1: `using AceriaData.Application.Dtos;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `namespace AceriaData.Infrastructure.Repositories;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 6: `public sealed partial class OrdenRepositorio` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 7: `{` → Delimita el bloque sintáctico asociado.

Línea 8: `    public ChecklistRendimientoDto EjecutarChecklistFinalM4()` → Declara un método concreto de la implementación, con su tipo de retorno, nombre y parámetros.

Línea 9: `    {` → Delimita el bloque sintáctico asociado.

Línea 10: `        _context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 11: `        SqlCommandCounterInterceptor.Instance.Reset();` → Mide comandos SQL reales ejecutados.

Línea 13: `        var consulta = _context.OrdenesFabricacion` → Calcula y conserva el resultado que será validado o mostrado.

Línea 14: `            .TagWith("M4.12-CHECKLIST-FINAL")` → Etiqueta el SQL para correlacionarlo con logs.

Línea 15: `            .AsNoTracking()` → Desactiva tracking para esta consulta de lectura.

Línea 16: `            .Where(o => o.Estado == "Pendiente")` → Añade el predicado de filtrado a la forma de consulta.

Línea 17: `            .OrderBy(o => o.FechaCreacion)` → Forma parte del orden determinista.

Línea 18: `            .ThenBy(o => o.Id)` → Forma parte del orden determinista.

Línea 19: `            .Select(o => new OrdenPaginaDto` → Proyecta la forma de resultado y controla datos materializados.

Línea 20: `            {` → Delimita el bloque sintáctico asociado.

Línea 21: `                Id = o.Id,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 22: `                NumeroOrden = o.NumeroOrden,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 23: `                Cliente = o.Cliente,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 24: `                Estado = o.Estado,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 25: `                FechaCreacion = o.FechaCreacion` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 26: `            })` → Continúa la implementación del punto con esta expresión: })

Línea 27: `            .Take(5);` → Limita el número máximo de elementos.

Línea 29: `        var sql = consulta.ToQueryString();` → Obtiene la representación SQL sin materializar la consulta.

Línea 30: `        var filas = consulta.ToList();` → Materializa la consulta y ejecuta SQL si sigue siendo IQueryable.

Línea 32: `        return new ChecklistRendimientoDto` → Devuelve el resultado calculado al llamador.

Línea 33: `        {` → Delimita el bloque sintáctico asociado.

Línea 34: `            Filas = filas.Count,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 35: `            ConsultasSql = checked((int)SqlCommandCounterInterceptor.Instance.Count),` → Mide comandos SQL reales ejecutados.

Línea 36: `            EntidadesRastreadas = _context.ChangeTracker.Entries().Count(),` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 37: `            Sql = sql,` → Asigna el valor calculado a la propiedad o variable correspondiente del resultado.

Línea 38: `            DecisionCompiledQuery =` → Continúa la implementación del punto con esta expresión: DecisionCompiledQuery =

Línea 39: `                "No se aplica: esta consulta final no se ha demostrado como hot path; medir antes de introducir complejidad.",` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 40: `            DecisionLoading =` → Continúa la implementación del punto con esta expresión: DecisionLoading =

Línea 41: `                "Proyeccion escalar: no se cargan navegaciones, por lo que Include/SplitQuery no aportan valor en esta consulta."` → Continúa la implementación del punto con esta expresión: "Proyeccion escalar: no se cargan navegaciones, por lo que Include/SplitQuery no aportan valor en esta consulta."

Línea 42: `        };` → Cierra la llamada, expresión o inicializador abierto en las líneas anteriores.

Línea 43: `    }` → Delimita el bloque sintáctico asociado.

Línea 44: `}` → Delimita el bloque sintáctico asociado.


Application no recibe DbContext, IQueryable ni referencias a Microsoft.EntityFrameworkCore.

### Paso 5: Implementar el caso de uso

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ChecklistRendimientoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ChecklistRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== 4.12 CHECKLIST FINAL DE OPTIMIZACION ===");
        var r = _unidad.Ordenes.EjecutarChecklistFinalM4();

        if (r.Filas == 0 || r.ConsultasSql != 1 || r.EntidadesRastreadas != 0)
            throw new InvalidOperationException(
                "4.12: el cierre no cumple consulta unica/no-tracking.");
        if (r.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.12: la consulta final incurre en over-fetching.");
        if (!r.Sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "4.12: falta ordenacion determinista.");

        Console.WriteLine(
            $"Filas={r.Filas} | Consultas={r.ConsultasSql} | Tracking={r.EntidadesRastreadas}");
        Console.WriteLine(r.DecisionLoading);
        Console.WriteLine(r.DecisionCompiledQuery);
        Console.WriteLine(r.Sql);
    }
}
```

#### Explicación línea a línea — ChecklistRendimientoUseCase.cs

Línea 1: `using AceriaData.Application.Interfaces;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `namespace AceriaData.Application.UseCases;` → Sitúa el archivo en la capa y espacio de nombres correspondiente.

Línea 5: `public sealed class ChecklistRendimientoUseCase` → Declara la clase concreta que implementa la funcionalidad del punto.

Línea 6: `{` → Delimita el bloque sintáctico asociado.

Línea 7: `    private readonly IUnidadDeTrabajo _unidad;` → Declara la dependencia conservada por la instancia.

Línea 8: `    public ChecklistRendimientoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Constructor del caso de uso e inyección de la unidad de trabajo.

Línea 10: `    public void Ejecutar()` → Define el flujo principal de demostración del punto.

Línea 11: `    {` → Delimita el bloque sintáctico asociado.

Línea 12: `        Console.WriteLine("=== 4.12 CHECKLIST FINAL DE OPTIMIZACION ===");` → Publica evidencia observable en la consola.

Línea 13: `        var r = _unidad.Ordenes.EjecutarChecklistFinalM4();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 15: `        if (r.Filas == 0 || r.ConsultasSql != 1 || r.EntidadesRastreadas != 0)` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 16: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 17: `                "4.12: el cierre no cumple consulta unica/no-tracking.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 18: `        if (r.Sql.Contains("Observaciones", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 19: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 20: `                "4.12: la consulta final incurre en over-fetching.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 21: `        if (!r.Sql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))` → Comprueba una condición necesaria para considerar correcto el resultado.

Línea 22: `            throw new InvalidOperationException(` → Detiene la ejecución con una excepción cuando la comprobación no se cumple.

Línea 23: `                "4.12: falta ordenacion determinista.");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 25: `        Console.WriteLine(` → Publica evidencia observable en la consola.

Línea 26: `            $"Filas={r.Filas} | Consultas={r.ConsultasSql} | Tracking={r.EntidadesRastreadas}");` → Completa el mensaje diagnóstico que documenta la evidencia observada o el motivo del fallo.

Línea 27: `        Console.WriteLine(r.DecisionLoading);` → Publica evidencia observable en la consola.

Línea 28: `        Console.WriteLine(r.DecisionCompiledQuery);` → Publica evidencia observable en la consola.

Línea 29: `        Console.WriteLine(r.Sql);` → Publica evidencia observable en la consola.

Línea 30: `    }` → Delimita el bloque sintáctico asociado.

Línea 31: `}` → Delimita el bloque sintáctico asociado.


### Paso 6: Preparar el composition root

```csharp
using AceriaData.Application.UseCases;
using AceriaData.ConsoleApp;
using AceriaData.Infrastructure;
using AceriaData.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

var connectionString = configuration.GetConnectionString("AceriaDB")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");

var services = new ServiceCollection();
services.AddAceriaInfrastructure(connectionString);
services.AddScoped<BuenasPracticasUseCase>();
services.AddScoped<AnalisisSqlUseCase>();
services.AddScoped<TrackingUseCase>();
services.AddScoped<IdentityResolutionUseCase>();
services.AddScoped<NMasUnoUseCase>();
services.AddScoped<SolucionesNMasUnoUseCase>();
services.AddScoped<OverFetchingUseCase>();
services.AddScoped<TraduccionConsultasUseCase>();
services.AddScoped<SplitQueriesUseCase>();
services.AddScoped<CompiledQueriesUseCase>();
services.AddScoped<PaginacionUseCase>();
services.AddScoped<DiagnosticoRendimientoUseCase>();
services.AddScoped<ChecklistRendimientoUseCase>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
using var scope = provider.CreateScope();

var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();
DemoData.Seed(context);
context.ChangeTracker.Clear();

var useCase = scope.ServiceProvider.GetRequiredService<ChecklistRendimientoUseCase>();
useCase.Ejecutar();

Console.WriteLine("4.12 OK");
```

#### Explicación línea a línea — Program.cs

Línea 1: `using AceriaData.Application.UseCases;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 2: `using AceriaData.ConsoleApp;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 3: `using AceriaData.Infrastructure;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa tipos o extensiones requeridos por esta implementación.

Línea 9: `var configuration = new ConfigurationBuilder()` → Calcula y conserva el resultado que será validado o mostrado.

Línea 10: `    .SetBasePath(AppContext.BaseDirectory)` → Fija el directorio base desde el que Configuration localizará los archivos de configuración.

Línea 11: `    .AddJsonFile("appsettings.json", optional: false)` → Añade appsettings.json como origen obligatorio de configuración.

Línea 12: `    .AddEnvironmentVariables()` → Añade variables de entorno para permitir sobrescribir configuración sin modificar archivos.

Línea 13: `    .Build();` → Construye el objeto de configuración a partir de los proveedores añadidos.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Calcula y conserva el resultado que será validado o mostrado.

Línea 16: `    ?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Hace obligatoria la cadena de conexión y falla de forma explícita si no está configurada.

Línea 18: `var services = new ServiceCollection();` → Calcula y conserva el resultado que será validado o mostrado.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorios, unidad de trabajo e infraestructura usando la cadena de conexión validada.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 21: `services.AddScoped<AnalisisSqlUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 22: `services.AddScoped<TrackingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 23: `services.AddScoped<IdentityResolutionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 24: `services.AddScoped<NMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 25: `services.AddScoped<SolucionesNMasUnoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 26: `services.AddScoped<OverFetchingUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 27: `services.AddScoped<TraduccionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 28: `services.AddScoped<SplitQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 29: `services.AddScoped<CompiledQueriesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 30: `services.AddScoped<PaginacionUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 31: `services.AddScoped<DiagnosticoRendimientoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 32: `services.AddScoped<ChecklistRendimientoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped.

Línea 34: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });` → Importa tipos o extensiones requeridos por esta implementación.

Línea 35: `using var scope = provider.CreateScope();` → Importa tipos o extensiones requeridos por esta implementación.

Línea 37: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 38: `context.Database.EnsureDeleted();` → Reinicia la base de demostración para partir de un estado conocido.

Línea 39: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada.

Línea 40: `DemoData.Seed(context);` → Carga el dataset determinista de AceriaData.

Línea 41: `context.ChangeTracker.Clear();` → Limpia tracking antes de la demostración.

Línea 43: `var useCase = scope.ServiceProvider.GetRequiredService<ChecklistRendimientoUseCase>();` → Resuelve una dependencia obligatoria desde el ámbito.

Línea 44: `useCase.Ejecutar();` → Ejecuta el caso de uso del punto después de preparar la base de datos y los datos de demostración.

Línea 46: `Console.WriteLine("4.12 OK");` → Publica evidencia observable en la consola.


### Paso 7: Compilar

```powershell
dotnet build AceriaData.sln --configuration Release
```

El build debe finalizar sin errores; este estado será la base del punto siguiente.

### Paso 8: Ejecutar en LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

La salida debe terminar con 4.12 OK. Si alguna comprobación no se cumple, la aplicación lanza una excepción y la ejecución no se considera válida.

### Paso 9: Diagnóstico técnico

**Reto resuelto.** Auditar una consulta y justificar tanto técnicas aplicadas como descartadas.

No uses solo tiempo: revisa SQL/roundtrips, cardinalidad, columnas, tracking y frontera de materialización.

### Paso 10: Cierre acumulativo

1. Comprueba que la ejecución termina con 4.12 OK.
2. Comprueba que EnsureCreated no aparece.
3. Conserva las migraciones heredadas.
4. Verifica que Application no depende de EF Core.
5. Compara el comportamiento con el punto anterior y documenta los cambios introducidos.

### Comprobaciones del punto

- Checklist, ciclo medir-identificar-aplicar-verificar-documentar y anti-patrones.
- Métricas de tiempo, comandos, volumen, memoria y coste de materialización.
- Estado acumulativo final de AceriaData y documentación de decisiones.

### Reto de ampliación

Audita una consulta completa y documenta cada decisión: aplicada, descartada y evidencia que la sustenta.

### Errores comunes

- Confundir una medición aislada con una conclusión de rendimiento.
- Materializar antes de terminar filtros o proyecciones sin intención.
- Aplicar una técnica por regla general en lugar de observar la consulta.
- Relajar una comprobación para ocultar un fallo en vez de corregir su causa.
- No optimizar antes de medir.
- No forzar todas las técnicas del módulo sobre una misma consulta.
- No considerar una micro-medición aislada como prueba concluyente.

### Estado final de AceriaData al cerrar M4

- Arquitectura en cuatro proyectos: Domain, Application, Infrastructure y Console.
- Dominio con OrdenFabricacion, PlanchaAcero, Aleacion, EstadoOrden, DetalleOrden, CertificadoCalidad y OrdenAleacion.
- Application mantiene interfaces, DTOs y casos de uso sin depender de Microsoft.EntityFrameworkCore.
- Infrastructure contiene AceriaDbContext, configuraciones Fluent, repositorios, UnitOfWork, migraciones y observabilidad de comandos.
- Modelo con relaciones uno-a-muchos, uno-a-uno y muchos-a-muchos, claves e índices heredados.
- Soft Delete y filtros globales heredados permanecen activos.
- M4 añade análisis SQL, tracking/no-tracking, Identity Resolution, diagnóstico N+1, proyecciones, Split Queries, compiled queries, paginación y métricas.
- Lazy Loading no se activa en M4: los escenarios que podrían producir N+1 se hacen explícitos y medibles.

### Analogía operativa

El checklist final no cambia todas las piezas de la máquina, solo las que la medición justifica.

### Resultado esperado

Al finalizar el punto 4.12, el proyecto debe compilar, ejecutarse y terminar con 4.12 OK.

### Conexión con el siguiente punto

Este punto cierra el código acumulativo de M4 y consolida el checklist.

---
