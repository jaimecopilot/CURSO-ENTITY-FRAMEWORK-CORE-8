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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ConsultasLinqUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ConsultasLinqUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== FUNDAMENTOS DE LINQ TO ENTITIES ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var enMemoria = _unidad.Ordenes.ObtenerTodas().Where(o => o.Cliente == "Constructora del Norte").ToList();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var consulta = _unidad.Ordenes.Consulta().Where(o => o.Cliente == "Constructora del Norte").OrderBy(o => o.FechaCreacion);` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `Console.WriteLine("Consulta IQueryable construida: aún no se ha materializado.");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 16: `var enSql = consulta.ToList();` → Materializa la secuencia en una lista; si el origen sigue siendo IQueryable, aquí se ejecuta SQL.

Línea 17: `if (enMemoria.Count != 3 || enSql.Count != 3) throw new InvalidOperationException("Comparación IEnumerable/IQueryable inesperada.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Enumerable: {enMemoria.Count} | IQueryable: {enSql.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlFundamentos());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ConsultasLinqUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.1

#### Diagnóstico técnico

El objetivo no es sólo obtener el mismo número de órdenes con `IEnumerable<T>` e `IQueryable<T>`, sino comprobar dónde se ejecuta el filtro. Si se materializa primero, el SQL trae todas las filas visibles y el predicado se aplica en memoria; si se mantiene `IQueryable<T>`, el predicado llega al `WHERE` de SQL Server.

#### Reto resuelto y verificación adicional

Construye una consulta de órdenes pendientes del Norte, ordenadas de más reciente a más antigua y proyectadas a número y cliente. El pipeline correcto es `Where → OrderByDescending → Select → ToList`. Antes de materializar, usa `ToQueryString()` y verifica `WHERE`, `ORDER BY` y una proyección mínima.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Filtro se aplica en memoria | Se llamó a ToList antes del filtro | Aplicar ToList al final |
| Expresión no traducida en filtro u operación de servidor | Se usó lógica que el proveedor no puede traducir fuera de la proyección superior | Reescribir con expresiones traducibles o cambiar explícitamente a evaluación cliente cuando esté justificado |
| Múltiples consultas | Se materializó varias veces | Materializar una sola vez al final |
| ToQueryString ejecuta la consulta | No, solo la construye | Verificar que no se llama a ToList |
| IQueryable expuesto en el repositorio | Anti-patrón | Encapsular en métodos específicos (Módulo 4) |
| Falta el using de EF Core | El método ToQueryString no está disponible | Añadir using Microsoft.EntityFrameworkCore; |

#### Analogía operativa

LINQ to Entities en una acería es como el lenguaje que usa el jefe de planta para pedir informes al archivo central. El jefe no va al archivo a buscar los documentos: envía una orden con las condiciones (cliente, estado, fecha) y el archivo devuelve solo los documentos que cumplen esas condiciones. Si el jefe pidiera todos los documentos y luego los filtrara él mismo, perdería tiempo y esfuerzo. Eso es lo que ocurre en este ejemplo cuando primero se llama a `ToList()` y después se filtra como `IEnumerable`: los datos ya están materializados y el filtro posterior se ejecuta en el cliente. `IEnumerable` por sí solo no significa que toda secuencia esté previamente cargada en memoria. La ejecución diferida es como preparar la orden de búsqueda antes de enviarla: el jefe puede añadir condiciones hasta que esté seguro, y solo entonces la envía. La materialización es el momento en que se envía la orden y se recibe la respuesta. `ToQueryString()` es como pedir una copia de la orden sin enviarla: permite revisar el SQL que EF Core ha construido sin ejecutar esa consulta.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ConsultasBasicasUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ConsultasBasicasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== WHERE, ORDERBY Y THENBY ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var norte = _unidad.Ordenes.ObtenerPendientesPorCliente("Constructora del Norte");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var pendientes = _unidad.Ordenes.ObtenerPorEstadoOrdenadasPorFecha("Pendiente");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var rango = _unidad.Ordenes.ObtenerPorRangoDeFechas(new DateTime(2024,1,1), new DateTime(2024,12,31));` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var combinada = _unidad.Ordenes.ObtenerPorClienteYRangoDeFechas("Constructora del Norte", new DateTime(2024,1,1), new DateTime(2024,12,31));` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (norte.Count != 2 || pendientes.Count != 3 || rango.Count != 5 || combinada.Count != 3) throw new InvalidOperationException("Resultados de filtros/ordenaciones inesperados.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Norte pendientes: {norte.Count} | Pendientes: {pendientes.Count} | Rango: {rango.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlConsultaBasica());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ConsultasBasicasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.2

#### Diagnóstico técnico

Prueba filtros por cliente, estado y fechas antes de combinarlos. `ThenBy` añade un criterio a un `OrderBy`; un segundo `OrderBy` sustituiría el criterio anterior. Los métodos de cadena traducibles no deben confundirse con formas necesariamente óptimas para índices.

#### Reto resuelto y verificación adicional

Para Constructora del Norte durante 2024, valida el método acumulativo que filtra por cliente/rango y ordena por estado ascendente y fecha descendente. El SQL debe parametrizar las fechas y no concatenar valores.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Filtro se aplica en memoria | Se llamó a ToList antes del filtro | Aplicar ToList al final |
| Ordenación no aplicada | Se llamó a OrderBy después de ToList | Aplicar OrderBy antes de ToList |
| Múltiples consultas | Se materializó varias veces | Materializar una sola vez al final |
| IQueryable expuesto en el repositorio | Anti-patrón | Encapsular en métodos específicos (Módulo 4) |
| Filtro por fecha incorrecto | Se usó == en lugar de >= y <= | Usar operadores de comparación |
| ThenBy sin OrderBy | Se llamó a ThenBy sin OrderBy previo | Llamar a OrderBy primero |
| Falta el using de EF Core | El método ToQueryString no está disponible | Añadir using Microsoft.EntityFrameworkCore; |

#### Analogía operativa

Las consultas básicas con LINQ en una acería son como las órdenes de búsqueda que el jefe de planta envía al archivo central. Where es el filtro: "solo quiero las órdenes del cliente Constructora del Norte". OrderBy es el criterio de ordenación: "ordénalas por fecha de creación". ThenBy es el criterio secundario: "y dentro de cada fecha, ordénalas por estado". El archivo central recibe la orden, busca en sus índices y devuelve exactamente lo que se pidió. Si el jefe pidiera todos los documentos y los ordenara él mismo, perdería tiempo y esfuerzo. Eso es lo que pasa cuando se materializa antes de filtrar. Las consultas encapsuladas en el repositorio son como las ventanillas especializadas del archivo: cada una sabe qué buscar y cómo ordenarlo. El jefe no necesita conocer el archivo por dentro: solo pide lo que necesita a la ventanilla correspondiente. Así funcionan las consultas básicas con LINQ: se filtran y se ordenan en el servidor, y los resultados se devuelven ya preparados.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ProyeccionesUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ProyeccionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== PROYECCIONES CON SELECT ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var clientes = _unidad.Ordenes.ObtenerClientesUnicos();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var resumenes = _unidad.Ordenes.ObtenerResumenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var pendientes = _unidad.Ordenes.ObtenerResumenesPorEstado("Pendiente");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var totales = _unidad.Ordenes.ObtenerOrdenesConTotales();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (clientes.Count != 3 || resumenes.Count != 5 || pendientes.Count != 3 || totales.Count != 5) throw new InvalidOperationException("Proyecciones inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Clientes: {string.Join(", ", clientes)} | Resúmenes: {resumenes.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccion());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ProyeccionesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.3

#### Diagnóstico técnico

Compara materializar la entidad completa con proyectar sólo `NumeroOrden`, `Cliente`, `Estado` y `FechaCreacion`. La ventaja demostrable es la reducción del shape transferido y de entidades materializadas. `Distinct()` debe permanecer en servidor si se aplica antes de `ToList()`.

#### Reto resuelto y verificación adicional

Usa `ObtenerClientesUnicos()` y confirma ausencia de duplicados y orden estable. Después contrasta `ObtenerResumenes()` con una carga completa: el DTO contiene exactamente los campos consumidos por Application.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Propiedad no proyectada | Se accede a una propiedad que no está en la proyección | Proyectar todas las propiedades necesarias |
| Tipo anónimo fuera de ámbito | Se intenta usar un tipo anónimo fuera del método | Proyectar a un DTO con nombre |
| Función no traducida | Se usa una función que EF Core no puede traducir | Usar funciones traducibles |
| Proyección antes del filtro | Se proyecta antes de filtrar | Filtrar y ordenar antes de proyectar |
| Tipo de colección incompatible | El DTO exige `List<T>` pero la proyección entrega otra forma de secuencia | Usar `ToList()` cuando el *shape* de destino exige una lista; no convertirlo en una regla universal |
| Distinct no aplicado | Se olvidó el operador Distinct | Añadir .Distinct() antes de ToList |

#### Analogía operativa

Las proyecciones con Select en una acería son como los resúmenes que el jefe de planta pide al archivo central. En lugar de recibir la carpeta completa de cada orden con todos sus documentos, el jefe pide solo los datos que necesita: el número de orden, el cliente y el estado. El archivo central prepara un resumen con esos datos y lo entrega. El jefe no necesita los documentos completos para tomar decisiones: solo los datos clave. Proyectar a un tipo anónimo es como pedir un resumen informal: se usa en el momento y no se archiva. Proyectar a un DTO es como pedir un formulario estandarizado: tiene nombre, se puede archivar y se puede usar en otros departamentos. Proyectar con Distinct es como pedir la lista de clientes únicos: sin duplicados. Proyectar con funciones de agregación es como pedir el total de planchas y el peso total: valores calculados en el archivo central. Así funcionan las proyecciones en EF Core: se seleccionan solo los datos necesarios, se reduce el volumen de información y se mejora el rendimiento.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ProyeccionesDtoUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ProyeccionesDtoUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== PROYECCIONES A DTOs ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var conPlanchas = _unidad.Ordenes.ObtenerOrdenesConPlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var conDetalle = _unidad.Ordenes.ObtenerOrdenesConDetalle();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var completas = _unidad.Ordenes.ObtenerOrdenesCompletas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var primera = completas.Single(o => o.NumeroOrden == "OF-2024-0001");` → Selecciona de forma inequívoca la orden OF-2024-0001 del resultado proyectado para comprobar sus datos anidados.

Línea 17: `if (conPlanchas.Count != 5 || conDetalle.Count != 5 || completas.Count != 5 || primera.Planchas.Count != 2 || primera.Detalle is null) throw new InvalidOperationException("Proyecciones DTO inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"OF-2024-0001 -> planchas: {primera.Planchas.Count}, detalle: {primera.Detalle.ComposicionQuimica}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlProyeccionNavegacion());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ProyeccionesDtoUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.4

#### Diagnóstico técnico

Las proyecciones a DTO deben decidir qué ocurre con relaciones opcionales. `Detalle` puede ser `null`; el DTO también debe admitirlo. Las colecciones relacionadas se convierten a DTOs de lectura, no se filtran entidades de Infrastructure hacia Application.

#### Reto resuelto y verificación adicional

Selecciona `OF-2024-0001` en `OrdenCompletaDto`: debe tener dos planchas y detalle. Comprueba también una relación opcional ausente sin provocar `NullReferenceException`.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| NullReferenceException en DTO anidado | No se comprobó si la entidad relacionada es null | Usar operador ternario antes de proyectar |
| Tipo de colección incompatible | El DTO declara `List<T>` pero la proyección no produce una lista compatible | Materializar con `ToList()` cuando lo exija el tipo de destino |
| Propiedad no proyectada | Se accede a una propiedad que no está en la proyección | Proyectar todas las propiedades necesarias |
| GroupBy tratado siempre como `GROUP BY` SQL | Se presupone que cualquier forma de agrupación tiene la misma traducción | Para el patrón relacional usar clave + agregados; para otros *shapes*, inspeccionar la traducción real |
| SelectMany con colección vacía | La entidad principal no aparece en el resultado | Usar DefaultIfEmpty |
| Average sobre colección vacía | La semántica depende de si el tipo es anulable | Proyectar a nullable y definir el resultado, o validar que existan elementos |

#### Analogía operativa

Las proyecciones a DTOs en una acería son como los formularios estandarizados que se usan para transferir información entre departamentos. En lugar de enviar la carpeta completa de una orden con todos sus documentos, se envía un formulario con los campos que el departamento receptor necesita. El formulario tiene un nombre, un formato fijo y se puede archivar. Proyectar a un DTO con inicializador de objeto es como rellenar un formulario campo a campo. Proyectar a un DTO con constructor es como usar un formulario preimpreso que garantiza que todos los campos estén rellenos. Proyectar una colección de navegación dentro de un DTO es como adjuntar una lista de planchas al formulario de la orden. Proyectar un DTO anidado es como incluir un formulario de detalle dentro del formulario principal. SelectMany es como aplanar varias listas en una sola. GroupBy con agregaciones es como pedir un resumen por cliente con totales. Así funcionan las proyecciones a DTOs en EF Core: se seleccionan solo los datos necesarios, se estructuran en formularios estandarizados y se transfieren entre capas de forma eficiente.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class AgregacionesUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public AgregacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

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

Línea 22: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 23: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<AgregacionesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.5

#### Diagnóstico técnico

Para agregaciones sobre conjuntos potencialmente vacíos, el repositorio proyecta a `decimal?` y aplica `?? 0m`. `Any()` expresa existencia y no debe sustituirse mecánicamente por `Count() > 0`.

#### Reto resuelto y verificación adicional

Calcula cardinalidades y peso total/promedio/mínimo/máximo y revisa que el SQL use agregados del servidor. Después agrupa por cliente y estado y contrasta los resultados con el dataset determinista.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Average sobre colección vacía | Una sobrecarga no anulable puede fallar; una anulable puede devolver null | En AceriaData proyectar a `decimal?` y aplicar `?? 0m` |
| Min o Max sobre colección vacía | El resultado no anulable no tiene un valor válido que devolver | Proyectar a nullable y definir el valor del contrato, o comprobar existencia |
| GroupBy sin agregación | Se intenta tratar cualquier agrupación como `GROUP BY` SQL | Usar claves + agregados para la traducción relacional o revisar el *shape* real |
| HAVING mal expresado | El filtro agregado se aplica a las filas antes de agrupar | Aplicar `Where` sobre el agrupamiento para expresar el filtro de grupos |
| Count después de ToList | Se transfirieron filas solo para contarlas después en memoria | Si solo se necesita el total, llamar a `Count()` directamente sobre `IQueryable` |
| Agregación vacía | No se definió la semántica del resultado | Usar nullable + `??` cuando el contrato de AceriaData requiera un valor por defecto |
| Any con Count() > 0 | Se usa Count en lugar de Any | Usar Any para mejor rendimiento |

#### Analogía operativa

Las consultas de agregación en una acería son como los informes que el jefe de planta pide al archivo central. En lugar de revisar todas las carpetas una por una, el jefe pide un resumen: cuántas órdenes hay, cuánto pesan las planchas, cuál es la fecha más antigua. El archivo central calcula el resumen y lo entrega. Count es como contar las carpetas. Sum es como sumar los pesos. Average es como calcular el promedio. Min y Max son como encontrar el más ligero y el más pesado. GroupBy es como agrupar las carpetas por cliente o por estado. El filtro Having es como pedir solo los grupos que cumplen una condición. Las agregaciones sobre colecciones vacías son como pedir un promedio cuando no hay datos: hay que tener cuidado porque puede dar error. `DefaultIfEmpty` introduce un elemento por defecto en una secuencia vacía; el valor concreto y su traducción deben analizarse según el tipo y el proveedor. Así funcionan las agregaciones en EF Core: se calculan en el servidor, se devuelven resúmenes y se evita transferir todas las filas.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class AgrupacionesUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public AgrupacionesUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== AGRUPACIONES CON PROYECCIÓN ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var porCliente = _unidad.Ordenes.ObtenerResumenPorClienteConOrdenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var porClienteEstado = _unidad.Ordenes.ObtenerResumenPorClienteYEstado();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var having = _unidad.Ordenes.ObtenerResumenPorClienteYEstadoConFiltro();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var mensual = _unidad.Ordenes.ObtenerResumenMensualConOrdenes();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `var norte = porCliente.Single(x => x.Cliente == "Constructora del Norte");` → Localiza el único resumen de Constructora del Norte para validar el número de órdenes y la colección interna proyectada.

Línea 18: `if (norte.TotalOrdenes != 3 || norte.Ordenes.Count != 3 || porClienteEstado.Count != 4 || having.Count != 1 || mensual.Count != 5) throw new InvalidOperationException("Agrupaciones inesperadas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 19: `Console.WriteLine($"Norte: {norte.TotalOrdenes} órdenes | HAVING: {having.Count} grupo");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlAgrupacionClienteEstado());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 22: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<AgrupacionesUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.6

#### Diagnóstico técnico

Una colección interna dentro de `GroupBy` no implica automáticamente N+1. El checkpoint usa una consulta de cabeceras agrupadas y otra de filas proyectadas, y compone en memoria con un número fijo de consultas. `Where(g => g.Count() > 1)` debe observarse como `HAVING`.

#### Reto resuelto y verificación adicional

Comprueba tres órdenes para Constructora del Norte y un único grupo cliente-estado con más de una orden. `ObtenerSqlAgrupacionClienteEstado()` debe contener un `GROUP BY` real.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Colección interna agrupada | Se presupuso una traducción SQL o N+1 sin medirla | Inspeccionar la traducción o usar la estrategia acotada de dos consultas de 3.6 |
| Filtro colocado en la fase equivocada | `Where` antes de `GroupBy` filtra filas; `Where` sobre el grupo filtra grupos | Elegir conscientemente `WHERE` o `HAVING` según lo que se quiera filtrar |
| Proyección relacional de grupo no válida | Se intenta proyectar una columna que no forma parte de la clave ni de un agregado en un `GROUP BY` SQL | Usar la clave/agregados o cambiar el *shape* y comprobar su traducción |
| N+1 asumido en agrupaciones | Se atribuye N+1 a cualquier colección interna sin medir consultas | Inspeccionar la traducción real y usar una estrategia acotada como la de 3.6 |
| SelectMany sin proyección | Se aplana sin proyectar | Añadir una proyección clara |
| Agrupación anidada | Se presupuso una traducción o número de consultas fijo | Revisar el *shape* y hacer explícita la frontera cliente/servidor si hace falta |

#### Analogía operativa

Las agrupaciones con proyección en una acería son como los informes agrupados que el jefe de planta pide al archivo central. En lugar de una lista plana de órdenes, el jefe pide un informe agrupado por cliente: para cada cliente, cuántas órdenes tiene y cuáles son. El archivo central agrupa las carpetas por cliente, cuenta las órdenes y prepara una lista con los números de cada orden. Proyectar una colección interna dentro de un grupo es como adjuntar la lista de órdenes al informe del cliente. El filtro Having es como pedir solo los clientes que tienen más de una orden. La agrupación por múltiples claves es como agrupar por cliente y estado a la vez. SelectMany es como aplanar los grupos en una lista plana. Así funcionan las agrupaciones con proyección en EF Core: se agrupan los datos en el servidor, se proyectan los resúmenes y se adjuntan las colecciones internas cuando es necesario. El resultado es un informe estructurado y eficiente.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class JoinsUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public JoinsUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== JOINS Y NAVEGACIÓN ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var inner = _unidad.Ordenes.ObtenerJoinOrdenesPlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var left = _unidad.Ordenes.ObtenerLeftJoinOrdenesPlanchas();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var detalle = _unidad.Ordenes.ObtenerOrdenesConDetalleJoin();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleaciones();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (inner.Count != 5 || left.Count != 6 || detalle.Count != 5 || aleaciones.Count != 5) throw new InvalidOperationException("Joins inesperados.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `if (!left.Any(x => x.NumeroOrden == "OF-2024-0004" && x.Peso is null)) throw new InvalidOperationException("LEFT JOIN no conservó la orden sin planchas.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 19: `Console.WriteLine($"INNER filas: {inner.Count} | LEFT filas: {left.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlJoinExplicito());` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 22: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<JoinsUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.7

#### Diagnóstico técnico

`Join` modela coincidencias; el patrón de left join conserva la fila exterior y exige nulabilidad en la proyección interior. Con varias relaciones uno-a-muchos puede multiplicarse el número de filas: no son duplicados accidentales, sino el shape relacional.

#### Reto resuelto y verificación adicional

Compara `ObtenerJoinOrdenesPlanchas()` y `ObtenerLeftJoinOrdenesPlanchas()`: la orden sin planchas desaparece del INNER JOIN y permanece en el LEFT JOIN con datos relacionados nulos. Revisa la condición por `OrdenId` en el SQL.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| NullReferenceException en detalle | No se comprobó si el detalle es null | Usar operador ternario antes de proyectar |
| Se perdieron filas sin relación | En el patrón correlacionado se aplanó la colección sin `DefaultIfEmpty()` | Usar el patrón de `LEFT JOIN` con `DefaultIfEmpty()` cuando deban conservarse las entidades exteriores |
| Multiplicación de filas | Se cargaron colecciones hermanas con `JOIN` al mismo nivel | Evaluar proyección o `AsSplitQuery()` según el *shape* |
| ThenInclude sin Include | Se usó ThenInclude sin Include previo | Usar Include antes de ThenInclude |
| GroupJoin con forma no traducible | Se devolvió directamente la agrupación sin respetar un patrón soportado | Para un `LEFT JOIN`, aplanar inmediatamente con `DefaultIfEmpty()` y comprobar la traducción |
| Proyección de colección no validada | Se asumió que añadir `ToList()` determina la traducción o elimina N+1 | Inspeccionar la traducción y reformular el *shape*; `ToList()` no es una cura universal para N+1 |

#### Analogía operativa

Los joins en una acería son como las consultas que el jefe de planta hace al archivo central para combinar información de varias carpetas. En lugar de mirar la carpeta de órdenes, la carpeta de planchas y la carpeta de aleaciones por separado, el jefe pide un informe que combine las tres. El `Join` explícito es como indicar manualmente cómo se relacionan las carpetas; el patrón `GroupJoin` + `DefaultIfEmpty` permite conservar la carpeta principal aunque no exista una relacionada. Las propiedades de navegación permiten expresar relaciones sin escribir el join manualmente. En EF Core, la forma SQL concreta depende del patrón: puede aparecer un `JOIN`, una subconsulta u otra construcción, y la carga relacionada con `AsSplitQuery()` puede dividirse en varias sentencias coordinadas. Por eso el laboratorio inspecciona el SQL real en lugar de asumir una única forma.

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

Los elementos trazados en este estado son: `ObtenerOrdenesConPlanchasInclude`, `ObtenerOrdenesConAleacionesInclude`, `ObtenerOrdenesConPlanchasPesadasInclude`, `ObtenerOrdenesConPlanchasYDetalleSplitQuery`, `ObtenerSqlInclude`, `AutoInclude`, `ObtenerOrdenesAutoInclude` e `IgnoreAutoIncludes`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

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
        var auto = _unidad.Ordenes.ObtenerOrdenesAutoInclude();
        var sinAuto = _unidad.Ordenes.ObtenerOrdenesIgnorandoAutoInclude();

        if (planchas.Count != 5 || detalle.Count != 5 || aleaciones.Count != 5 || split.Count != 5)
            throw new InvalidOperationException("Carga Eager inesperada.");
        if (filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 1)
            throw new InvalidOperationException("Filtered Include inesperado.");
        if (auto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 2)
            throw new InvalidOperationException("AutoInclude no cargó Planchas.");
        if (sinAuto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 0)
            throw new InvalidOperationException("IgnoreAutoIncludes no suprimió la carga automática.");
        if (split.Sum(o => o.Planchas.Count) != 5 || split.Sum(o => o.OrdenesAleaciones.Count) != 4)
            throw new InvalidOperationException("SplitQuery no materializó las dos colecciones esperadas.");

        Console.WriteLine($"Include: {planchas.Count} órdenes | SplitQuery: {split.Count} | AutoInclude: {auto.Count} | IgnoreAutoIncludes: {sinAuto.Count}");
        Console.WriteLine(_unidad.Ordenes.ObtenerSqlInclude());
    }
}
```

#### Explicación línea a línea del caso de uso 3.8

Línea 1: `using AceriaData.Application.Interfaces;` → Importa IUnidadDeTrabajo, el puerto que permite al caso de uso consumir consultas sin conocer AceriaDbContext.

Línea 2: `namespace AceriaData.Application.UseCases;` → Sitúa la clase en el espacio de nombres de casos de uso de Application.

Línea 3: `public sealed class CargaEagerUseCase` → Declara el caso de uso dedicado a comparar las variantes de Eager Loading del checkpoint.

Línea 4: `{` → Abre el bloque de la declaración inmediatamente anterior.

Línea 5: `private readonly IUnidadDeTrabajo _unidad;` → Conserva la unidad de trabajo inyectada para acceder al repositorio de órdenes.

Línea 6: `public CargaEagerUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante DI y la asigna al campo de solo lectura.

Línea 7: `public void Ejecutar()` → Define la operación que ejecutará todas las comprobaciones E2E de carga Eager.

Línea 8: `{` → Abre el bloque de la declaración inmediatamente anterior.

Línea 9: `Console.WriteLine("=== EAGER LOADING ===");` → Muestra la cabecera de la demostración.

Línea 10: `var planchas = _unidad.Ordenes.ObtenerOrdenesConPlanchasInclude();` → Ejecuta Include sobre la colección Planchas.

Línea 11: `var detalle = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleInclude();` → Carga conjuntamente Planchas y la referencia Detalle.

Línea 12: `var aleaciones = _unidad.Ordenes.ObtenerOrdenesConAleacionesInclude();` → Ejecuta Include + ThenInclude sobre OrdenesAleaciones → Aleacion.

Línea 13: `var filtradas = _unidad.Ordenes.ObtenerOrdenesConPlanchasPesadasInclude();` → Ejecuta Filtered Include y conserva las planchas que cumplen el umbral del repositorio.

Línea 14: `var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSplitQuery();` → Ejecuta AsSplitQuery sobre Planchas y OrdenesAleaciones, más Detalle, para evitar expansión cartesiana entre colecciones hermanas.

Línea 15: `var auto = _unidad.Ordenes.ObtenerOrdenesAutoInclude();` → Consulta órdenes sin Include explícito; el modelo debe cargar Planchas automáticamente mediante AutoInclude.

Línea 16: `var sinAuto = _unidad.Ordenes.ObtenerOrdenesIgnorandoAutoInclude();` → Ejecuta la lectura con IgnoreAutoIncludes para demostrar que la carga automática puede suprimirse por consulta.

Línea 17: `if (planchas.Count != 5 || detalle.Count != 5 || aleaciones.Count != 5 || split.Count != 5)` → Comprueba que todas las estrategias devuelven las cinco órdenes visibles del dataset.

Línea 18: `throw new InvalidOperationException("Carga Eager inesperada.");` → Hace fallar el E2E si la forma principal de la carga no coincide con el dataset.

Línea 19: `if (filtradas.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 1)` → Verifica que Filtered Include deja una sola plancha de al menos 300 kg en OF-2024-0001.

Línea 20: `throw new InvalidOperationException("Filtered Include inesperado.");` → Falla si el filtro de la colección incluida no se respeta.

Línea 21: `if (auto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 2)` → Comprueba que AutoInclude cargó las dos planchas de OF-2024-0001 sin Include explícito.

Línea 22: `throw new InvalidOperationException("AutoInclude no cargó Planchas.");` → Falla si la navegación configurada con AutoInclude no fue materializada.

Línea 23: `if (sinAuto.Single(o => o.NumeroOrden == "OF-2024-0001").Planchas.Count != 0)` → Comprueba que IgnoreAutoIncludes deja la colección sin cargar en una consulta AsNoTracking.

Línea 24: `throw new InvalidOperationException("IgnoreAutoIncludes no suprimió la carga automática.");` → Falla si la consulta no logra neutralizar AutoInclude.

Línea 25: `if (split.Sum(o => o.Planchas.Count) != 5 || split.Sum(o => o.OrdenesAleaciones.Count) != 4)` → Valida que SplitQuery materializó las cinco planchas y las cuatro relaciones de aleación del dataset.

Línea 26: `throw new InvalidOperationException("SplitQuery no materializó las dos colecciones esperadas.");` → Falla si alguna de las dos colecciones incluidas queda incompleta.

Línea 27: `Console.WriteLine($"Include: {planchas.Count} órdenes | SplitQuery: {split.Count} | AutoInclude: {auto.Count} | IgnoreAutoIncludes: {sinAuto.Count}");` → Resume en consola los tamaños obtenidos por las estrategias validadas.

Línea 28: `Console.WriteLine(_unidad.Ordenes.ObtenerSqlInclude());` → Imprime el SQL generado para la consulta Include de referencia.

Línea 29: `}` → Cierra el bloque de la declaración inmediatamente anterior.

Línea 30: `}` → Cierra el bloque de la declaración inmediatamente anterior.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<CargaEagerUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.8

#### Diagnóstico técnico

El checkpoint ejecuta `Include`, `ThenInclude`, Filtered Include, `AsSplitQuery()`, `AutoInclude()` e `IgnoreAutoIncludes()`. El split carga dos colecciones hermanas —Planchas y OrdenesAleaciones— más Detalle, de modo que la decisión responde a un riesgo real de multiplicación de filas.

#### Reto resuelto y verificación adicional

Valida OF-2024-0001 con dos planchas mediante AutoInclude, cero planchas usando `IgnoreAutoIncludes + AsNoTracking`, una sola plancha en Filtered Include y, en SplitQuery, cinco planchas y cuatro relaciones de aleación en todo el dataset.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Multiplicación de filas | Se cargaron colecciones hermanas al mismo nivel en modo single-query | Comparar `AsSingleQuery()` y `AsSplitQuery()` para el volumen real |
| N+1 | Se accedió repetidamente a navegaciones Lazy no cargadas | Planificar Eager Loading, proyección o Explicit Loading según el caso |
| ThenInclude sin Include | Se usó ThenInclude sin Include previo | Usar Include antes de ThenInclude |
| NullReferenceException en referencia | La propiedad de navegación es null | Comprobar si la propiedad es null |
| Filtro no aplicado | Se aplicó el filtro fuera del Include | Aplicar el filtro dentro del Include |
| AutoInclude excesivo | Se configuró en muchas propiedades | Usar con moderación |

#### Analogía operativa

La carga Eager en una acería es como pedirle al archivo central que, además de la carpeta de la orden, adjunte también las carpetas de las planchas y del detalle. En lugar de pedir la carpeta de la orden y después ir a buscar las planchas y el detalle por separado, se pide todo junto. El archivo central prepara un paquete con la orden y sus documentos relacionados. Include es como pedir que se adjunte una carpeta. ThenInclude es como pedir que se adjunten los documentos de la carpeta adjunta. Filtered Include es como pedir que solo se adjunten las planchas activas. AsSplitQuery es como pedir que el paquete se prepare en varios envíos separados para evitar que el paquete sea demasiado grande. AutoInclude es como pedir que siempre se adjunten las planchas, sin tener que pedirlo cada vez. Así funciona la carga Eager en EF Core: las relaciones necesarias se solicitan de forma planificada como parte de la operación inicial, que puede resolverse con una consulta única o con varias consultas coordinadas mediante `AsSplitQuery()`.

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

Los elementos trazados en este estado son: `ObtenerTodasSinInclude`, `IgnoreAutoIncludes`, `UseLazyLoadingProxies`, `virtual`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class CargaLazyUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public CargaLazyUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== LAZY LOADING (DEMOSTRACIÓN) ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var ordenes = _unidad.Ordenes.ObtenerTodasSinInclude();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var totalPlanchas = 0;` → Inicializa el acumulador que permitirá comprobar cuántas planchas se cargan al acceder a las navegaciones Lazy.

Línea 15: `foreach (var orden in ordenes)` → Recorre los resultados para evaluar el comportamiento de cada entidad o relación.

Línea 16: `totalPlanchas += orden.Planchas.Count;` → Accede a la navegación virtual Planchas de cada orden y acumula su tamaño; en 3.9 este acceso es precisamente el que puede disparar una consulta Lazy por entidad.

Línea 17: `if (ordenes.Count != 5 || totalPlanchas != 5) throw new InvalidOperationException("Lazy Loading no cargó las relaciones.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Órdenes: {ordenes.Count} | Planchas accedidas bajo demanda: {totalPlanchas}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine("Advertencia docente: el acceso dentro de un bucle puede producir N+1 consultas.");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<CargaLazyUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.9

#### Diagnóstico técnico

Lazy Loading necesita Proxies, `UseLazyLoadingProxies()` y navegaciones virtuales, además de un DbContext vivo. Como 3.8 introdujo AutoInclude, `ObtenerTodasSinInclude()` usa `IgnoreAutoIncludes()` para que la navegación se cargue realmente bajo demanda.

#### Reto resuelto y verificación adicional

Recorre las cinco órdenes y accede a `orden.Planchas.Count`; el total esperado es cinco. Contrasta este acceso con Eager Loading y explica por qué el patrón puede producir N+1 al crecer N.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Proxy Lazy no intercepta una navegación | La navegación que se quiere cargar bajo demanda no puede ser sobrescrita por el proxy | En el modelo basado en proxies, declarar `virtual` las navegaciones destinadas a Lazy Loading |
| Carga Lazy no funciona | No se configuró UseLazyLoadingProxies | Configurar en OnConfiguring |
| N+1 en bucle | Con Lazy Loading se accede repetidamente a navegaciones todavía no cargadas | Planificar Eager Loading, proyección o Explicit Loading según el caso |
| ObjectDisposedException | Se accede a propiedades de navegación fuera del ámbito | Usar Include o DTOs |
| Referencia circular en serialización | Las entidades se referencian mutuamente | Usar DTOs o configurar el serializador |
| NullReferenceException | La propiedad de navegación es null | Comprobar antes de acceder |
| Proxies no generados | Las clases son sealed | Quitar sealed |

#### Analogía operativa

La carga Lazy en una acería es como pedirle al archivo central que no te traiga las carpetas relacionadas hasta que las pidas. En lugar de recibir la carpeta de la orden con las planchas y el detalle adjuntos, recibes solo la carpeta de la orden. Si después necesitas las planchas, el archivo central te las trae en ese momento. Si necesitas el detalle, te lo trae después. Cada petición adicional es un viaje al archivo central. Si tienes cien órdenes y pides las planchas de cada una, haces cien viajes. Eso es el problema N+1. La carga Eager es como pedir todas las carpetas relacionadas de una vez: un solo viaje con todo lo necesario. La carga Lazy es cómoda porque no tienes que decidir de antemano qué necesitas, pero es ineficiente si haces muchas peticiones. En una acería con mucho volumen, la carga Eager es la opción correcta. En un prototipo o en una aplicación pequeña, la carga Lazy puede ser aceptable. Así funciona la carga Lazy en EF Core: cómoda pero peligrosa, útil en prototipos pero ineficiente en producción.

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

Los elementos trazados en este estado son: `ObtenerConCargaExplicita`, `ObtenerConPlanchasPesadasExplicitas`, `IgnoreAutoIncludes`, `IsLoaded`, `Query().Where`. El contrato crece de forma acumulativa, salvo en 3.12, donde el seam docente `IQueryable` se retira del puerto público y la composición queda encapsulada en Infrastructure.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class CargaExplicitaUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public CargaExplicitaUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== EXPLICIT LOADING ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var orden = _unidad.Ordenes.ObtenerConCargaExplicita("OF-2024-0001");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var filtrada = _unidad.Ordenes.ObtenerConPlanchasPesadasExplicitas("OF-2024-0002", 300m);` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `if (orden is null || orden.Planchas.Count != 2 || orden.Detalle is null) throw new InvalidOperationException("Carga explícita incompleta.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 16: `if (filtrada is null || filtrada.Planchas.Count != 0) throw new InvalidOperationException("Query() de carga explícita inesperado.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 17: `Console.WriteLine($"Carga completa: {orden.Planchas.Count} planchas | Filtrada OF-0002: {filtrada.Planchas.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 18: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 19: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<CargaExplicitaUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.10

#### Diagnóstico técnico

Explicit Loading parte de una entidad rastreada y decide cuándo ejecutar `Collection().Load()` o `Reference().Load()`. `IsLoaded` evita repetir una carga; `Query()` permite filtrar antes de `Load()`. Las consultas raíz neutralizan AutoInclude para no falsear la demostración.

#### Reto resuelto y verificación adicional

Carga `OF-2024-0001`: espera dos planchas y detalle. Después carga las planchas de `OF-2024-0002` con peso mínimo 300 kg: espera colección vacía. Así se demuestra que `Query()` altera la consulta de navegación antes de materializar.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Estado de carga no entendido | Se desconoce si la navegación está completa | Consultar `IsLoaded` para explicitar el estado; si ya es `true`, `Load()` es un no-op |
| Filtro no aplicado | Se aplicó el filtro fuera del Query | Aplicar el filtro dentro del Query |
| NullReferenceException | La entidad principal es null | Comprobar antes de llamar a Load |
| Referencia circular | Se carga una entidad que referencia a la principal | Usar DTOs o evitar cargar la referencia inversa |
| Muchas consultas explícitas | Se llama a `Load()` para cada entidad dentro de un bucle | Si todas las relaciones se necesitan, valorar Eager Loading o una proyección; si la carga es condicional, medir el coste |
| Estrategia de carga difícil de razonar | Se mezclan Lazy y Explicit sin intención documentada | Definir qué navegación usa cada estrategia y evitar cargas implícitas inesperadas |

#### Analogía operativa

La carga Explicit en una acería es como pedirle al archivo central que traiga una carpeta relacionada solo cuando se decide que hace falta. `IsLoaded` es como comprobar si ya se dispone de esa carpeta antes de pedirla de nuevo y `Query()` permite pedir solo los documentos que cumplen una condición. Si las relaciones necesarias se conocen desde el principio, Eager Loading o una proyección pueden reducir viajes al archivo; si la necesidad aparece de forma condicional o se quiere filtrar una navegación concreta, Explicit Loading puede resultar más apropiada. No existe una opción universalmente más eficiente: se comparan número de consultas, volumen transferido y claridad del flujo.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class ComposicionConsultasUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public ComposicionConsultasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== COMPOSICIÓN DE CONSULTAS ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var resultado = _unidad.Ordenes.BuscarOrdenes("Constructora del Norte", "Pendiente", new DateTime(2024,1,1), "fecha", true, 1, 10);` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `if (resultado.Elementos.Count != 2) throw new InvalidOperationException("Consulta compuesta inesperada.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 15: `Console.WriteLine($"Resultados: {resultado.Elementos.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 16: `Console.WriteLine(resultado.Sql);` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 17: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 18: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<ComposicionConsultasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.11

#### Diagnóstico técnico

La composición flexible permanece dentro de Infrastructure. Filtros opcionales, orden dinámico y paginación se encadenan sin materializar. `Skip`/`Take` debe aplicarse tras un orden determinista para que las páginas sean repetibles.

#### Reto resuelto y verificación adicional

Ejecuta `BuscarOrdenes` con Norte + Pendiente, fecha descendente, página 1 y tamaño 2. El DTO devuelve resultados y `ToQueryString()` de la misma consulta; los filtros ausentes no deben aparecer en SQL.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| Materialización prematura | Se llamó a ToList antes de aplicar todos los filtros | Materializar solo al final |
| Filtros en memoria | Se materializó la consulta antes de filtrar | Construir la consulta con IQueryable |
| Múltiples consultas | Se materializó varias veces | Materializar una sola vez al final |
| ThenBy sin OrderBy | Se llamó a ThenBy sin OrderBy previo | Llamar a OrderBy primero |
| Skip sin OrderBy | Se aplicó Skip sin ordenar | Aplicar OrderBy antes de Skip |
| Paginación incorrecta | Se calculó mal el Skip | Usar (pagina - 1) * tamanoPagina |

#### Analogía operativa

La composición de consultas en una acería es como construir una orden de búsqueda al archivo central paso a paso. En lugar de pedir todos los documentos y después filtrarlos, el jefe de planta va añadiendo condiciones a la orden: primero el cliente, después el estado, después el rango de fechas. Cada condición se añade a la orden sin enviarla todavía. Solo cuando la orden está completa, el jefe la envía al archivo central. El archivo recibe la orden con todas las condiciones y devuelve solo los documentos que cumplen todas ellas. La ejecución diferida es como preparar la orden sin enviarla: se puede modificar hasta el último momento. La materialización prematura es como enviar la orden antes de terminarla: el archivo devuelve documentos que después hay que filtrar a mano. La composición de consultas permite construir consultas flexibles y eficientes, adaptadas a las necesidades de cada momento.

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

Los elementos trazados en este estado son: `ObtenerResumenesPendientesOptimizado`, `ExisteAlgunaOrdenPendiente`, `ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano`, `ObtenerPorNumeroOptimizado`. El contrato crece de forma acumulativa, salvo en 3.12, donde se retiran dos decisiones docentes: el seam `IQueryable` sale del puerto público y `AutoInclude` deja de ser comportamiento global. La composición queda encapsulada en Infrastructure. El `AsSplitQuery()` final se conserva sobre dos colecciones hermanas —Planchas y OrdenesAleaciones— más Detalle.

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

Línea 1: `using AceriaData.Application.Interfaces;` → Importa los puertos de Application, en especial IUnidadDeTrabajo e IOrdenRepositorio, que desacoplan el caso de uso de EF Core.

Línea 3: `namespace AceriaData.Application.UseCases;` → Declara el espacio de nombres de la capa a la que pertenece el archivo.

Línea 5: `public sealed class BuenasPracticasUseCase` → Declara el caso de uso concreto que coordina la demostración del punto.

Línea 6: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 7: `private readonly IUnidadDeTrabajo _unidad;` → Guarda el puerto de unidad de trabajo que da acceso al repositorio sin depender de DbContext.

Línea 8: `public BuenasPracticasUseCase(IUnidadDeTrabajo unidad) => _unidad = unidad;` → Recibe la unidad de trabajo mediante inyección de dependencias.

Línea 10: `public void Ejecutar()` → Define la operación docente que ejecutará el composition root.

Línea 11: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 12: `Console.WriteLine("=== BUENAS PRÁCTICAS EN EL ACCESO A DATOS ===");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 13: `var resumenes = _unidad.Ordenes.ObtenerResumenesPendientesOptimizado();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 14: `var existe = _unidad.Ordenes.ExisteAlgunaOrdenPendiente();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 15: `var split = _unidad.Ordenes.ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano();` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 16: `var inexistente = _unidad.Ordenes.ObtenerPorNumeroOptimizado("OF-2024-9999");` → Invoca el método de consulta del repositorio correspondiente y conserva su resultado para validarlo.

Línea 17: `if (resumenes.Count != 3 || !existe || split.Count != 5 || inexistente is not null) throw new InvalidOperationException("Buenas prácticas: validación E2E fallida.");` → Comprueba una condición E2E y hace fallar la ejecución si los resultados no coinciden con los datos esperados.

Línea 18: `Console.WriteLine($"Pendientes proyectadas: {resumenes.Count} | Any: {existe} | SplitQuery: {split.Count}");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 19: `Console.WriteLine("El contrato final ya no expone IQueryable fuera de Infrastructure.");` → Escribe una evidencia legible en consola para poder inspeccionar el flujo o el SQL.

Línea 20: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

Línea 21: `}` → Cierra el bloque sintáctico abierto previamente y termina el ámbito correspondiente.

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

Línea 1: `using AceriaData.Application.UseCases;` → Importa los casos de uso de Application para poder registrarlos y resolverlos desde el composition root.

Línea 2: `using AceriaData.ConsoleApp;` → Importa DemoData, responsable de sembrar el conjunto determinista usado por las pruebas E2E del checkpoint.

Línea 3: `using AceriaData.Infrastructure;` → Importa la extensión AddAceriaInfrastructure, que registra DbContext, repositorio y unidad de trabajo.

Línea 4: `using AceriaData.Infrastructure.Persistence;` → Importa AceriaDbContext para preparar la base de demostración desde el composition root.

Línea 5: `using Microsoft.EntityFrameworkCore;` → Importa las extensiones de EF Core utilizadas por Migrate, ChangeTracker y las operaciones de consulta del checkpoint.

Línea 6: `using Microsoft.Extensions.Configuration;` → Importa ConfigurationBuilder y las extensiones necesarias para cargar appsettings.json y variables de entorno.

Línea 7: `using Microsoft.Extensions.DependencyInjection;` → Importa ServiceCollection, AddScoped, CreateScope y GetRequiredService para componer el grafo de dependencias.

Línea 9: `var configuration = new ConfigurationBuilder()` → Inicia la construcción de la configuración de la aplicación.

Línea 10: `.SetBasePath(AppContext.BaseDirectory)` → Fija como base el directorio de ejecución para localizar appsettings.json.

Línea 11: `.AddJsonFile("appsettings.json", optional: false)` → Carga la configuración JSON obligatoria que contiene la cadena de conexión.

Línea 12: `.AddEnvironmentVariables()` → Permite sobrescribir configuración mediante variables de entorno.

Línea 13: `.Build();` → Finaliza la construcción y devuelve la configuración inmutable que se usará para leer ConnectionStrings:AceriaDB.

Línea 15: `var connectionString = configuration.GetConnectionString("AceriaDB")` → Obtiene la cadena AceriaDB desde la configuración.

Línea 16: `?? throw new InvalidOperationException("Falta ConnectionStrings:AceriaDB");` → Detiene el arranque con un error explícito si no existe la cadena de conexión AceriaDB, evitando ejecutar el ejercicio con una configuración incompleta.

Línea 18: `var services = new ServiceCollection();` → Crea el contenedor de servicios del composition root.

Línea 19: `services.AddAceriaInfrastructure(connectionString);` → Registra DbContext, repositorio y unidad de trabajo de Infrastructure.

Línea 20: `services.AddScoped<BuenasPracticasUseCase>();` → Registra el caso de uso con ciclo de vida Scoped para compartir el mismo ámbito que DbContext.

Línea 22: `using var provider = services.BuildServiceProvider(new ServiceProviderOptions` → Construye el proveedor de servicios y lo declara con using para liberar correctamente los servicios IDisposable al terminar el proceso.

Línea 23: `{` → Abre el bloque de la clase, método, inicializador u opción declarada inmediatamente antes; su cierre delimita exactamente ese ámbito.

Línea 24: `ValidateOnBuild = true,` → Obliga al contenedor a comprobar durante la construcción que los servicios registrados pueden resolverse.

Línea 25: `ValidateScopes = true` → Activa la validación de ciclos de vida para detectar, por ejemplo, un servicio Scoped consumido incorrectamente desde un Singleton.

Línea 26: `});` → Cierra las opciones y completa la llamada que construye el proveedor de servicios.

Línea 27: `using var scope = provider.CreateScope();` → Crea el ámbito Scoped que compartirá AceriaDbContext, IUnidadDeTrabajo y el caso de uso durante esta ejecución.

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

### Laboratorio adicional del punto 3.12

#### Diagnóstico técnico

El cierre conserva `AsNoTracking`, proyecciones, `Any`, `FirstOrDefault` y SplitQuery cuando el shape lo justifica. Retira `IQueryable` del puerto y `AutoInclude` del modelo para evitar decisiones implícitas en Application.

#### Reto resuelto y verificación adicional

Valida tres pendientes proyectadas, `Any == true`, cinco órdenes en la consulta SplitQuery y `null` para `OF-2024-9999`. El SplitQuery final carga Planchas, OrdenesAleaciones→Aleacion y Detalle.

#### Errores comunes revisados

| Error | Causa | Solución |
| --- | --- | --- |
| AsNoTracking en consulta de escritura | Se aplicó AsNoTracking a una consulta que modifica datos | Usar AsNoTracking solo en consultas de solo lectura |
| Count() > 0 en lugar de Any | Se usó Count para saber si hay elementos | Usar Any |
| First en lugar de FirstOrDefault | Se usó First cuando puede no haber resultados | Usar FirstOrDefault |
| Multiplicación de filas | Se cargaron colecciones hermanas al mismo nivel en modo single-query | Comparar `AsSingleQuery()` y `AsSplitQuery()` para el volumen real |
| Materialización prematura | Se llamó a ToList antes de aplicar todos los filtros | Materializar solo al final |
| N+1 | Se accede en un bucle a navegaciones Lazy todavía no cargadas | Elegir de forma explícita Eager Loading, proyección o Explicit Loading |
| Falta de documentación | No se documentaron las decisiones | Añadir comentarios XML |

#### Analogía operativa

Las buenas prácticas de acceso a datos en una acería son como las normas de seguridad y eficiencia de la planta. No se trata solo de producir acero, sino de producirlo de forma segura, eficiente y sostenible. Usar proyecciones es como pedir solo los datos que se necesitan: no se pide la carpeta completa si solo se necesita el número de orden. Usar AsNoTracking es como no registrar cada plancha en el libro de producción si solo se va a consultar. Usar Any es como comprobar si hay planchas en el almacén sin contarlas todas. Usar FirstOrDefault es como buscar una plancha por su número y devolver null si no existe, en lugar de lanzar una alarma. Usar AsSplitQuery es como dividir una orden de búsqueda grande en varias más pequeñas para evitar un envío masivo. Documentar las decisiones es como escribir las normas en el manual de la planta para que todos las conozcan. Así funcionan las buenas prácticas en EF Core: producen código más eficiente, más mantenible y más predecible.

### Paso 10: Reto resuelto y conexión con el siguiente punto

**Reto:** Revisa IOrdenRepositorio y justifica por qué ya no contiene IQueryable aunque Infrastructure siga componiendo consultas.

La solución está representada por el estado acumulativo del propio checkpoint y sus métodos de repositorio. Tras validar `3.12 OK`, el siguiente estado parte exactamente de esta solución; con ello queda cerrado el Módulo 3.
