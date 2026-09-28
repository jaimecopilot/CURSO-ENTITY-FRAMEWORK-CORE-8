# Curso Profesional de Entity Framework Core 8

# Módulo 3 — Prácticas: Consultas con LINQ

**Autor: JAIME GALLO**

Cada práctica trabaja sobre un checkpoint completo en `M03/PROYECTO/3.x`. La progresión es acumulativa: 3.1 parte de M2.12 y cada carpeta posterior conserva el estado anterior más el concepto nuevo.

## Punto 3.1 — Fundamentos de LINQ to Entities

### Contexto del proyecto

Este checkpoint continúa `M02/PROYECTO/2.12`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Fundamentos de LINQ to Entities** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.1
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `Consulta`, `ObtenerSqlFundamentos`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/ConsultasLinqUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConsultasLinqUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");
        var enMemoria = _unidad.Ordenes.ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList();
        var consulta = _unidad.Ordenes.Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion);
        Console.WriteLine("Consulta IQueryable construida: aún no se ha materializado.");
        var enSql = consulta.ToList();
        if (enMemoria.Count != 3 || enSql.Count != 3) throw new InvalidOperationException("Comparación IEnumerable/IQueryable inesperada.");
        Console.WriteLine($"Enumerable: {enMemoria.Count} | IQueryable: {enSql.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlFundamentos());
    }
}
```

#### Explicación línea a línea del caso de uso 3.1

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ConsultasLinqUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var enMemoria = _unidad.Ordenes.ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var consulta = _unidad.Ordenes.Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion);` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `Console.WriteLine("Consulta IQueryable construida: aún no se ha materializado.");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 16: `var enSql = consulta.ToList();` → Materializa la secuencia en una lista; si el origen sigue siendo IQueryable, aquí se ejecuta SQL.

Línea 17: `if (enMemoria.Count != 3 || enSql.Count != 3) throw new InvalidOperationException("Comparación IEnumerable/IQueryable inesperada.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Enumerable: {enMemoria.Count} | IQueryable: {enSql.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlFundamentos());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<ConsultasLinqUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<ConsultasLinqUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.1 OK");
```

#### Explicación línea a línea de Program.cs 3.1

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ConsultasLinqUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<ConsultasLinqUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.1 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.1 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Materializar antes de filtrar mueve el trabajo a memoria y oculta el SQL que se quería estudiar.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Añade un segundo filtro opcional por estado sin materializar hasta el final y compara el SQL.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.1 OK`, el siguiente estado parte exactamente de esta solución y añade **Consultas básicas: Where, OrderBy y ThenBy**.

---

## Punto 3.2 — Consultas básicas: Where, OrderBy y ThenBy

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.1`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Consultas básicas: Where, OrderBy y ThenBy** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.2
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerPendientesPorCliente`, `ObtenerPorEstadoOrdenadasPorFecha`, `ObtenerPorRangoDeFechas`, `ObtenerPorClienteYRangoDeFechas`, `ObtenerSqlConsultaBasica`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/ConsultasBasicasUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ConsultasBasicasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ConsultasBasicasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== WHERE, ORDERBY Y THENBY ===");
        var norte = _unidad.Ordenes.ObtenerPendientesPorCliente("Constructora del Norte");
        var pendientes = _unidad.Ordenes.ObtenerPorEstadoOrdenadasPorFecha("Pendiente");
        var rango = _unidad.Ordenes.ObtenerPorRangoDeFechas(new DateTime(2024,1,1), new DateTime(2024,12,31));
        var combinada = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas("Constructora del Norte", new DateTime(2024,1,1), new DateTime(2024,12,31));
        if (norte.Count != 2 || pendientes.Count != 3 || rango.Count != 5 || combinada.Count != 3) throw new InvalidOperationException("Resultados de filtros/ordenaciones inesperados.");
        Console.WriteLine($"Norte pendientes: {norte.Count} | Pendientes: {pendientes.Count} | Rango: {rango.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlConsultaBasica());
    }
}
```

#### Explicación línea a línea del caso de uso 3.2

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ConsultasBasicasUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ConsultasBasicasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== WHERE, ORDERBY Y THENBY ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var norte = _unidad.Ordenes.ObtenerPendientesPorCliente("Constructora del Norte");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var pendientes = _unidad.Ordenes.ObtenerPorEstadoOrdenadasPorFecha("Pendiente");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var rango = _unidad.Ordenes.ObtenerPorRangoDeFechas(new DateTime(2024,1,1), new DateTime(2024,12,31));` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var combinada = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas("Constructora del Norte", new DateTime(2024,1,1), new DateTime(2024,12,31));` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (norte.Count != 2 || pendientes.Count != 3 || rango.Count != 5 || combinada.Count != 3) throw new InvalidOperationException("Resultados de filtros/ordenaciones inesperados.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Norte pendientes: {norte.Count} | Pendientes: {pendientes.Count} | Rango: {rango.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlConsultaBasica());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<ConsultasBasicasUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<ConsultasBasicasUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.2 OK");
```

#### Explicación línea a línea de Program.cs 3.2

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ConsultasBasicasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<ConsultasBasicasUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.2 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.2 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Un segundo OrderBy sustituye la ordenación anterior; para añadir criterios se usa ThenBy.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Crea una consulta de un cliente en un rango de fechas ordenada por estado y fecha descendente.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.2 OK`, el siguiente estado parte exactamente de esta solución y añade **Proyecciones con Select y tipos anónimos**.

---

## Punto 3.3 — Proyecciones con Select y tipos anónimos

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.2`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Proyecciones con Select y tipos anónimos** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.3
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerClientesUnicos`, `ObtenerResumenes`, `ObtenerResumenesPorEstado`, `ObtenerOrdenesConTotales`, `ObtenerSqlProyeccion`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/ProyeccionesUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ProyeccionesUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ProyeccionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== PROYECCIONES CON SELECT ===");
        var clientes = _unidad.Ordenes.ObtenerClientesUnicos();
        var resumenes = _unidad.Ordenes.ObtenerResumenes();
        var pendientes = _unidad.Ordenes.ObtenerResumenesPorEstado("Pendiente");
        var totales = _unidad.Ordenes.ObtenerOrdenesConTotales();
        if (clientes.Count != 3 || resumenes.Count != 5 || pendientes.Count != 3 || totales.Count != 5) throw new InvalidOperationException("Proyecciones inesperadas.");
        Console.WriteLine($"Clientes: {string.Join(", ", clientes)} | Resúmenes: {resumenes.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccion());
    }
}
```

#### Explicación línea a línea del caso de uso 3.3

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ProyeccionesUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ProyeccionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== PROYECCIONES CON SELECT ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var clientes = _unidad.Ordenes.ObtenerClientesUnicos();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var resumenes = _unidad.Ordenes.ObtenerResumenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var pendientes = _unidad.Ordenes.ObtenerResumenesPorEstado("Pendiente");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var totales = _unidad.Ordenes.ObtenerOrdenesConTotales();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (clientes.Count != 3 || resumenes.Count != 5 || pendientes.Count != 3 || totales.Count != 5) throw new InvalidOperationException("Proyecciones inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Clientes: {string.Join(", ", clientes)} | Resúmenes: {resumenes.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccion());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<ProyeccionesUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<ProyeccionesUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.3 OK");
```

#### Explicación línea a línea de Program.cs 3.3

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ProyeccionesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<ProyeccionesUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.3 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.3 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Un tipo anónimo no puede actuar como contrato público entre capas; cuando el resultado cruza la frontera se usa un DTO.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Proyecta solo número de orden y cliente, y compara el SELECT con la carga de la entidad completa.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.3 OK`, el siguiente estado parte exactamente de esta solución y añade **Proyecciones a DTOs**.

---

## Punto 3.4 — Proyecciones a DTOs

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.3`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Proyecciones a DTOs** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.4
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerOrdenesConPlanchas`, `ObtenerOrdenesConDetalle`, `ObtenerOrdenesCompletas`, `ObtenerSqlProyeccionNavegacion`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/ProyeccionesDtoUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ProyeccionesDtoUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ProyeccionesDtoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== PROYECCIONES A DTOs ===");
        var conPlanchas = _unidad.Ordenes.ObtenerOrdenesConPlanchas();
        var conDetalle = _unidad.Ordenes.ObtenerOrdenesConDetalle();
        var completas = _unidad.Ordenes.ObtenerOrdenesCompletas();
        var primera = completas.Single(o => o.NumeroOrden == "OF-2024-0001");
        if (conPlanchas.Count != 5 || conDetalle.Count != 5 || completas.Count != 5 || primera.Planchas.Count != 2 || primera.Detalle is null) throw new InvalidOperationException("Proyecciones DTO inesperadas.");
        Console.WriteLine($"OF-2024-0001 -> planchas: {primera.Planchas.Count}, detalle: {primera.Detalle.ComposicionQuimica}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccionNavegacion());
    }
}
```

#### Explicación línea a línea del caso de uso 3.4

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ProyeccionesDtoUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ProyeccionesDtoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== PROYECCIONES A DTOs ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var conPlanchas = _unidad.Ordenes.ObtenerOrdenesConPlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var conDetalle = _unidad.Ordenes.ObtenerOrdenesConDetalle();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var completas = _unidad.Ordenes.ObtenerOrdenesCompletas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var primera = completas.Single(o => o.NumeroOrden == "OF-2024-0001");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 17: `if (conPlanchas.Count != 5 || conDetalle.Count != 5 || completas.Count != 5 || primera.Planchas.Count != 2 || primera.Detalle is null) throw new InvalidOperationException("Proyecciones DTO inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"OF-2024-0001 -> planchas: {primera.Planchas.Count}, detalle: {primera.Detalle.ComposicionQuimica}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccionNavegacion());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<ProyeccionesDtoUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<ProyeccionesDtoUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.4 OK");
```

#### Explicación línea a línea de Program.cs 3.4

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ProyeccionesDtoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<ProyeccionesDtoUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.4 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.4 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** No comprobar una relación opcional antes de proyectarla rompe la semántica de null del modelo.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Crea un DTO que incluya una lista de planchas y un detalle opcional sin exponer entidades.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.4 OK`, el siguiente estado parte exactamente de esta solución y añade **Consultas de agregación: Count, Sum, Average, Min y Max**.

---

## Punto 3.5 — Consultas de agregación: Count, Sum, Average, Min y Max

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.4`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Consultas de agregación: Count, Sum, Average, Min y Max** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.5
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ContarOrdenes`, `ContarOrdenesPorEstado`, `ExisteAlgunaOrden`, `ObtenerPesoTotalDePlanchas`, `ObtenerPesoPromedioDePlanchas`, `ObtenerResumenPorCliente`, `ObtenerResumenPorEstado`, `ObtenerResumenMensual`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/AgregacionesUseCase.cs`

```csharp
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
        var peso = _unidad.Ordenes.ObtenerPesoTotalDePlanchas();
        var promedio = _unidad.Ordenes.ObtenerPesoPromedioDePlanchas();
        var porCliente = _unidad.Ordenes.ObtenerResumenPorCliente();
        var porEstado = _unidad.Ordenes.ObtenerResumenPorEstado();
        var mensual = _unidad.Ordenes.ObtenerResumenMensual();
        if (total != 5 || pendientes != 3 || peso <= 0 || promedio <= 0 || porCliente.Count != 3 || porEstado.Count != 3 || mensual.Count != 5) throw new InvalidOperationException("Agregaciones inesperadas.");
        Console.WriteLine($"Órdenes: {total} | Pendientes: {pendientes} | Peso: {peso:N1} kg | Promedio: {promedio:N1} kg");
    }
}
```

#### Explicación línea a línea del caso de uso 3.5

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class AgregacionesUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public AgregacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== AGREGACIONES ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var total = _unidad.Ordenes.ContarOrdenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var pendientes = _unidad.Ordenes.ContarOrdenesPorEstado("Pendiente");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var peso = _unidad.Ordenes.ObtenerPesoTotalDePlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var promedio = _unidad.Ordenes.ObtenerPesoPromedioDePlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `var porCliente = _unidad.Ordenes.ObtenerResumenPorCliente();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 18: `var porEstado = _unidad.Ordenes.ObtenerResumenPorEstado();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 19: `var mensual = _unidad.Ordenes.ObtenerResumenMensual();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 20: `if (total != 5 || pendientes != 3 || peso <= 0 || promedio <= 0 || porCliente.Count != 3 || porEstado.Count != 3 || mensual.Count != 5) throw new InvalidOperationException("Agregaciones inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 21: `Console.WriteLine($"Órdenes: {total} | Pendientes: {pendientes} | Peso: {peso:N1} kg | Promedio: {promedio:N1} kg");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 22: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 23: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<AgregacionesUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<AgregacionesUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.5 OK");
```

#### Explicación línea a línea de Program.cs 3.5

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<AgregacionesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<AgregacionesUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.5 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.5 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Average/Min/Max sobre una secuencia vacía requieren una estrategia explícita; el repositorio usa proyección nullable y coalescencia.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Calcula un resumen mensual y explica qué operaciones se ejecutan como agregados SQL.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.5 OK`, el siguiente estado parte exactamente de esta solución y añade **Agrupaciones con proyección**.

---

## Punto 3.6 — Agrupaciones con proyección

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.5`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Agrupaciones con proyección** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.6
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerResumenPorClienteConOrdenes`, `ObtenerResumenPorClienteYEstado`, `ObtenerResumenPorClienteYEstadoConFiltro`, `ObtenerResumenMensualConOrdenes`, `ObtenerSqlAgrupacionClienteEstado`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/AgrupacionesUseCase.cs`

```csharp
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
    }
}
```

#### Explicación línea a línea del caso de uso 3.6

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class AgrupacionesUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public AgrupacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== AGRUPACIONES CON PROYECCIÓN ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var porCliente = _unidad.Ordenes.ObtenerResumenPorClienteConOrdenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var porClienteEstado = _unidad.Ordenes.ObtenerResumenPorClienteYEstado();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var having = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var mensual = _unidad.Ordenes.ObtenerResumenMensualConOrdenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `var norte = porCliente.Single(x => x.Cliente == "Constructora del Norte");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `if (norte.TotalOrdenes != 3 || norte.Ordenes.Count != 3 || porClienteEstado.Count != 4 || having.Count != 1 || mensual.Count != 5) throw new InvalidOperationException("Agrupaciones inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 19: `Console.WriteLine($"Norte: {norte.TotalOrdenes} órdenes | HAVING: {having.Count} grupo");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlAgrupacionClienteEstado());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 22: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<AgrupacionesUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<AgrupacionesUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.6 OK");
```

#### Explicación línea a línea de Program.cs 3.6

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<AgrupacionesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<AgrupacionesUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.6 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.6 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

En 3.6, la colección interna por cliente se resuelve con **dos consultas acotadas** y composición posterior en memoria. Esto evita enseñar que una proyección de colección dentro de `GroupBy` implique necesariamente una consulta adicional por grupo. El SQL de `GROUP BY` y `HAVING` se valida con una consulta agregada independiente.

### Paso 9: Diagnosticar un error común

**Error:** No hay que asumir que una colección interna dentro de GroupBy implica automáticamente N+1. Se debe inspeccionar la consulta real y diseñar una estrategia acotada.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Construye un HAVING para grupos con más de una orden y verifica que el SQL contiene GROUP BY/HAVING.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.6 OK`, el siguiente estado parte exactamente de esta solución y añade **Joins y navegación en consultas**.

---

## Punto 3.7 — Joins y navegación en consultas

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.6`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Joins y navegación en consultas** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.7
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerJoinOrdenesPlanchas`, `ObtenerLeftJoinOrdenesPlanchas`, `ObtenerOrdenesConDetalleJoin`, `ObtenerOrdenesConAleaciones`, `ObtenerSqlJoinExplicito`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/JoinsUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class JoinsUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public JoinsUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== JOINS Y NAVEGACIÓN ===");
        var inner = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();
        var left = _unidad.Ordenes.ObtenerLeftJoinOrdenesPlanchas();
        var detalle = _unidad.Ordenes.ObtenerOrdenesConDetalleJoin();
        var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleaciones();
        if (inner.Count != 5 || left.Count != 6 || detalle.Count != 5 || aleaciones.Count != 5) throw new InvalidOperationException("Joins inesperados.");
        if (!left.Any(x => x.NumeroOrden == "OF-2024-0004" && x.Peso is null)) throw new InvalidOperationException("LEFT JOIN no conservó la orden sin planchas.");
        Console.WriteLine($"INNER filas: {inner.Count} | LEFT filas: {left.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlJoinExplicito());
    }
}
```

#### Explicación línea a línea del caso de uso 3.7

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class JoinsUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public JoinsUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== JOINS Y NAVEGACIÓN ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var inner = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var left = _unidad.Ordenes.ObtenerLeftJoinOrdenesPlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var detalle = _unidad.Ordenes.ObtenerOrdenesConDetalleJoin();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleaciones();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (inner.Count != 5 || left.Count != 6 || detalle.Count != 5 || aleaciones.Count != 5) throw new InvalidOperationException("Joins inesperados.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `if (!left.Any(x => x.NumeroOrden == "OF-2024-0004" && x.Peso is null)) throw new InvalidOperationException("LEFT JOIN no conservó la orden sin planchas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 19: `Console.WriteLine($"INNER filas: {inner.Count} | LEFT filas: {left.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlJoinExplicito());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 22: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<JoinsUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<JoinsUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.7 OK");
```

#### Explicación línea a línea de Program.cs 3.7

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<JoinsUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<JoinsUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.7 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.7 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Un INNER JOIN elimina órdenes sin coincidencia. Si deben conservarse, se usa un patrón LEFT JOIN y propiedades anulables.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Incluye órdenes sin planchas con LEFT JOIN y demuestra que OF-2024-0004 sigue presente.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.7 OK`, el siguiente estado parte exactamente de esta solución y añade **Eager Loading con Include y ThenInclude**.

---

## Punto 3.8 — Eager Loading con Include y ThenInclude

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.7`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Eager Loading con Include y ThenInclude** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.8
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerOrdenesConPlanchasInclude`, `ObtenerOrdenesConAleacionesInclude`, `ObtenerOrdenesConPlanchasPesadasInclude`, `ObtenerOrdenesConPlanchasYDetalleSplitQuery`, `ObtenerSqlInclude`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/CargaEagerUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class CargaEagerUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public CargaEagerUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== EAGER LOADING ===");
        var planchas = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();
        var detalle = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleInclude();
        var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleacionesInclude();
        var filtradas = _unidad.Ordenes.ObtenerOrdenesConPlanchasPesadasInclude();
        var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSplitQuery();
        if (planchas.Count != 5 || detalle.Count != 5 || aleaciones.Count != 5 || split.Count != 5) throw new InvalidOperationException("Carga Eager inesperada.");
        if (filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 1) throw new InvalidOperationException("Filtered Include inesperado.");
        Console.WriteLine($"Include: {planchas.Count} órdenes | SplitQuery: {split.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlInclude());
    }
}
```

#### Explicación línea a línea del caso de uso 3.8

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class CargaEagerUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public CargaEagerUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== EAGER LOADING ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var planchas = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var detalle = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleInclude();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleacionesInclude();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var filtradas = _unidad.Ordenes.ObtenerOrdenesConPlanchasPesadasInclude();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSplitQuery();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 18: `if (planchas.Count != 5 || detalle.Count != 5 || aleaciones.Count != 5 || split.Count != 5) throw new InvalidOperationException("Carga Eager inesperada.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 19: `if (filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 1) throw new InvalidOperationException("Filtered Include inesperado.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 20: `Console.WriteLine($"Include: {planchas.Count} órdenes | SplitQuery: {split.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 21: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlInclude());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 22: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 23: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<CargaEagerUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<CargaEagerUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.8 OK");
```

#### Explicación línea a línea de Program.cs 3.8

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<CargaEagerUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<CargaEagerUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.8 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.8 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

En 3.8, el Filtered Include usa `AsNoTracking()` para que navigation fix-up no reincorpore planchas previamente rastreadas que no cumplan el filtro.

### Paso 9: Diagnosticar un error común

**Error:** Filtered Include con entidades ya rastreadas puede verse afectado por navigation fix-up; el ejemplo usa AsNoTracking.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Compara Include único y AsSplitQuery con las mismas relaciones y observa el SQL.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.8 OK`, el siguiente estado parte exactamente de esta solución y añade **Lazy Loading: configuración, funcionamiento y riesgos**.

---

## Punto 3.9 — Lazy Loading: configuración, funcionamiento y riesgos

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.8`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Lazy Loading: configuración, funcionamiento y riesgos** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.9
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerTodasSinInclude`, `UseLazyLoadingProxies`, `virtual`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/CargaLazyUseCase.cs`

```csharp
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
    }
}
```

#### Explicación línea a línea del caso de uso 3.9

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class CargaLazyUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public CargaLazyUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== LAZY LOADING (DEMOSTRACIÓN) ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var ordenes = _unidad.Ordenes.ObtenerTodasSinInclude();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var totalPlanchas = 0;` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `foreach (var orden in ordenes)` → Recorre los resultados para evaluar el comportamiento de cada entidad o relación.

Línea 16: `totalPlanchas += orden.Planchas.Count;` → Participa en la composición o validación concreta del flujo de este punto.

Línea 17: `if (ordenes.Count != 5 || totalPlanchas != 5) throw new InvalidOperationException("Lazy Loading no cargó las relaciones.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Órdenes: {ordenes.Count} | Planchas accedidas bajo demanda: {totalPlanchas}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine("Advertencia docente: el acceso dentro de un bucle puede producir N+1 consultas.");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<CargaLazyUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<CargaLazyUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.9 OK");
```

#### Explicación línea a línea de Program.cs 3.9

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<CargaLazyUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<CargaLazyUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.9 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.9 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

En 3.9, los proxies y las navegaciones `virtual` existen solo para demostrar Lazy Loading y el riesgo N+1. El siguiente checkpoint vuelve a desactivar la configuración de proxies.

### Paso 9: Diagnosticar un error común

**Error:** Acceder a navegaciones Lazy dentro de un bucle puede producir N+1 consultas y depender de un DbContext aún vivo.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Cuenta cuántas navegaciones se acceden en el bucle y razona cuántas consultas puede provocar Lazy Loading.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.9 OK`, el siguiente estado parte exactamente de esta solución y añade **Explicit Loading**.

---

## Punto 3.10 — Explicit Loading

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.9`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Explicit Loading** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.10
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerConCargaExplicita`, `ObtenerConPlanchasPesadasExplicitas`, `IsLoaded`, `Query().Where`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/CargaExplicitaUseCase.cs`

```csharp
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
    }
}
```

#### Explicación línea a línea del caso de uso 3.10

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class CargaExplicitaUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public CargaExplicitaUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== EXPLICIT LOADING ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var orden = _unidad.Ordenes.ObtenerConCargaExplicita("OF-2024-0001");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var filtrada = _unidad.Ordenes.ObtenerConPlanchasPesadasExplicitas("OF-2024-0002", 300m);` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `if (orden is null || orden.Planchas.Count != 2 || orden.Detalle is null) throw new InvalidOperationException("Carga explícita incompleta.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 16: `if (filtrada is null || filtrada.Planchas.Count != 0) throw new InvalidOperationException("Query() de carga explícita inesperado.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 17: `Console.WriteLine($"Carga completa: {orden.Planchas.Count} planchas | Filtrada OF-0002: {filtrada.Planchas.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 18: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 19: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<CargaExplicitaUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<CargaExplicitaUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.10 OK");
```

#### Explicación línea a línea de Program.cs 3.10

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<CargaExplicitaUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<CargaExplicitaUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.10 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.10 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Cargar explícitamente una navegación que ya estaba cargada puede repetir trabajo; IsLoaded permite evitarlo.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Carga únicamente planchas por encima de un peso mínimo mediante Collection(...).Query().

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.10 OK`, el siguiente estado parte exactamente de esta solución y añade **Composición de consultas y ejecución diferida**.

---

## Punto 3.11 — Composición de consultas y ejecución diferida

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.10`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Composición de consultas y ejecución diferida** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.11
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `BuscarOrdenes`, `Skip`, `Take`, `ToQueryString`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/ComposicionConsultasUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class ComposicionConsultasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public ComposicionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== COMPOSICIÓN DE CONSULTAS ===");
        var resultado = _unidad.Ordenes.BuscarOrdenes("Constructora del Norte", "Pendiente", new DateTime(2024,1,1), "fecha", true, 1, 10);
        if (resultado.Elementos.Count != 2) throw new InvalidOperationException("Consulta compuesta inesperada.");
        Console.WriteLine($"Resultados: {resultado.Elementos.Count}");
        Console.WriteLine(resultado.Sql);
    }
}
```

#### Explicación línea a línea del caso de uso 3.11

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ComposicionConsultasUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ComposicionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== COMPOSICIÓN DE CONSULTAS ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var resultado = _unidad.Ordenes.BuscarOrdenes("Constructora del Norte", "Pendiente", new DateTime(2024,1,1), "fecha", true, 1, 10);` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `if (resultado.Elementos.Count != 2) throw new InvalidOperationException("Consulta compuesta inesperada.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 15: `Console.WriteLine($"Resultados: {resultado.Elementos.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 16: `Console.WriteLine(resultado.Sql);` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 17: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 18: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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
services.AddScoped<ComposicionConsultasUseCase>();

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

var useCase = scope.ServiceProvider.GetRequiredService<ComposicionConsultasUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.11 OK");
```

#### Explicación línea a línea de Program.cs 3.11

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ComposicionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<ComposicionConsultasUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.11 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.11 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

### Paso 9: Diagnosticar un error común

**Error:** Skip/Take sin un orden determinista produce páginas inestables; la paginación debe seguir a OrderBy.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Añade un filtro opcional y una segunda forma de ordenación sin ejecutar la consulta antes de Skip/Take.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.11 OK`, el siguiente estado parte exactamente de esta solución y añade **Buenas prácticas en el acceso a datos y composición de consultas**.

---

## Punto 3.12 — Buenas prácticas en el acceso a datos y composición de consultas

### Contexto del proyecto

Este checkpoint continúa `M03/PROYECTO/3.11`. Se mantiene la separación Domain/Application/Infrastructure/Console y el esquema se obtiene con migraciones, no con `EnsureCreated()`.

### Objetivo práctico

Implementar y ejecutar **Buenas prácticas en el acceso a datos y composición de consultas** sobre AceriaData, inspeccionando resultados reales y, cuando corresponde, el SQL generado por EF Core 8.

### Paso 1: Abrir el checkpoint

```powershell
cd M03/PROYECTO/3.12
dotnet restore AceriaData.sln
```

La solución contiene los cuatro proyectos y puede abrirse directamente en Visual Studio 2022/2026 compatible con .NET 8.

### Paso 2: Comprobar la historia de migraciones

```powershell
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console
```

El listado debe conservar las migraciones de M1 y M2, incluida `M2_2_12_Architecture`. LINQ y las estrategias de carga de M3 no cambian el esquema, por lo que M3 no inventa migraciones vacías.

### Paso 3: Revisar los cambios de este punto

Los elementos trazados en este estado son: `ObtenerResumenesPendientesOptimizado`, `ExisteAlgunaOrdenPendiente`, `ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano`, `ObtenerPorNumeroOptimizado`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

### Paso 4: Implementar y estudiar el caso de uso

Archivo: `src/AceriaData.Application/BuenasPracticasUseCase.cs`

```csharp
using AceriaData.Application.Interfaces;

namespace AceriaData.Application.UseCases;

public sealed class BuenasPracticasUseCase
{
    private readonly IUnidadDeTrabajo _unidad;
    public BuenasPracticasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;

    public void Ejecutar()
    {
        Console.WriteLine("=== BUENAS PRÁCTICAS EN EL ACCESO A DATOS ===");
        var resumenes = _unidad.Ordenes.ObtenerResumenesPendientesOptimizado();
        var existe = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();
        var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();
        var inexistente = _unidad.Ordenes.ObtenerPorNumeroOptimizado("OF-2024-9999");
        if (resumenes.Count != 3 || !existe || split.Count != 5 || inexistente is not null) throw new InvalidOperationException("Buenas prácticas: validación E2E fallida.");
        Console.WriteLine($"Pendientes proyectadas: {resumenes.Count} | Any: {existe} | SplitQuery: {split.Count}");
        Console.WriteLine("El contrato final ya no expone IQueryable fuera de Infrastructure.");
    }
}
```

#### Explicación línea a línea del caso de uso 3.12

Línea 1: `using AceriaData.Application.Interfaces;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class BuenasPracticasUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public BuenasPracticasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 12: `Console.WriteLine("=== BUENAS PRÁCTICAS EN EL ACCESO A DATOS ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var resumenes = _unidad.Ordenes.ObtenerResumenesPendientesOptimizado();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var existe = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var inexistente = _unidad.Ordenes.ObtenerPorNumeroOptimizado("OF-2024-9999");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (resumenes.Count != 3 || !existe || split.Count != 5 || inexistente is not null) throw new InvalidOperationException("Buenas prácticas: validación E2E fallida.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Pendientes proyectadas: {resumenes.Count} | Any: {existe} | SplitQuery: {split.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine("El contrato final ya no expone IQueryable fuera de Infrastructure.");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Abre o cierra el bloque sintáctico correspondiente.

Línea 21: `}` → Abre o cierra el bloque sintáctico correspondiente.

### Paso 5: Preparar y ejecutar el composition root

Archivo: `src/AceriaData.Console/Program.cs`

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

var useCase = scope.ServiceProvider.GetRequiredService<BuenasPracticasUseCase>();
useCase.Ejecutar();

Console.WriteLine("3.12 OK");
```

#### Explicación línea a línea de Program.cs 3.12

Línea 1: `using AceriaData.Application.UseCases;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 2: `using AceriaData.ConsoleApp;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 3: `using AceriaData.Infrastructure;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Participa en la composición o validación concreta del flujo de este punto.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Participa en la composición o validación concreta del flujo de este punto.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 23: `{` → Abre o cierra el bloque sintáctico correspondiente.

Línea 24: `ValidateOnBuild = true,` → Participa en la composición o validación concreta del flujo de este punto.

Línea 25: `ValidateScopes = true` → Participa en la composición o validación concreta del flujo de este punto.

Línea 26: `});` → Participa en la composición o validación concreta del flujo de este punto.

Línea 27: `using var scope = provider.CreateScope();` → Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.

Línea 29: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext para preparar la base de demostración.

Línea 30: `context.Database.EnsureDeleted();` → Elimina solo la base de demostración para que el E2E comience desde un estado reproducible; no crea el esquema.

Línea 31: `context.Database.Migrate();` → Aplica la historia real de migraciones heredada de M2 para crear o actualizar el esquema.

Línea 32: `DemoData.Seed(context);` → Inserta el conjunto determinista de órdenes, planchas, detalles y aleaciones usado por los E2E.

Línea 33: `context.ChangeTracker.Clear();` → Desacopla las entidades sembradas para que las consultas se materialicen de nuevo desde SQL.

Línea 35: `var useCase = scope.ServiceProvider.GetRequiredService<BuenasPracticasUseCase>();` → Resuelve el caso de uso concreto registrado para este checkpoint.

Línea 36: `useCase.Ejecutar();` → Ejecuta la demostración y sus aserciones E2E.

Línea 38: `Console.WriteLine("3.12 OK");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

### Paso 6: Compilar el estado acumulativo

```powershell
dotnet build AceriaData.sln --configuration Release
```

La compilación debe terminar sin errores. Application no referencia Entity Framework Core; todas las llamadas específicas de EF permanecen en Infrastructure o en el composition root.

### Paso 7: Ejecutar el E2E contra SQL Server LocalDB

```powershell
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release
```

El proceso elimina la base de demostración, aplica `Database.Migrate()`, inserta un dataset determinista, limpia el Change Tracker y ejecuta el caso de uso. El marcador final obligatorio es:

```text
3.12 OK
```

### Paso 8: Analizar resultado y SQL

El caso de uso contiene aserciones que hacen fallar el proceso si cambian los resultados esperados. Cuando el punto estudia traducción SQL, el repositorio devuelve `ToQueryString()` y la consola imprime exactamente la sentencia que EF Core 8 ha construido para SQL Server. No se usa una sentencia SQL ficticia como prueba.

En 3.12, `IOrdenRepositorio` ya no devuelve `IQueryable<OrdenFabricacion>`; el objetivo es cerrar el módulo con una frontera limpia entre Application e Infrastructure.

### Paso 9: Diagnosticar un error común

**Error:** Exponer IQueryable desde Application deja que capas superiores acoplen su lógica al proveedor; el contrato final lo elimina.

Para diagnosticarlo, compara el método del repositorio, la salida E2E y el SQL obtenido con `ToQueryString()` cuando esté disponible. No cambies simultáneamente datos, query y expectativa: modifica una sola variable para poder atribuir la causa.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Revisa IOrdenRepositorio y justifica por qué ya no contiene IQueryable aunque Infrastructure siga componiendo consultas.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.12 OK`, el siguiente estado parte exactamente de esta solución; con ello queda cerrado el Módulo 3.

---

