# Curso Profesional de Entity Framework Core 8

# Módulo 5 — Prácticas: Persistencia empresarial, concurrencia, transacciones, despliegue, testing y buenas prácticas

**Autor: JAIME GALLO**

Cada práctica trabaja sobre un estado completo y ejecutable de AceriaData. La secuencia es acumulativa desde `M04/PROYECTO/4.12`: cada punto conserva todo lo incorporado anteriormente y añade únicamente lo necesario para el nuevo contenido.

> Entorno de trabajo: .NET 8 · C# 12 · Entity Framework Core 8 · SQL Server Express LocalDB · Visual Studio Community · PowerShell para los scripts de despliegue.

## Punto 5.1 — Concurrencia optimista: concepto y necesidad

### Contexto del proyecto

Este punto continúa directamente `M04/PROYECTO/4.12`. La carpeta `M05/PROYECTO/5.1` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo no cambia todavía; se conserva la historia heredada del Módulo 4.

### Objetivo práctico

Reproducir una actualización perdida real sobre la misma propiedad y contrastarla con cambios concurrentes sobre propiedades distintas.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.1
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Repositories/ConcurrenciaOptimistaM5Repositorio.cs`
- `src/AceriaData.Application/ConcurrenciaOptimistaM5UseCase.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `ConcurrenciaOptimistaM5Repositorio.cs`

```csharp
public ConcurrenciaMismaPropiedadDto DemostrarActualizacionPerdidaMismaPropiedad()
{
    RestaurarEstadoInicial();

    using var scopeA = _scopeFactory.CreateScope();
    using var scopeB = _scopeFactory.CreateScope();

    var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

    SqlCommandCounterInterceptor.Instance.Reset();

    var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
    var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);

    var clienteInicial = ordenA.Cliente;

    ordenA.Cliente = "Cliente actualizado por A";
    contextA.SaveChanges();

    ordenB.Cliente = "Cliente actualizado por B";
    contextB.SaveChanges();

    using var scopeVerificacion = _scopeFactory.CreateScope();
    var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var clienteFinal = contextVerificacion.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.NumeroOrden == NumeroOrden)
        .Select(o => o.Cliente)
        .Single();

    return new ConcurrenciaMismaPropiedadDto(
        clienteInicial,
        ordenA.Cliente,
        ordenB.Cliente,
        clienteFinal,
        clienteFinal == ordenB.Cliente && clienteFinal != ordenA.Cliente,
        SqlCommandCounterInterceptor.Instance.SnapshotCommands());
}
```

#### Explicación línea a línea — ConcurrenciaOptimistaM5Repositorio.cs

Línea 1: `public ConcurrenciaMismaPropiedadDto DemostrarActualizacionPerdidaMismaPropiedad()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `RestaurarEstadoInicial();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 5: `using var scopeA = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 6: `using var scopeB = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 8: `var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 9: `var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 11: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 13: `var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 14: `var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 16: `var clienteInicial = ordenA.Cliente;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 18: `ordenA.Cliente = "Cliente actualizado por A";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 19: `contextA.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 21: `ordenB.Cliente = "Cliente actualizado por B";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 22: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 24: `using var scopeVerificacion = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 25: `var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 26: `var clienteFinal = contextVerificacion.OrdenesFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 27: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 28: `.Where(o => o.NumeroOrden == NumeroOrden)` → Añade el predicado a la consulta; EF Core intentará traducirlo al proveedor antes de materializar.

Línea 29: `.Select(o => o.Cliente)` → Proyecta únicamente la forma de datos necesaria para el resultado.

Línea 30: `.Single();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 32: `return new ConcurrenciaMismaPropiedadDto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 33: `clienteInicial,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 34: `ordenA.Cliente,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 35: `ordenB.Cliente,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 36: `clienteFinal,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 37: `clienteFinal == ordenB.Cliente && clienteFinal != ordenA.Cliente,` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 38: `SqlCommandCounterInterceptor.Instance.SnapshotCommands());` → Recupera los comandos SQL realmente interceptados durante la operación.

Línea 39: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `ConcurrenciaOptimistaM5Repositorio.cs`

```csharp
public ConcurrenciaPropiedadesDistintasDto DemostrarCambiosEnPropiedadesDistintas()
{
    RestaurarEstadoInicial();
    using var scopeA = _scopeFactory.CreateScope();
    using var scopeB = _scopeFactory.CreateScope();
    var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

    var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
    var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);

    ordenA.Cliente = "Cliente actualizado por A";
    contextA.SaveChanges();
    ordenB.Estado = "EnProceso";
    contextB.SaveChanges();

    using var scopeVerificacion = _scopeFactory.CreateScope();
    var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var resultado = contextVerificacion.OrdenesFabricacion
        .AsNoTracking()
        .Where(o => o.NumeroOrden == NumeroOrden)
        .Select(o => new { o.Cliente, o.Estado })
        .Single();

    return new ConcurrenciaPropiedadesDistintasDto(
        ClienteInicial, EstadoInicial, resultado.Cliente, resultado.Estado,
        resultado.Cliente == ordenA.Cliente && resultado.Estado == ordenB.Estado,
        SqlCommandCounterInterceptor.Instance.SnapshotCommands());
}
```

#### Explicación línea a línea — ConcurrenciaOptimistaM5Repositorio.cs

Línea 1: `public ConcurrenciaPropiedadesDistintasDto DemostrarCambiosEnPropiedadesDistintas()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `RestaurarEstadoInicial();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `using var scopeA = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 5: `using var scopeB = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 6: `var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 7: `var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 9: `var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 10: `var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 12: `ordenA.Cliente = "Cliente actualizado por A";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 13: `contextA.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 14: `ordenB.Estado = "EnProceso";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 17: `using var scopeVerificacion = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 18: `var contextVerificacion = scopeVerificacion.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 19: `var resultado = contextVerificacion.OrdenesFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 20: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 21: `.Where(o => o.NumeroOrden == NumeroOrden)` → Añade el predicado a la consulta; EF Core intentará traducirlo al proveedor antes de materializar.

Línea 22: `.Select(o => new { o.Cliente, o.Estado })` → Proyecta únicamente la forma de datos necesaria para el resultado.

Línea 23: `.Single();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 25: `return new ConcurrenciaPropiedadesDistintasDto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 26: `ClienteInicial, EstadoInicial, resultado.Cliente, resultado.Estado,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 27: `resultado.Cliente == ordenA.Cliente && resultado.Estado == ordenB.Estado,` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 28: `SqlCommandCounterInterceptor.Instance.SnapshotCommands());` → Recupera los comandos SQL realmente interceptados durante la operación.

Línea 29: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.1 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- Dos DbContext independientes cargan la misma orden antes de guardar.
- La misma propiedad permite observar una actualización perdida sin token.
- Propiedades distintas se conservan cuando EF Core genera UPDATE de las propiedades modificadas.
- La verificación final usa un tercer contexto AsNoTracking.
- El SQL mostrado procede del interceptor real.

### Reto resuelto y ampliación

**Reto:** Añade una tercera escritura concurrente sobre Cliente y comprueba, mediante el SQL capturado, qué valor queda finalmente sin token.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Usar el mismo DbContext para ambos actores y creer que se reproduce concurrencia.
- Modificar propiedades distintas y afirmar que necesariamente existe actualización perdida.
- Leer el resultado final desde un contexto con estado antiguo en el ChangeTracker.
- Inventar un SQL esperado en vez de observar el comando ejecutado.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.1 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.1, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.1 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.2` y parte directamente de este proyecto completo.

---

## Punto 5.2 — Configuración de tokens de concurrencia

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.1`. La carpeta `M05/PROYECTO/5.2` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. Este punto introduce la migración real `M5_5_2_ConcurrencyTokens`.

### Objetivo práctico

Configurar rowversion y un token de propiedad, generar la migración real y demostrar el conflicto sobre SQL Server LocalDB.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.2
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Persistence/Configurations/ConcurrencyTokensConfiguration.cs`
- `src/AceriaData.Infrastructure/Repositories/TokensConcurrenciaM5Repositorio.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `ConcurrencyTokensConfiguration.cs`

```csharp
public sealed class OrdenFabricacionConcurrencyConfiguration : IEntityTypeConfiguration<OrdenFabricacion>
{
    public void Configure(EntityTypeBuilder<OrdenFabricacion> b) =>
        b.Property(x => x.RowVersion).IsRowVersion();
}

public sealed class PlanchaAceroConcurrencyConfiguration : IEntityTypeConfiguration<PlanchaAcero>
{
    public void Configure(EntityTypeBuilder<PlanchaAcero> b) =>
        b.Property(x => x.RowVersion).IsRowVersion();
}

public sealed class AleacionConcurrencyConfiguration : IEntityTypeConfiguration<Aleacion>
{
    public void Configure(EntityTypeBuilder<Aleacion> b) =>
        b.Property(x => x.RowVersion).IsRowVersion();
}

public sealed class DetalleOrdenConcurrencyConfiguration : IEntityTypeConfiguration<DetalleOrden>
{
    public void Configure(EntityTypeBuilder<DetalleOrden> b)
    {
        b.Property(x => x.EstadoDetalle)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pendiente")
            .IsConcurrencyToken();
    }
}
```

#### Explicación línea a línea — ConcurrencyTokensConfiguration.cs

Línea 1: `public sealed class OrdenFabricacionConcurrencyConfiguration : IEntityTypeConfiguration<OrdenFabricacion>` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `public void Configure(EntityTypeBuilder<OrdenFabricacion> b) =>` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 4: `b.Property(x => x.RowVersion).IsRowVersion();` → Configura la propiedad como token rowversion generado por SQL Server y usado por EF Core para concurrencia optimista.

Línea 5: `}` → Delimita el bloque sintáctico correspondiente.

Línea 7: `public sealed class PlanchaAceroConcurrencyConfiguration : IEntityTypeConfiguration<PlanchaAcero>` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 8: `{` → Delimita el bloque sintáctico correspondiente.

Línea 9: `public void Configure(EntityTypeBuilder<PlanchaAcero> b) =>` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 10: `b.Property(x => x.RowVersion).IsRowVersion();` → Configura la propiedad como token rowversion generado por SQL Server y usado por EF Core para concurrencia optimista.

Línea 11: `}` → Delimita el bloque sintáctico correspondiente.

Línea 13: `public sealed class AleacionConcurrencyConfiguration : IEntityTypeConfiguration<Aleacion>` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 14: `{` → Delimita el bloque sintáctico correspondiente.

Línea 15: `public void Configure(EntityTypeBuilder<Aleacion> b) =>` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 16: `b.Property(x => x.RowVersion).IsRowVersion();` → Configura la propiedad como token rowversion generado por SQL Server y usado por EF Core para concurrencia optimista.

Línea 17: `}` → Delimita el bloque sintáctico correspondiente.

Línea 19: `public sealed class DetalleOrdenConcurrencyConfiguration : IEntityTypeConfiguration<DetalleOrden>` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 20: `{` → Delimita el bloque sintáctico correspondiente.

Línea 21: `public void Configure(EntityTypeBuilder<DetalleOrden> b)` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 22: `{` → Delimita el bloque sintáctico correspondiente.

Línea 23: `b.Property(x => x.EstadoDetalle)` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 24: `.IsRequired()` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 25: `.HasMaxLength(50)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 26: `.HasDefaultValue("Pendiente")` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 27: `.IsConcurrencyToken();` → Marca la propiedad existente como token de concurrencia para que su valor original participe en escrituras.

Línea 28: `}` → Delimita el bloque sintáctico correspondiente.

Línea 29: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `TokensConcurrenciaM5Repositorio.cs`

```csharp
public RowVersionM5Dto DemostrarRowVersion()
{
    using var scopeA = _scopeFactory.CreateScope();
    using var scopeB = _scopeFactory.CreateScope();
    var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();

    var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
    var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);
    var inicialA = Convert.ToHexString(ordenA.RowVersion);
    var inicialB = Convert.ToHexString(ordenB.RowVersion);

    SqlCommandCounterInterceptor.Instance.Reset();
    ordenA.Cliente = "Cliente 5.2 A";
    contextA.SaveChanges();
    var despuesA = Convert.ToHexString(ordenA.RowVersion);

    ordenB.Estado = "EnProceso";
    var conflicto = false;
    try
    {
        contextB.SaveChanges();
    }
    catch (DbUpdateConcurrencyException)
    {
        conflicto = true;
    }

    return new RowVersionM5Dto(
        inicialA, inicialB, despuesA, conflicto,
        SqlCommandCounterInterceptor.Instance.SnapshotCommands());
}
```

#### Explicación línea a línea — TokensConcurrenciaM5Repositorio.cs

Línea 1: `public RowVersionM5Dto DemostrarRowVersion()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `using var scopeA = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 4: `using var scopeB = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 5: `var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 6: `var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 8: `var ordenA = contextA.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 9: `var ordenB = contextB.OrdenesFabricacion.Single(o => o.NumeroOrden == NumeroOrden);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 10: `var inicialA = Convert.ToHexString(ordenA.RowVersion);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 11: `var inicialB = Convert.ToHexString(ordenB.RowVersion);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 13: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 14: `ordenA.Cliente = "Cliente 5.2 A";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `contextA.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 16: `var despuesA = Convert.ToHexString(ordenA.RowVersion);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 18: `ordenB.Estado = "EnProceso";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 19: `var conflicto = false;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 20: `try` → Abre el bloque en el que se espera que pueda producirse la excepción que se quiere estudiar.

Línea 21: `{` → Delimita el bloque sintáctico correspondiente.

Línea 22: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 23: `}` → Delimita el bloque sintáctico correspondiente.

Línea 24: `catch (DbUpdateConcurrencyException)` → Captura la excepción específica para aplicar la política de resolución prevista.

Línea 25: `{` → Delimita el bloque sintáctico correspondiente.

Línea 26: `conflicto = true;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 27: `}` → Delimita el bloque sintáctico correspondiente.

Línea 29: `return new RowVersionM5Dto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 30: `inicialA, inicialB, despuesA, conflicto,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 31: `SqlCommandCounterInterceptor.Instance.SnapshotCommands());` → Recupera los comandos SQL realmente interceptados durante la operación.

Línea 32: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Implementar y estudiar `TokensConcurrenciaM5Repositorio.cs`

```csharp
public IndiceRowVersionM5Dto ComprobarIndiceRowVersion()
{
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var connection = context.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
        connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = """
        SELECT COUNT(*)
        FROM sys.indexes AS i
        INNER JOIN sys.index_columns AS ic
            ON i.object_id = ic.object_id AND i.index_id = ic.index_id
        INNER JOIN sys.columns AS c
            ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        WHERE i.object_id = OBJECT_ID(N'dbo.OrdenesFabricacion')
          AND c.name = N'RowVersion';
        """;

    return new IndiceRowVersionM5Dto(Convert.ToInt32(command.ExecuteScalar()) > 0);
}
```

#### Explicación línea a línea — TokensConcurrenciaM5Repositorio.cs

Línea 1: `public IndiceRowVersionM5Dto ComprobarIndiceRowVersion()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `using var scope = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 4: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 5: `var connection = context.Database.GetDbConnection();` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 6: `if (connection.State != ConnectionState.Open)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 7: `connection.Open();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 9: `using var command = connection.CreateCommand();` → Importa el espacio de nombres necesario para los tipos y extensiones usados por el archivo, o declara un recurso con liberación automática cuando se usa `using var`.

Línea 10: `command.CommandText = """` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 11: `SELECT COUNT(*)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 12: `FROM sys.indexes AS i` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 13: `INNER JOIN sys.index_columns AS ic` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 14: `ON i.object_id = ic.object_id AND i.index_id = ic.index_id` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `INNER JOIN sys.columns AS c` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 16: `ON ic.object_id = c.object_id AND ic.column_id = c.column_id` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 17: `WHERE i.object_id = OBJECT_ID(N'dbo.OrdenesFabricacion')` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 18: `AND c.name = N'RowVersion';` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 19: `""";` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 21: `return new IndiceRowVersionM5Dto(Convert.ToInt32(command.ExecuteScalar()) > 0);` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 22: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 6: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations list --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet ef database update --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.2 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- RowVersion se configura explícitamente con IsRowVersion().
- EstadoDetalle se configura como IsConcurrencyToken().
- La migración M5_5_2_ConcurrencyTokens existe y se aplica.
- Dos contextos producen DbUpdateConcurrencyException cuando cambia el token.
- Se comprueba directamente que RowVersion no obtiene un índice automático.

### Reto resuelto y ampliación

**Reto:** Inspecciona el UPDATE capturado y localiza el token original dentro del predicado de concurrencia.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Confiar en el nombre RowVersion sin configurarlo.
- Suponer que rowversion crea automáticamente un índice.
- Probar semántica de rowversion de SQL Server con SQLite.
- Modificar a mano el snapshot o la migración para forzar el resultado.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.2 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.2, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.2 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.3` y parte directamente de este proyecto completo.

---

## Punto 5.3 — Resolución de conflictos de concurrencia

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.2`. La carpeta `M05/PROYECTO/5.3` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Resolver DbUpdateConcurrencyException con políticas explícitas: cliente gana, base gana, merge, notificación, fila eliminada y reintento acotado.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.3
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Repositories/ResolucionConflictosM5Repositorio.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`

```csharp
public ResolucionConflictoM5Dto ClienteGana()
{
    RestaurarOrden();
    using var scopeA = _scopeFactory.CreateScope();
    using var scopeB = _scopeFactory.CreateScope();
    var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var a = Cargar(contextA);
    var b = Cargar(contextB);

    a.Cliente = "Cliente A - cliente gana";
    contextA.SaveChanges();

    b.Cliente = "Cliente B - cliente gana";
    var conflicto = false;
    var intentos = 1;
    var valores = new List<ValorConflictoM5Dto>();

    try
    {
        contextB.SaveChanges();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        conflicto = true;
        var entry = ex.Entries.Single();
        var db = entry.GetDatabaseValues()
            ?? throw new InvalidOperationException("La orden desapareció durante ClienteGana.");
        valores = CrearValores(entry, db);
        entry.OriginalValues.SetValues(db);
        intentos++;
        contextB.SaveChanges();
    }

    var final = LeerFinal();
    return Resultado("Cliente gana", conflicto, final, intentos, valores);
}
```

#### Explicación línea a línea — ResolucionConflictosM5Repositorio.cs

Línea 1: `public ResolucionConflictoM5Dto ClienteGana()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `RestaurarOrden();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `using var scopeA = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 5: `using var scopeB = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 6: `var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 7: `var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 8: `var a = Cargar(contextA);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 9: `var b = Cargar(contextB);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 11: `a.Cliente = "Cliente A - cliente gana";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 12: `contextA.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 14: `b.Cliente = "Cliente B - cliente gana";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `var conflicto = false;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 16: `var intentos = 1;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 17: `var valores = new List<ValorConflictoM5Dto>();` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 19: `try` → Abre el bloque en el que se espera que pueda producirse la excepción que se quiere estudiar.

Línea 20: `{` → Delimita el bloque sintáctico correspondiente.

Línea 21: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 22: `}` → Delimita el bloque sintáctico correspondiente.

Línea 23: `catch (DbUpdateConcurrencyException ex)` → Captura la excepción específica para aplicar la política de resolución prevista.

Línea 24: `{` → Delimita el bloque sintáctico correspondiente.

Línea 25: `conflicto = true;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 26: `var entry = ex.Entries.Single();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 27: `var db = entry.GetDatabaseValues()` → Obtiene de la base el estado actual de la fila tras detectar el conflicto.

Línea 28: `?? throw new InvalidOperationException("La orden desapareció durante ClienteGana.");` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 29: `valores = CrearValores(entry, db);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 30: `entry.OriginalValues.SetValues(db);` → Actualiza los valores originales del entry para reintentar contra la versión recién leída.

Línea 31: `intentos++;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 32: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 33: `}` → Delimita el bloque sintáctico correspondiente.

Línea 35: `var final = LeerFinal();` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 36: `return Resultado("Cliente gana", conflicto, final, intentos, valores);` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 37: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`

```csharp
public ResolucionConflictoM5Dto ResolucionPersonalizada()
{
    RestaurarOrden();
    using var scopeA = _scopeFactory.CreateScope();
    using var scopeB = _scopeFactory.CreateScope();
    var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var a = Cargar(contextA);
    var b = Cargar(contextB);

    a.Cliente = "Cliente A - merge";
    a.Estado = "EnProceso A";
    contextA.SaveChanges();

    b.Cliente = "Cliente B - merge";
    b.Estado = "Completada B";
    var intentos = 1;

    try
    {
        contextB.SaveChanges();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        var entry = ex.Entries.Single();
        var db = entry.GetDatabaseValues()
            ?? throw new InvalidOperationException("La orden desapareció durante la fusión.");
        entry.OriginalValues.SetValues(db);
        entry.CurrentValues[nameof(OrdenFabricacion.Estado)] = db[nameof(OrdenFabricacion.Estado)];
        intentos++;
        contextB.SaveChanges();
    }

    return Resultado("Resolución personalizada", true, LeerFinal(), intentos, []);
}
```

#### Explicación línea a línea — ResolucionConflictosM5Repositorio.cs

Línea 1: `public ResolucionConflictoM5Dto ResolucionPersonalizada()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `RestaurarOrden();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `using var scopeA = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 5: `using var scopeB = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 6: `var contextA = scopeA.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 7: `var contextB = scopeB.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 8: `var a = Cargar(contextA);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 9: `var b = Cargar(contextB);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 11: `a.Cliente = "Cliente A - merge";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 12: `a.Estado = "EnProceso A";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 13: `contextA.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 15: `b.Cliente = "Cliente B - merge";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 16: `b.Estado = "Completada B";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 17: `var intentos = 1;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 19: `try` → Abre el bloque en el que se espera que pueda producirse la excepción que se quiere estudiar.

Línea 20: `{` → Delimita el bloque sintáctico correspondiente.

Línea 21: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 22: `}` → Delimita el bloque sintáctico correspondiente.

Línea 23: `catch (DbUpdateConcurrencyException ex)` → Captura la excepción específica para aplicar la política de resolución prevista.

Línea 24: `{` → Delimita el bloque sintáctico correspondiente.

Línea 25: `var entry = ex.Entries.Single();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 26: `var db = entry.GetDatabaseValues()` → Obtiene de la base el estado actual de la fila tras detectar el conflicto.

Línea 27: `?? throw new InvalidOperationException("La orden desapareció durante la fusión.");` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 28: `entry.OriginalValues.SetValues(db);` → Actualiza los valores originales del entry para reintentar contra la versión recién leída.

Línea 29: `entry.CurrentValues[nameof(OrdenFabricacion.Estado)] = db[nameof(OrdenFabricacion.Estado)];` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 30: `intentos++;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 31: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 32: `}` → Delimita el bloque sintáctico correspondiente.

Línea 34: `return Resultado("Resolución personalizada", true, LeerFinal(), intentos, []);` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 35: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Implementar y estudiar `ResolucionConflictosM5Repositorio.cs`

```csharp
while (true)
{
    intentos++;
    try
    {
        contextB.SaveChanges();
        break;
    }
    catch (DbUpdateConcurrencyException ex) when (intentos < maxIntentos)
    {
        conflicto = true;
        var entry = ex.Entries.Single();
        var db = entry.GetDatabaseValues()
            ?? throw new InvalidOperationException("La fila ya no existe.");
        entry.OriginalValues.SetValues(db);
    }
}
```

#### Explicación línea a línea — ResolucionConflictosM5Repositorio.cs

Línea 1: `while (true)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `intentos++;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `try` → Abre el bloque en el que se espera que pueda producirse la excepción que se quiere estudiar.

Línea 5: `{` → Delimita el bloque sintáctico correspondiente.

Línea 6: `contextB.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 7: `break;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 8: `}` → Delimita el bloque sintáctico correspondiente.

Línea 9: `catch (DbUpdateConcurrencyException ex) when (intentos < maxIntentos)` → Captura la excepción específica para aplicar la política de resolución prevista.

Línea 10: `{` → Delimita el bloque sintáctico correspondiente.

Línea 11: `conflicto = true;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 12: `var entry = ex.Entries.Single();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 13: `var db = entry.GetDatabaseValues()` → Obtiene de la base el estado actual de la fila tras detectar el conflicto.

Línea 14: `?? throw new InvalidOperationException("La fila ya no existe.");` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 15: `entry.OriginalValues.SetValues(db);` → Actualiza los valores originales del entry para reintentar contra la versión recién leída.

Línea 16: `}` → Delimita el bloque sintáctico correspondiente.

Línea 17: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 6: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.3 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- GetDatabaseValues recupera el estado actual de SQL Server.
- OriginalValues.SetValues actualiza la versión contra la que se reintenta.
- Reload descarta cambios locales cuando la base gana.
- La resolución personalizada decide propiedad por propiedad.
- Los reintentos están limitados.

### Reto resuelto y ampliación

**Reto:** Añade una política que preserve Cliente local, Estado de base y combine Observaciones solo cuando ambos lados no sean nulos.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Reintentar indefinidamente.
- Confundir CurrentValues, OriginalValues y valores actuales de base.
- Sobrescribir automáticamente sin informar al usuario cuando el negocio requiere decisión.
- No tratar GetDatabaseValues()==null cuando la fila fue eliminada.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.3 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.3, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.3 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.4` y parte directamente de este proyecto completo.

---

## Punto 5.4 — Transacciones: SaveChanges y transacciones explícitas

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.3`. La carpeta `M05/PROYECTO/5.4` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Demostrar atomicidad de SaveChanges, Commit, Rollback y savepoints reales con SQL Server y MARS desactivado.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.4
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Repositories/TransaccionesM5Repositorio.cs`
- `src/AceriaData.Console/appsettings.json`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `TransaccionesM5Repositorio.cs`

```csharp
public ResultadoTransaccionM5Dto DemostrarAtomicidadSaveChanges()
{
    const string numeroValido = "OF-M5-54-ATOMIC";
    Limpiar(numeroValido);
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

    context.OrdenesFabricacion.Add(CrearOrden(numeroValido));
    context.OrdenesFabricacion.Add(CrearOrden(NumeroExistente));

    var falloEsperado = false;
    try
    {
        context.SaveChanges();
    }
    catch (DbUpdateException)
    {
        falloEsperado = true;
        context.ChangeTracker.Clear();
    }

    var existeValida = Existe(numeroValido);
    return new ResultadoTransaccionM5Dto(
        "Atomicidad de un único SaveChanges",
        falloEsperado && !existeValida,
        existeValida,
        Existe(NumeroExistente),
        MarsHabilitado(context),
        SqlCommandCounterInterceptor.Instance.SnapshotCommands());
}
```

#### Explicación línea a línea — TransaccionesM5Repositorio.cs

Línea 1: `public ResultadoTransaccionM5Dto DemostrarAtomicidadSaveChanges()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `const string numeroValido = "OF-M5-54-ATOMIC";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 4: `Limpiar(numeroValido);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 5: `using var scope = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 6: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 8: `context.OrdenesFabricacion.Add(CrearOrden(numeroValido));` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 9: `context.OrdenesFabricacion.Add(CrearOrden(NumeroExistente));` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 11: `var falloEsperado = false;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 12: `try` → Abre el bloque en el que se espera que pueda producirse la excepción que se quiere estudiar.

Línea 13: `{` → Delimita el bloque sintáctico correspondiente.

Línea 14: `context.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 15: `}` → Delimita el bloque sintáctico correspondiente.

Línea 16: `catch (DbUpdateException)` → Captura la excepción específica para aplicar la política de resolución prevista.

Línea 17: `{` → Delimita el bloque sintáctico correspondiente.

Línea 18: `falloEsperado = true;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 19: `context.ChangeTracker.Clear();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 20: `}` → Delimita el bloque sintáctico correspondiente.

Línea 22: `var existeValida = Existe(numeroValido);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 23: `return new ResultadoTransaccionM5Dto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 24: `"Atomicidad de un único SaveChanges",` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 25: `falloEsperado && !existeValida,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 26: `existeValida,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 27: `Existe(NumeroExistente),` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 28: `MarsHabilitado(context),` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 29: `SqlCommandCounterInterceptor.Instance.SnapshotCommands());` → Recupera los comandos SQL realmente interceptados durante la operación.

Línea 30: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `TransaccionesM5Repositorio.cs`

```csharp
public ResultadoTransaccionM5Dto DemostrarRollbackASavepoint()
{
    const string numero1 = "OF-M5-54-SP-1";
    const string numero2 = "OF-M5-54-SP-2";
    const string savepoint = "AntesSegundaOrden";
    Limpiar(numero1, numero2);

    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();
    var mars = MarsHabilitado(context);
    if (mars)
        throw new InvalidOperationException("5.4 requiere MultipleActiveResultSets=false.");

    using (var transaction = context.Database.BeginTransaction())
    {
        context.OrdenesFabricacion.Add(CrearOrden(numero1));
        context.SaveChanges();
        transaction.CreateSavepoint(savepoint);

        context.OrdenesFabricacion.Add(CrearOrden(numero2));
        context.SaveChanges();
        transaction.RollbackToSavepoint(savepoint);

        context.ChangeTracker.Clear();
        transaction.Commit();
    }

    return new ResultadoTransaccionM5Dto(
        "Rollback a savepoint",
        Existe(numero1) && !Existe(numero2),
        Existe(numero1), Existe(numero2), mars,
        SqlCommandCounterInterceptor.Instance.SnapshotCommands());
}
```

#### Explicación línea a línea — TransaccionesM5Repositorio.cs

Línea 1: `public ResultadoTransaccionM5Dto DemostrarRollbackASavepoint()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `const string numero1 = "OF-M5-54-SP-1";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 4: `const string numero2 = "OF-M5-54-SP-2";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 5: `const string savepoint = "AntesSegundaOrden";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 6: `Limpiar(numero1, numero2);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 8: `using var scope = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 9: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 10: `var mars = MarsHabilitado(context);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 11: `if (mars)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 12: `throw new InvalidOperationException("5.4 requiere MultipleActiveResultSets=false.");` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 14: `using (var transaction = context.Database.BeginTransaction())` → Abre una transacción explícita sobre la conexión del DbContext.

Línea 15: `{` → Delimita el bloque sintáctico correspondiente.

Línea 16: `context.OrdenesFabricacion.Add(CrearOrden(numero1));` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 17: `context.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 18: `transaction.CreateSavepoint(savepoint);` → Crea un punto intermedio al que puede volver la transacción sin descartar todo el trabajo previo.

Línea 20: `context.OrdenesFabricacion.Add(CrearOrden(numero2));` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 21: `context.SaveChanges();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 22: `transaction.RollbackToSavepoint(savepoint);` → Revierte solo las operaciones posteriores al savepoint indicado.

Línea 24: `context.ChangeTracker.Clear();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 25: `transaction.Commit();` → Marca/confirma la unidad transaccional cuando todas las operaciones requeridas han terminado correctamente.

Línea 26: `}` → Delimita el bloque sintáctico correspondiente.

Línea 28: `return new ResultadoTransaccionM5Dto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 29: `"Rollback a savepoint",` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 30: `Existe(numero1) && !Existe(numero2),` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 31: `Existe(numero1), Existe(numero2), mars,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 32: `SqlCommandCounterInterceptor.Instance.SnapshotCommands());` → Recupera los comandos SQL realmente interceptados durante la operación.

Línea 33: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Implementar y estudiar `appsettings.json`

```json
{
  "ConnectionStrings": {
    "AceriaDB": "Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;"
  }
}
```

#### Explicación línea a línea — appsettings.json

Línea 1: `{` → Delimita el bloque sintáctico correspondiente.

Línea 2: `"ConnectionStrings": {` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 3: `"AceriaDB": "Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;"` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 4: `}` → Delimita el bloque sintáctico correspondiente.

Línea 5: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 6: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.4 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- Un único SaveChanges revierte todo el conjunto si una escritura falla.
- Una transacción explícita puede agrupar varias llamadas a SaveChanges.
- Rollback deshace una escritura ya enviada antes del fallo posterior.
- RollbackToSavepoint conserva lo anterior al savepoint y revierte lo posterior.
- MultipleActiveResultSets=false en el laboratorio de savepoints.

### Reto resuelto y ampliación

**Reto:** Introduce un tercer SaveChanges después del savepoint y decide qué parte debe persistir tras un rollback parcial.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Dar por hecho que Commit fallido siempre implica un estado conocido.
- Usar MARS y esperar savepoints automáticos de EF Core.
- Presentar RELEASE SAVEPOINT como operación necesaria de SQL Server.
- Mantener transacciones abiertas durante trabajo ajeno a la base.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.4 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.4, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.4 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.5` y parte directamente de este proyecto completo.

---

## Punto 5.5 — Transacciones ambientales y buenas prácticas

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.4`. La carpeta `M05/PROYECTO/5.5` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Usar TransactionScope con async, varios DbContext sobre una conexión compartida, opciones de scope y un efecto externo no transaccional.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.5
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Repositories/TransaccionesAmbientalesM5Repositorio.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `TransaccionesAmbientalesM5Repositorio.cs`

```csharp
public async Task<TransaccionAmbientalM5Dto> DemostrarDosContextosAsync()
{
    var options = new TransactionOptions
    {
        IsolationLevel = IsolationLevel.ReadCommitted,
        Timeout = TimeSpan.FromSeconds(30)
    };

    bool ambienteActivo;
    bool flujoAsync;
    bool promocion;
    string aislamiento;
    int ordenId;

    using (var scope = new TransactionScope(
               TransactionScopeOption.Required,
               options,
               TransactionScopeAsyncFlowOption.Enabled))
    {
        await using var connection = new SqlConnection(ObtenerConnectionString());
        await connection.OpenAsync();
        ambienteActivo = Transaction.Current is not null;
        aislamiento = Transaction.Current?.IsolationLevel.ToString() ?? "<none>";

        await using (var contextOrden = CrearContexto(connection))
        {
            var orden = CrearOrden("OF-M5-55-AMBIENT");
            contextOrden.OrdenesFabricacion.Add(orden);
            await contextOrden.SaveChangesAsync();
            ordenId = orden.Id;
        }

        await Task.Yield();
        flujoAsync = Transaction.Current is not null;

        await using (var contextPlancha = CrearContexto(connection))
        {
            contextPlancha.PlanchasAcero.Add(new PlanchaAcero
            {
                OrdenId = ordenId,
                Espesor = 10, Ancho = 1000, Largo = 2000, Peso = 150m
            });
            await contextPlancha.SaveChangesAsync();
        }

        promocion = Transaction.Current?.TransactionInformation.DistributedIdentifier != Guid.Empty;
        scope.Complete();
    }

    return new TransaccionAmbientalM5Dto(
        "Dos DbContext con una conexión compartida",
        ExisteOrdenYPlancha("OF-M5-55-AMBIENT"),
        ambienteActivo, flujoAsync, promocion, aislamiento,
        SqlCommandCounterInterceptor.Instance.SnapshotCommands());
}
```

#### Explicación línea a línea — TransaccionesAmbientalesM5Repositorio.cs

Línea 1: `public async Task<TransaccionAmbientalM5Dto> DemostrarDosContextosAsync()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `var options = new TransactionOptions` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 4: `{` → Delimita el bloque sintáctico correspondiente.

Línea 5: `IsolationLevel = IsolationLevel.ReadCommitted,` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 6: `Timeout = TimeSpan.FromSeconds(30)` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 7: `};` → Delimita el bloque sintáctico correspondiente.

Línea 9: `bool ambienteActivo;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 10: `bool flujoAsync;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 11: `bool promocion;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 12: `string aislamiento;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 13: `int ordenId;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 15: `using (var scope = new TransactionScope(` → Crea o participa en una transacción ambiental según la opción indicada.

Línea 16: `TransactionScopeOption.Required,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `options,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 18: `TransactionScopeAsyncFlowOption.Enabled))` → Permite que Transaction.Current fluya correctamente a través de await.

Línea 19: `{` → Delimita el bloque sintáctico correspondiente.

Línea 20: `await using var connection = new SqlConnection(ObtenerConnectionString());` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 21: `await connection.OpenAsync();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 22: `ambienteActivo = Transaction.Current is not null;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 23: `aislamiento = Transaction.Current?.IsolationLevel.ToString() ?? "<none>";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 25: `await using (var contextOrden = CrearContexto(connection))` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 26: `{` → Delimita el bloque sintáctico correspondiente.

Línea 27: `var orden = CrearOrden("OF-M5-55-AMBIENT");` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 28: `contextOrden.OrdenesFabricacion.Add(orden);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 29: `await contextOrden.SaveChangesAsync();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 30: `ordenId = orden.Id;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 31: `}` → Delimita el bloque sintáctico correspondiente.

Línea 33: `await Task.Yield();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 34: `flujoAsync = Transaction.Current is not null;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 36: `await using (var contextPlancha = CrearContexto(connection))` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 37: `{` → Delimita el bloque sintáctico correspondiente.

Línea 38: `contextPlancha.PlanchasAcero.Add(new PlanchaAcero` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 39: `{` → Delimita el bloque sintáctico correspondiente.

Línea 40: `OrdenId = ordenId,` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 41: `Espesor = 10, Ancho = 1000, Largo = 2000, Peso = 150m` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 42: `});` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 43: `await contextPlancha.SaveChangesAsync();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 44: `}` → Delimita el bloque sintáctico correspondiente.

Línea 46: `promocion = Transaction.Current?.TransactionInformation.DistributedIdentifier != Guid.Empty;` → Comprueba si la transacción fue promocionada a una transacción distribuida.

Línea 47: `scope.Complete();` → Marca/confirma la unidad transaccional cuando todas las operaciones requeridas han terminado correctamente.

Línea 48: `}` → Delimita el bloque sintáctico correspondiente.

Línea 50: `return new TransaccionAmbientalM5Dto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 51: `"Dos DbContext con una conexión compartida",` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 52: `ExisteOrdenYPlancha("OF-M5-55-AMBIENT"),` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 53: `ambienteActivo, flujoAsync, promocion, aislamiento,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 54: `SqlCommandCounterInterceptor.Instance.SnapshotCommands());` → Recupera los comandos SQL realmente interceptados durante la operación.

Línea 55: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `TransaccionesAmbientalesM5Repositorio.cs`

```csharp
public OpcionesTransactionScopeM5Dto DemostrarOpcionesDeScope()
{
    string outerId;
    bool requiredMisma;
    bool requiresNewOtra;
    bool suppressSinAmbiente;
    string aislamiento;

    using (var outer = new TransactionScope())
    {
        var outerTransaction = Transaction.Current
            ?? throw new InvalidOperationException("No se creó la transacción ambiental.");
        outerId = outerTransaction.TransactionInformation.LocalIdentifier;
        aislamiento = outerTransaction.IsolationLevel.ToString();

        using (var required = new TransactionScope(TransactionScopeOption.Required))
        {
            requiredMisma = Transaction.Current?.TransactionInformation.LocalIdentifier == outerId;
            required.Complete();
        }

        using (var requiresNew = new TransactionScope(TransactionScopeOption.RequiresNew))
        {
            requiresNewOtra = Transaction.Current?.TransactionInformation.LocalIdentifier != outerId;
            requiresNew.Complete();
        }

        using (var suppressed = new TransactionScope(TransactionScopeOption.Suppress))
        {
            suppressSinAmbiente = Transaction.Current is null;
            suppressed.Complete();
        }
        outer.Complete();
    }

    return new OpcionesTransactionScopeM5Dto(
        requiredMisma, requiresNewOtra, suppressSinAmbiente,
        aislamiento, TransactionManager.DefaultTimeout.TotalSeconds);
}
```

#### Explicación línea a línea — TransaccionesAmbientalesM5Repositorio.cs

Línea 1: `public OpcionesTransactionScopeM5Dto DemostrarOpcionesDeScope()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `string outerId;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 4: `bool requiredMisma;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 5: `bool requiresNewOtra;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 6: `bool suppressSinAmbiente;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 7: `string aislamiento;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 9: `using (var outer = new TransactionScope())` → Crea o participa en una transacción ambiental según la opción indicada.

Línea 10: `{` → Delimita el bloque sintáctico correspondiente.

Línea 11: `var outerTransaction = Transaction.Current` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 12: `?? throw new InvalidOperationException("No se creó la transacción ambiental.");` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 13: `outerId = outerTransaction.TransactionInformation.LocalIdentifier;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 14: `aislamiento = outerTransaction.IsolationLevel.ToString();` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 16: `using (var required = new TransactionScope(TransactionScopeOption.Required))` → Crea o participa en una transacción ambiental según la opción indicada.

Línea 17: `{` → Delimita el bloque sintáctico correspondiente.

Línea 18: `requiredMisma = Transaction.Current?.TransactionInformation.LocalIdentifier == outerId;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 19: `required.Complete();` → Marca/confirma la unidad transaccional cuando todas las operaciones requeridas han terminado correctamente.

Línea 20: `}` → Delimita el bloque sintáctico correspondiente.

Línea 22: `using (var requiresNew = new TransactionScope(TransactionScopeOption.RequiresNew))` → Crea o participa en una transacción ambiental según la opción indicada.

Línea 23: `{` → Delimita el bloque sintáctico correspondiente.

Línea 24: `requiresNewOtra = Transaction.Current?.TransactionInformation.LocalIdentifier != outerId;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 25: `requiresNew.Complete();` → Marca/confirma la unidad transaccional cuando todas las operaciones requeridas han terminado correctamente.

Línea 26: `}` → Delimita el bloque sintáctico correspondiente.

Línea 28: `using (var suppressed = new TransactionScope(TransactionScopeOption.Suppress))` → Crea o participa en una transacción ambiental según la opción indicada.

Línea 29: `{` → Delimita el bloque sintáctico correspondiente.

Línea 30: `suppressSinAmbiente = Transaction.Current is null;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 31: `suppressed.Complete();` → Marca/confirma la unidad transaccional cuando todas las operaciones requeridas han terminado correctamente.

Línea 32: `}` → Delimita el bloque sintáctico correspondiente.

Línea 33: `outer.Complete();` → Marca/confirma la unidad transaccional cuando todas las operaciones requeridas han terminado correctamente.

Línea 34: `}` → Delimita el bloque sintáctico correspondiente.

Línea 36: `return new OpcionesTransactionScopeM5Dto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 37: `requiredMisma, requiresNewOtra, suppressSinAmbiente,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 38: `aislamiento, TransactionManager.DefaultTimeout.TotalSeconds);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 39: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.5 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- TransactionScopeAsyncFlowOption.Enabled conserva Transaction.Current tras await.
- Los dos DbContext usan una misma SqlConnection abierta para no depender de promoción distribuida.
- Required, RequiresNew y Suppress se distinguen por su Transaction.Current.
- Omitir Complete revierte SQL.
- El efecto externo simulado permanece porque no participa en System.Transactions.

### Reto resuelto y ampliación

**Reto:** Ejecuta el escenario con Suppress alrededor de una operación y verifica qué queda fuera de la transacción ambiental.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Asumir que ReadCommitted permite lecturas sucias.
- Afirmar que Snapshot elimina todos los bloqueos y conflictos.
- Abrir varios recursos durables sin considerar posible promoción a transacción distribuida.
- Esperar que HTTP, colas o archivos se reviertan con TransactionScope.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.5 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.5, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.5 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.6` y parte directamente de este proyecto completo.

---

## Punto 5.6 — Migraciones en entornos de producción: estrategias y despliegue

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.5`. La carpeta `M05/PROYECTO/5.6` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Separar desarrollo y despliegue controlado mediante IMigrator, scripts revisables y migration bundles, conservando el historial real.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.6
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Repositories/MigracionesProduccionM5Repositorio.cs`
- `deployment/generate-production-artifacts.ps1`
- `deployment/PLAN_DESPLIEGUE.md`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `MigracionesProduccionM5Repositorio.cs`

```csharp
public async Task<MigracionesProduccionM5Dto> AplicarConIMigratorAsync()
{
    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();

    var migrator = context.GetService<IMigrator>();
    await migrator.MigrateAsync();

    var aplicadas = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
    var pendientes = (await context.Database.GetPendingMigrationsAsync()).ToArray();
    var tablaExiste = await ExisteTablaHistorialAsync(context);

    return new MigracionesProduccionM5Dto(
        aplicadas.Length,
        pendientes.Length,
        aplicadas.LastOrDefault() ?? "<ninguna>",
        tablaExiste,
        "__EFMigrationsHistory");
}
```

#### Explicación línea a línea — MigracionesProduccionM5Repositorio.cs

Línea 1: `public async Task<MigracionesProduccionM5Dto> AplicarConIMigratorAsync()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `using var scope = _scopeFactory.CreateScope();` → Crea un ámbito de DI independiente; los servicios Scoped, incluido AceriaDbContext, no se comparten con otros actores.

Línea 4: `var context = scope.ServiceProvider.GetRequiredService<AceriaDbContext>();` → Resuelve el DbContext del ámbito y garantiza que la configuración de Infrastructure sea la misma que en la aplicación.

Línea 6: `var migrator = context.GetService<IMigrator>();` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 7: `await migrator.MigrateAsync();` → Aplica la cadena real de migraciones al proveedor de pruebas o despliegue.

Línea 9: `var aplicadas = (await context.Database.GetAppliedMigrationsAsync()).ToArray();` → Lee la lista de migraciones que la base registra como aplicadas.

Línea 10: `var pendientes = (await context.Database.GetPendingMigrationsAsync()).ToArray();` → Comprueba si queda alguna migración del ensamblado pendiente de aplicar.

Línea 11: `var tablaExiste = await ExisteTablaHistorialAsync(context);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 13: `return new MigracionesProduccionM5Dto(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 14: `aplicadas.Length,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 15: `pendientes.Length,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 16: `aplicadas.LastOrDefault() ?? "<ninguna>",` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `tablaExiste,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 18: `"__EFMigrationsHistory");` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 19: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `generate-production-artifacts.ps1`

```powershell
$ErrorActionPreference = "Stop"
$Artifacts = Join-Path $PSScriptRoot "artifacts"
New-Item -ItemType Directory -Force -Path $Artifacts | Out-Null

dotnet ef migrations script --idempotent `
  --project ../src/AceriaData.Infrastructure `
  --startup-project ../src/AceriaData.Console `
  --configuration Release `
  --output (Join-Path $Artifacts "aceria-idempotent.sql")

if ($LASTEXITCODE -ne 0) { throw "No se pudo generar el script idempotente" }

dotnet ef migrations bundle `
  --project ../src/AceriaData.Infrastructure `
  --startup-project ../src/AceriaData.Console `
  --configuration Release `
  --output (Join-Path $Artifacts "aceria-efbundle.exe") `
  --force

if ($LASTEXITCODE -ne 0) { throw "No se pudo generar el migration bundle" }
```

#### Explicación línea a línea — generate-production-artifacts.ps1

Línea 1: `$ErrorActionPreference = "Stop"` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 2: `$Artifacts = Join-Path $PSScriptRoot "artifacts"` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 3: `New-Item -ItemType Directory -Force -Path $Artifacts | Out-Null` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 5: `dotnet ef migrations script --idempotent \`` → Genera SQL desde la cadena real de migraciones de EF Core.

Línea 6: `--project ../src/AceriaData.Infrastructure \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 7: `--startup-project ../src/AceriaData.Console \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 8: `--configuration Release \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 9: `--output (Join-Path $Artifacts "aceria-idempotent.sql")` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 11: `if ($LASTEXITCODE -ne 0) { throw "No se pudo generar el script idempotente" }` → Convierte un fallo del comando externo en un fallo explícito del script.

Línea 13: `dotnet ef migrations bundle \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 14: `--project ../src/AceriaData.Infrastructure \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 15: `--startup-project ../src/AceriaData.Console \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 16: `--configuration Release \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `--output (Join-Path $Artifacts "aceria-efbundle.exe") \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 18: `--force` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 20: `if ($LASTEXITCODE -ne 0) { throw "No se pudo generar el migration bundle" }` → Convierte un fallo del comando externo en un fallo explícito del script.

### Paso 5: Implementar y estudiar `Comandos de despliegue`

```powershell
dotnet ef migrations script --idempotent `
  --project src/AceriaData.Infrastructure `
  --startup-project src/AceriaData.Console `
  --configuration Release `
  --output aceria-idempotent.sql

# El bundle recibe la conexión en ejecución, no desde código fuente.
./aceria-efbundle.exe --connection $env:ACERIA_PROD_CONNECTION
```

#### Explicación línea a línea — Comandos de despliegue

Línea 1: `dotnet ef migrations script --idempotent \`` → Genera SQL desde la cadena real de migraciones de EF Core.

Línea 2: `--project src/AceriaData.Infrastructure \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 3: `--startup-project src/AceriaData.Console \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 4: `--configuration Release \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 5: `--output aceria-idempotent.sql` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 7: `# El bundle recibe la conexión en ejecución, no desde código fuente.` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 8: `./aceria-efbundle.exe --connection $env:ACERIA_PROD_CONNECTION` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

### Paso 6: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.6 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- MigrationsAssembly apunta al ensamblado real de migraciones.
- IMigrator aplica la cadena y deja cero migraciones pendientes.
- Se conserva __EFMigrationsHistory.
- El bundle recibe la cadena de conexión desde el entorno de despliegue.
- El plan distingue downgrade de recuperación de datos.

### Reto resuelto y ampliación

**Reto:** Genera un script revisable y un bundle para la misma cadena y documenta qué control operacional aporta cada uno.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Ejecutar Database.Migrate en cada réplica como regla general de producción.
- Renombrar MigrationsHistoryTable sin mover el historial existente.
- Guardar credenciales de producción en el repositorio.
- Confundir un Down destructivo con restauración segura.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.6 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.6, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.6 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.7` y parte directamente de este proyecto completo.

---

## Punto 5.7 — Migraciones idempotentes y scripts SQL

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.6`. La carpeta `M05/PROYECTO/5.7` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Generar scripts completo, idempotente, de rango y downgrade, y demostrar idempotencia aplicando dos veces el mismo script.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.7
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `deployment/validate-idempotent-scripts.ps1`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `validate-idempotent-scripts.ps1`

```powershell
dotnet ef migrations script `
  --project $Infrastructure --startup-project $Startup `
  --configuration Release --output $Complete

dotnet ef migrations script --idempotent `
  --project $Infrastructure --startup-project $Startup `
  --configuration Release --output $Idempotent

dotnet ef migrations script M2_2_12_Architecture M5_5_2_ConcurrencyTokens `
  --project $Infrastructure --startup-project $Startup `
  --configuration Release --output $Range

dotnet ef migrations script M5_5_2_ConcurrencyTokens M2_2_12_Architecture `
  --project $Infrastructure --startup-project $Startup `
  --configuration Release --output $Downgrade

$idempotentSql = Get-Content $Idempotent -Raw
if ($idempotentSql -notmatch "__EFMigrationsHistory") { throw "Falta historial" }
if ($idempotentSql -notmatch "IF NOT EXISTS") { throw "Faltan guardas" }
if ($idempotentSql -notmatch "M5_5_2_ConcurrencyTokens") { throw "Falta migración final" }
```

#### Explicación línea a línea — validate-idempotent-scripts.ps1

Línea 1: `dotnet ef migrations script \`` → Genera SQL desde la cadena real de migraciones de EF Core.

Línea 2: `--project $Infrastructure --startup-project $Startup \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 3: `--configuration Release --output $Complete` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 5: `dotnet ef migrations script --idempotent \`` → Genera SQL desde la cadena real de migraciones de EF Core.

Línea 6: `--project $Infrastructure --startup-project $Startup \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 7: `--configuration Release --output $Idempotent` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 9: `dotnet ef migrations script M2_2_12_Architecture M5_5_2_ConcurrencyTokens \`` → Genera SQL desde la cadena real de migraciones de EF Core.

Línea 10: `--project $Infrastructure --startup-project $Startup \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 11: `--configuration Release --output $Range` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 13: `dotnet ef migrations script M5_5_2_ConcurrencyTokens M2_2_12_Architecture \`` → Genera SQL desde la cadena real de migraciones de EF Core.

Línea 14: `--project $Infrastructure --startup-project $Startup \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 15: `--configuration Release --output $Downgrade` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `$idempotentSql = Get-Content $Idempotent -Raw` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 18: `if ($idempotentSql -notmatch "__EFMigrationsHistory") { throw "Falta historial" }` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 19: `if ($idempotentSql -notmatch "IF NOT EXISTS") { throw "Faltan guardas" }` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 20: `if ($idempotentSql -notmatch "M5_5_2_ConcurrencyTokens") { throw "Falta migración final" }` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

### Paso 4: Implementar y estudiar `validate-idempotent-scripts.ps1`

```powershell
Write-Host "== Primera aplicacion del script idempotente =="
& sqlcmd -S $Server -d $Database -E -I -b -i $Idempotent
Assert-LastExitCode "La primera aplicacion fallo"

$count1 = [int](Read-Scalar "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;")
if ($count1 -le 0) { throw "El historial quedó vacío" }

$lastMigration = Read-Scalar "SELECT TOP (1) MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId DESC;"
if ($lastMigration -notmatch "M5_5_2_ConcurrencyTokens") {
    throw "La migración final no es la esperada: $lastMigration"
}

Write-Host "== Segunda aplicacion del mismo script idempotente =="
& sqlcmd -S $Server -d $Database -E -I -b -i $Idempotent
Assert-LastExitCode "La segunda aplicacion fallo"

$count2 = [int](Read-Scalar "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;")
if ($count2 -ne $count1) {
    throw "La segunda aplicación alteró el número de migraciones"
}

Write-Host "5.7 IDEMPOTENCIA OK"
```

#### Explicación línea a línea — validate-idempotent-scripts.ps1

Línea 1: `Write-Host "== Primera aplicacion del script idempotente =="` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 2: `& sqlcmd -S $Server -d $Database -E -I -b -i $Idempotent` → Ejecuta o consulta SQL Server desde PowerShell para validar el artefacto generado.

Línea 3: `Assert-LastExitCode "La primera aplicacion fallo"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 5: `$count1 = [int](Read-Scalar "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;")` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 6: `if ($count1 -le 0) { throw "El historial quedó vacío" }` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 8: `$lastMigration = Read-Scalar "SELECT TOP (1) MigrationId FROM dbo.__EFMigrationsHistory ORDER BY MigrationId DESC;"` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 9: `if ($lastMigration -notmatch "M5_5_2_ConcurrencyTokens") {` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 10: `throw "La migración final no es la esperada: $lastMigration"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 11: `}` → Delimita el bloque sintáctico correspondiente.

Línea 13: `Write-Host "== Segunda aplicacion del mismo script idempotente =="` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 14: `& sqlcmd -S $Server -d $Database -E -I -b -i $Idempotent` → Ejecuta o consulta SQL Server desde PowerShell para validar el artefacto generado.

Línea 15: `Assert-LastExitCode "La segunda aplicacion fallo"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `$count2 = [int](Read-Scalar "SELECT COUNT(*) FROM dbo.__EFMigrationsHistory;")` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 18: `if ($count2 -ne $count1) {` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 19: `throw "La segunda aplicación alteró el número de migraciones"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 20: `}` → Delimita el bloque sintáctico correspondiente.

Línea 22: `Write-Host "5.7 IDEMPOTENCIA OK"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

### Paso 5: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
./deployment/validate-idempotent-scripts.ps1
```

La ejecución debe finalizar con `5.7 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- El script idempotente consulta __EFMigrationsHistory.
- La primera aplicación deja la migración final esperada.
- La segunda aplicación no cambia el número de filas del historial.
- El esquema esperado permanece estable.
- El downgrade se trata como operación potencialmente destructiva.

### Reto resuelto y ampliación

**Reto:** Crea una base aislada adicional y aplica primero un script de rango y después el idempotente completo; compara el historial final.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Llamar idempotente a un script sin ejecutarlo dos veces.
- Usar nombres ficticios de migración que no existen en el proyecto.
- Aplicar un downgrade sin revisar operaciones destructivas.
- Tomar el script generado como sustituto del código de migraciones.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.7 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.7, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.7 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.8` y parte directamente de este proyecto completo.

---

## Punto 5.8 — Migraciones en equipos: conflictos y buenas prácticas

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.7`. La carpeta `M05/PROYECTO/5.8` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Reproducir migraciones paralelas y resolverlas regenerando la migración propia sobre el modelo ya fusionado.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.8
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `team-migrations/validate-team-migrations.ps1`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `validate-team-migrations.ps1`

El script crea primero dos copias temporales y desechables del mismo estado inicial. A continuación aplica en cada copia un cambio de modelo distinto y genera las migraciones paralelas:

```powershell
Write-Host "== Rama A desde estado inicial =="
Add-TeamProperty $BranchA "EquipoRevisionA"
Add-Migration $BranchA "M5_5_8_TeamA"

Write-Host "== Rama B paralela desde el mismo estado inicial =="
Add-TeamProperty $BranchB "EquipoRevisionB"
Add-Migration $BranchB "M5_5_8_TeamBParallel"

$designerB = Get-ChildItem `
  (Join-Path $BranchB "src/AceriaData.Infrastructure/Migrations") `
  -Filter "*_M5_5_8_TeamBParallel.Designer.cs" | Select-Object -First 1
$parallelMetadata = Get-Content $designerB.FullName -Raw
if ($parallelMetadata -match "EquipoRevisionA") {
    throw "La rama B paralela no debería conocer el cambio A"
}
if ($parallelMetadata -notmatch "EquipoRevisionB") {
    throw "La rama B paralela no contiene su propio cambio"
}
Write-Host "Renombrar B no fusionaría sus metadatos."
```

#### Explicación línea a línea — validate-team-migrations.ps1

Línea 1: `Write-Host "== Rama A desde estado inicial =="` → Identifica en la salida la preparación de la primera rama temporal.

Línea 2: `Add-TeamProperty $BranchA "EquipoRevisionA"` → Añade el cambio de modelo propio de la rama A.

Línea 3: `Add-Migration $BranchA "M5_5_8_TeamA"` → Genera la migración A desde el estado inicial de esa rama.

Línea 5: `Write-Host "== Rama B paralela desde el mismo estado inicial =="` → Identifica la segunda rama, creada desde el mismo estado que A.

Línea 6: `Add-TeamProperty $BranchB "EquipoRevisionB"` → Añade a B un cambio diferente sin incorporar todavía A.

Línea 7: `Add-Migration $BranchB "M5_5_8_TeamBParallel"` → Genera la migración paralela B para poder inspeccionar su metadata.

Línea 11: `$designerB = Get-ChildItem \`` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 12: `(Join-Path $BranchB "src/AceriaData.Infrastructure/Migrations") \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 13: `-Filter "*_M5_5_8_TeamBParallel.Designer.cs" | Select-Object -First 1` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 14: `$parallelMetadata = Get-Content $designerB.FullName -Raw` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `if ($parallelMetadata -match "EquipoRevisionA") {` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 16: `throw "La rama B paralela no debería conocer el cambio A"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `}` → Delimita el bloque sintáctico correspondiente.

Línea 18: `if ($parallelMetadata -notmatch "EquipoRevisionB") {` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 19: `throw "La rama B paralela no contiene su propio cambio"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 20: `}` → Delimita el bloque sintáctico correspondiente.

Línea 21: `Write-Host "Renombrar B no fusionaría sus metadatos."` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

### Paso 4: Implementar y estudiar `validate-team-migrations.ps1`

```powershell
Write-Host "== Estado fusionado correcto: incorporar A y regenerar B =="
Copy-Item $BranchA -Destination $Merged -Recurse -Force
Add-TeamProperty $Merged "EquipoRevisionB"
Add-Migration $Merged "M5_5_8_TeamBRegenerated"

$designerRegenerated = Get-ChildItem `
  (Join-Path $Merged "src/AceriaData.Infrastructure/Migrations") `
  -Filter "*_M5_5_8_TeamBRegenerated.Designer.cs" | Select-Object -First 1
$mergedMetadata = Get-Content $designerRegenerated.FullName -Raw
if ($mergedMetadata -notmatch "EquipoRevisionA" -or
    $mergedMetadata -notmatch "EquipoRevisionB") {
    throw "La migración regenerada no representa el modelo fusionado A+B"
}

dotnet ef migrations has-pending-model-changes `
  --project src/AceriaData.Infrastructure `
  --startup-project src/AceriaData.Console `
  --configuration Release

Write-Host "5.8 EQUIPOS OK"
```

#### Explicación línea a línea — validate-team-migrations.ps1

Línea 1: `Write-Host "== Estado fusionado correcto: incorporar A y regenerar B =="` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 2: `Copy-Item $BranchA -Destination $Merged -Recurse -Force` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 3: `Add-TeamProperty $Merged "EquipoRevisionB"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 4: `Add-Migration $Merged "M5_5_8_TeamBRegenerated"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 6: `$designerRegenerated = Get-ChildItem \`` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 7: `(Join-Path $Merged "src/AceriaData.Infrastructure/Migrations") \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 8: `-Filter "*_M5_5_8_TeamBRegenerated.Designer.cs" | Select-Object -First 1` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 9: `$mergedMetadata = Get-Content $designerRegenerated.FullName -Raw` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 10: `if ($mergedMetadata -notmatch "EquipoRevisionA" -or` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 11: `$mergedMetadata -notmatch "EquipoRevisionB") {` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 12: `throw "La migración regenerada no representa el modelo fusionado A+B"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 13: `}` → Delimita el bloque sintáctico correspondiente.

Línea 15: `dotnet ef migrations has-pending-model-changes \`` → Comprueba que el modelo actual coincide con el snapshot de migraciones.

Línea 16: `--project src/AceriaData.Infrastructure \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 17: `--startup-project src/AceriaData.Console \`` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 18: `--configuration Release` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 20: `Write-Host "5.8 EQUIPOS OK"` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

### Paso 5: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
./team-migrations/validate-team-migrations.ps1
```

La ejecución debe finalizar con `5.8 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- Rama A y B parten del mismo estado y generan migraciones independientes.
- El Designer de B paralela desconoce el cambio A.
- Renombrar B no corrige sus metadatos.
- La B regenerada sobre A contiene ambos cambios.
- has-pending-model-changes y SQL Server verifican el resultado fusionado.

### Reto resuelto y ampliación

**Reto:** Simula una tercera rama C y describe el orden seguro para integrar A, B y C sin reescribir migraciones ya publicadas.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Resolver el conflicto renombrando archivos o timestamps.
- Borrar una migración que ya fue compartida o aplicada sin coordinación.
- Editar manualmente Designer o snapshot para aparentar coherencia.
- No ejecutar has-pending-model-changes tras el merge.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.8 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.8, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.8 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.9` y parte directamente de este proyecto completo.

---

## Punto 5.9 — Patrón Repositorio y Unidad de Trabajo en aplicaciones empresariales

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.8`. La carpeta `M05/PROYECTO/5.9` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Consolidar Repository/UoW como decisión arquitectónica de AceriaData, probarla con Moq y comparar funcionalmente con acceso directo.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.9
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/Repositories/RepositoryPattern.cs`
- `tests/AceriaData.Tests/RepositoryPatternTests.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `RepositoryPattern.cs`

```csharp
public class Repositorio<T> : IRepositorio<T> where T : class
{
    protected readonly AceriaDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repositorio(AceriaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual T? ObtenerPorId(int id) => _dbSet.Find(id);
    public virtual List<T> ObtenerTodas() => _dbSet.ToList();
    public virtual void Agregar(T entidad) => _dbSet.Add(entidad);
    public virtual void Eliminar(T entidad) => _dbSet.Remove(entidad);
}

public sealed class DetalleOrdenRepositorio : Repositorio<DetalleOrden>, IDetalleOrdenRepositorio
{
    public DetalleOrdenRepositorio(AceriaDbContext context) : base(context) { }

    public DetalleOrden? ObtenerPorOrden(int ordenId) =>
        _context.DetallesOrden.AsNoTracking().FirstOrDefault(d => d.OrdenId == ordenId);

    public List<DetalleOrden> ObtenerConTemperaturaMayorA(double temperatura) =>
        _context.DetallesOrden.AsNoTracking()
            .Where(d => d.TemperaturaColada > temperatura)
            .OrderBy(d => d.Id)
            .ToList();
}
```

#### Explicación línea a línea — RepositoryPattern.cs

Línea 1: `public class Repositorio<T> : IRepositorio<T> where T : class` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `protected readonly AceriaDbContext _context;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `protected readonly DbSet<T> _dbSet;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 6: `public Repositorio(AceriaDbContext context)` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 7: `{` → Delimita el bloque sintáctico correspondiente.

Línea 8: `_context = context;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 9: `_dbSet = context.Set<T>();` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 10: `}` → Delimita el bloque sintáctico correspondiente.

Línea 12: `public virtual T? ObtenerPorId(int id) => _dbSet.Find(id);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 13: `public virtual List<T> ObtenerTodas() => _dbSet.ToList();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 14: `public virtual void Agregar(T entidad) => _dbSet.Add(entidad);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `public virtual void Eliminar(T entidad) => _dbSet.Remove(entidad);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 16: `}` → Delimita el bloque sintáctico correspondiente.

Línea 18: `public sealed class DetalleOrdenRepositorio : Repositorio<DetalleOrden>, IDetalleOrdenRepositorio` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 19: `{` → Delimita el bloque sintáctico correspondiente.

Línea 20: `public DetalleOrdenRepositorio(AceriaDbContext context) : base(context) { }` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 22: `public DetalleOrden? ObtenerPorOrden(int ordenId) =>` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 23: `_context.DetallesOrden.AsNoTracking().FirstOrDefault(d => d.OrdenId == ordenId);` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 25: `public List<DetalleOrden> ObtenerConTemperaturaMayorA(double temperatura) =>` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 26: `_context.DetallesOrden.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 27: `.Where(d => d.TemperaturaColada > temperatura)` → Añade el predicado a la consulta; EF Core intentará traducirlo al proveedor antes de materializar.

Línea 28: `.OrderBy(d => d.Id)` → Añade orden determinista a la consulta antes de materializar.

Línea 29: `.ToList();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 30: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `RepositoryPatternTests.cs`

```csharp
[Fact]
public void ObtenerPendientes_UsaLaAbstraccionSinBaseDeDatos()
{
    var repo = new Mock<IOrdenRepositorio>();
    repo.Setup(r => r.ObtenerTodas()).Returns(
    [
        new OrdenFabricacion { NumeroOrden = "OF-1", Estado = "Pendiente" },
        new OrdenFabricacion { NumeroOrden = "OF-2", Estado = "EnProceso" },
        new OrdenFabricacion { NumeroOrden = "OF-3", Estado = "Pendiente" }
    ]);

    var servicio = new OrdenesConsultaM5Service(repo.Object);
    var pendientes = servicio.ObtenerPendientes();

    Assert.Equal(2, pendientes.Count);
    repo.Verify(r => r.ObtenerTodas(), Times.Once);
}

[Fact]
public void Registrar_DelegaEnElRepositorio()
{
    var repo = new Mock<IOrdenRepositorio>();
    var servicio = new OrdenesConsultaM5Service(repo.Object);
    var orden = new OrdenFabricacion { NumeroOrden = "OF-MOCK", Estado = "Pendiente" };

    servicio.Registrar(orden);

    repo.Verify(r => r.Agregar(orden), Times.Once);
}
```

#### Explicación línea a línea — RepositoryPatternTests.cs

Línea 1: `[Fact]` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 2: `public void ObtenerPendientes_UsaLaAbstraccionSinBaseDeDatos()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 3: `{` → Delimita el bloque sintáctico correspondiente.

Línea 4: `var repo = new Mock<IOrdenRepositorio>();` → Crea un mock de una abstracción de Application, sin necesitar EF Core ni una base de datos.

Línea 5: `repo.Setup(r => r.ObtenerTodas()).Returns(` → Define el comportamiento que devolverá el mock durante el caso de prueba.

Línea 6: `[` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 7: `new OrdenFabricacion { NumeroOrden = "OF-1", Estado = "Pendiente" },` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 8: `new OrdenFabricacion { NumeroOrden = "OF-2", Estado = "EnProceso" },` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 9: `new OrdenFabricacion { NumeroOrden = "OF-3", Estado = "Pendiente" }` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 10: `]);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 12: `var servicio = new OrdenesConsultaM5Service(repo.Object);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 13: `var pendientes = servicio.ObtenerPendientes();` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 15: `Assert.Equal(2, pendientes.Count);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 16: `repo.Verify(r => r.ObtenerTodas(), Times.Once);` → Comprueba que la colaboración con la dependencia ocurrió exactamente como espera el caso de uso.

Línea 17: `}` → Delimita el bloque sintáctico correspondiente.

Línea 19: `[Fact]` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 20: `public void Registrar_DelegaEnElRepositorio()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 21: `{` → Delimita el bloque sintáctico correspondiente.

Línea 22: `var repo = new Mock<IOrdenRepositorio>();` → Crea un mock de una abstracción de Application, sin necesitar EF Core ni una base de datos.

Línea 23: `var servicio = new OrdenesConsultaM5Service(repo.Object);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 24: `var orden = new OrdenFabricacion { NumeroOrden = "OF-MOCK", Estado = "Pendiente" };` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 26: `servicio.Registrar(orden);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 28: `repo.Verify(r => r.Agregar(orden), Times.Once);` → Comprueba que la colaboración con la dependencia ocurrió exactamente como espera el caso de uso.

Línea 29: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.9 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- Application trabaja con interfaces propias y no depende de EF Core.
- IUnidadDeTrabajo coordina repositorios con un SaveChanges.
- IDbContextFactory crea contextos independientes bajo demanda.
- Moq prueba lógica de Application sin base de datos.
- No se atribuye un coste fijo de tiempo o memoria al patrón.

### Reto resuelto y ampliación

**Reto:** Añade una operación específica de negocio al repositorio de órdenes y prueba con Moq que el caso de uso la invoca una sola vez.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Convertir Repository en obligación universal con EF Core.
- Exponer una abstracción que solo replique toda la API de DbSet sin aportar frontera.
- Mockear DbSet/LINQ cuando puede mockearse una interfaz de aplicación.
- Publicar un benchmark sin calentamiento, aislamiento y varias iteraciones.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.9 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.9, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.9 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.10` y parte directamente de este proyecto completo.

---

## Punto 5.10 — Logging y diagnóstico en Entity Framework Core

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.9`. La carpeta `M05/PROYECTO/5.10` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Configurar logging estructurado, rotación, DiagnosticListener, EventCounters y telemetría para una aplicación Console/Worker.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.10
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Console/Diagnostics/EfDiagnosticObserver.cs`
- `src/AceriaData.Console/Diagnostics/EfEventCounterListener.cs`
- `src/AceriaData.Console/LoggingDiagnosticoM5Runner.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `EfDiagnosticObserver.cs`

```csharp
public sealed class EfDiagnosticObserver :
    IObserver<DiagnosticListener>,
    IObserver<KeyValuePair<string, object?>>,
    IDisposable
{
    private readonly ConcurrentBag<IDisposable> _subscriptions = new();
    private readonly IDisposable _allListenersSubscription;
    private int _efEventCount;

    public EfDiagnosticObserver()
    {
        _allListenersSubscription = DiagnosticListener.AllListeners.Subscribe(this);
    }

    public int EfEventCount => Volatile.Read(ref _efEventCount);

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Microsoft.EntityFrameworkCore")
            _subscriptions.Add(listener.Subscribe(this, IsEnabled));
    }

    private static bool IsEnabled(string eventName, object? arg1, object? arg2) =>
        eventName.Contains("Command", StringComparison.Ordinal) ||
        eventName.Contains("SaveChanges", StringComparison.Ordinal);

    public void OnNext(KeyValuePair<string, object?> value)
    {
        if (value.Key.StartsWith("Microsoft.EntityFrameworkCore.", StringComparison.Ordinal))
            Interlocked.Increment(ref _efEventCount);
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
            subscription.Dispose();
        _allListenersSubscription.Dispose();
    }
}
```

#### Explicación línea a línea — EfDiagnosticObserver.cs

Línea 1: `public sealed class EfDiagnosticObserver :` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 2: `IObserver<DiagnosticListener>,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 3: `IObserver<KeyValuePair<string, object?>>,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 4: `IDisposable` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 5: `{` → Delimita el bloque sintáctico correspondiente.

Línea 6: `private readonly ConcurrentBag<IDisposable> _subscriptions = new();` → Declara una operación auxiliar usada para mantener aislada la lógica del ejemplo.

Línea 7: `private readonly IDisposable _allListenersSubscription;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 8: `private int _efEventCount;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 10: `public EfDiagnosticObserver()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 11: `{` → Delimita el bloque sintáctico correspondiente.

Línea 12: `_allListenersSubscription = DiagnosticListener.AllListeners.Subscribe(this);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 13: `}` → Delimita el bloque sintáctico correspondiente.

Línea 15: `public int EfEventCount => Volatile.Read(ref _efEventCount);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 17: `public void OnNext(DiagnosticListener listener)` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 18: `{` → Delimita el bloque sintáctico correspondiente.

Línea 19: `if (listener.Name == "Microsoft.EntityFrameworkCore")` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 20: `_subscriptions.Add(listener.Subscribe(this, IsEnabled));` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 21: `}` → Delimita el bloque sintáctico correspondiente.

Línea 23: `private static bool IsEnabled(string eventName, object? arg1, object? arg2) =>` → Declara una operación auxiliar usada para mantener aislada la lógica del ejemplo.

Línea 24: `eventName.Contains("Command", StringComparison.Ordinal) ||` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 25: `eventName.Contains("SaveChanges", StringComparison.Ordinal);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 27: `public void OnNext(KeyValuePair<string, object?> value)` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 28: `{` → Delimita el bloque sintáctico correspondiente.

Línea 29: `if (value.Key.StartsWith("Microsoft.EntityFrameworkCore.", StringComparison.Ordinal))` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 30: `Interlocked.Increment(ref _efEventCount);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 31: `}` → Delimita el bloque sintáctico correspondiente.

Línea 33: `public void OnError(Exception error) { }` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 34: `public void OnCompleted() { }` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 36: `public void Dispose()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 37: `{` → Delimita el bloque sintáctico correspondiente.

Línea 38: `foreach (var subscription in _subscriptions)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 39: `subscription.Dispose();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 40: `_allListenersSubscription.Dispose();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 41: `}` → Delimita el bloque sintáctico correspondiente.

Línea 42: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `EfEventCounterListener.cs`

```csharp
public sealed class EfEventCounterListener : EventListener
{
    private readonly ConcurrentDictionary<string, double> _values = new(StringComparer.Ordinal);

    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name == "Microsoft.EntityFrameworkCore")
        {
            EnableEvents(
                eventSource,
                EventLevel.LogAlways,
                EventKeywords.All,
                new Dictionary<string, string?> { ["EventCounterIntervalSec"] = "1" });
        }
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (eventData.EventName != "EventCounters" || eventData.Payload is null || eventData.Payload.Count == 0)
            return;
        if (eventData.Payload[0] is not IDictionary<string, object> payload)
            return;
        if (!payload.TryGetValue("Name", out var nameValue) || nameValue is not string name)
            return;

        double? value = null;
        if (payload.TryGetValue("Mean", out var mean) && mean is double meanValue)
            value = meanValue;
        else if (payload.TryGetValue("Increment", out var increment) && increment is double incrementValue)
            value = incrementValue;

        if (value.HasValue)
            _values[name] = value.Value;
    }
}
```

#### Explicación línea a línea — EfEventCounterListener.cs

Línea 1: `public sealed class EfEventCounterListener : EventListener` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `private readonly ConcurrentDictionary<string, double> _values = new(StringComparer.Ordinal);` → Declara una operación auxiliar usada para mantener aislada la lógica del ejemplo.

Línea 5: `protected override void OnEventSourceCreated(EventSource eventSource)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 6: `{` → Delimita el bloque sintáctico correspondiente.

Línea 7: `if (eventSource.Name == "Microsoft.EntityFrameworkCore")` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 8: `{` → Delimita el bloque sintáctico correspondiente.

Línea 9: `EnableEvents(` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 10: `eventSource,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 11: `EventLevel.LogAlways,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 12: `EventKeywords.All,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 13: `new Dictionary<string, string?> { ["EventCounterIntervalSec"] = "1" });` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 14: `}` → Delimita el bloque sintáctico correspondiente.

Línea 15: `}` → Delimita el bloque sintáctico correspondiente.

Línea 17: `protected override void OnEventWritten(EventWrittenEventArgs eventData)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 18: `{` → Delimita el bloque sintáctico correspondiente.

Línea 19: `if (eventData.EventName != "EventCounters" || eventData.Payload is null || eventData.Payload.Count == 0)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 20: `return;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 21: `if (eventData.Payload[0] is not IDictionary<string, object> payload)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 22: `return;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 23: `if (!payload.TryGetValue("Name", out var nameValue) || nameValue is not string name)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 24: `return;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 26: `double? value = null;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 27: `if (payload.TryGetValue("Mean", out var mean) && mean is double meanValue)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 28: `value = meanValue;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 29: `else if (payload.TryGetValue("Increment", out var increment) && increment is double incrementValue)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 30: `value = incrementValue;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 32: `if (value.HasValue)` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 33: `_values[name] = value.Value;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 34: `}` → Delimita el bloque sintáctico correspondiente.

Línea 35: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Implementar y estudiar `Program.cs`

```csharp
var serilog = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine(logDirectory, "aceria-.log"),
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 4096,
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 3,
        shared: false)
    .CreateLogger();

services.AddApplicationInsightsTelemetryWorkerService(options =>
{
    options.ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000";
});
```

#### Explicación línea a línea — Program.cs

Línea 1: `var serilog = new LoggerConfiguration()` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 2: `.MinimumLevel.Information()` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 3: `.Enrich.FromLogContext()` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 4: `.WriteTo.Console()` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 5: `.WriteTo.File(` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 6: `Path.Combine(logDirectory, "aceria-.log"),` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 7: `rollingInterval: RollingInterval.Day,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 8: `fileSizeLimitBytes: 4096,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 9: `rollOnFileSizeLimit: true,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 10: `retainedFileCountLimit: 3,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 11: `shared: false)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 12: `.CreateLogger();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 14: `services.AddApplicationInsightsTelemetryWorkerService(options =>` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 15: `{` → Delimita el bloque sintáctico correspondiente.

Línea 16: `options.ConnectionString = "InstrumentationKey=00000000-0000-0000-0000-000000000000";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 17: `});` → Ejecuta esta instrucción como parte del flujo del ejemplo.

### Paso 6: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.10 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Paso 7: Observar EventCounters desde otra terminal

Para mantener el proceso vivo unos segundos:

```powershell
$env:ACERIA_COUNTER_WAIT_MS=15000
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
```

La aplicación imprime el PID. Con ese valor, desde otra terminal:

```powershell
dotnet-counters monitor --process-id <PID> Microsoft.EntityFrameworkCore
```

Esta observación usa los EventCounters de EF Core 8. No se atribuyen al curso métricas introducidas en versiones posteriores.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- ILogger/Serilog conservan propiedades estructuradas.
- La rotación usa límite de tamaño y retención.
- EfDiagnosticObserver se suscribe solo al listener EF y libera subscriptions.
- EfEventCounterListener lee EventCounters de EF Core 8.
- Application Insights usa integración WorkerService y un canal local en el laboratorio.

### Reto resuelto y ampliación

**Reto:** Ejecuta la aplicación con ACERIA_COUNTER_WAIT_MS y conecta dotnet-counters al PID impreso por la consola.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Habilitar EnableSensitiveDataLogging por defecto.
- Suscribirse a DiagnosticListener y no liberar subscriptions.
- Atribuir a EF Core 8 métricas introducidas en versiones posteriores.
- Usar AddApplicationInsightsTelemetry de ASP.NET como si fuera la integración de una consola.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.10 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.10, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.10 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.11` y parte directamente de este proyecto completo.

---

## Punto 5.11 — Testing con EF Core

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.10`. La carpeta `M05/PROYECTO/5.11` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Separar unit, provider behavior, integración SQL Server y HTTP, usando migraciones reales, Respawn y WebApplicationFactory.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.11
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `tests/AceriaData.Tests/ProviderBehavior/ProviderBehaviorTests.cs`
- `tests/AceriaData.Tests/Integration/SqlServerDatabaseFixture.cs`
- `tests/AceriaData.Tests/Integration/SqlServerIntegrationTests.cs`
- `tests/AceriaData.Tests/Integration/ApiIntegrationTests.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `ProviderBehaviorTests.cs`

```csharp
[Fact]
public async Task InMemory_NoImponeClaveForaneaRelacional()
{
    var options = new DbContextOptionsBuilder<ProbeContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
        .Options;

    await using var context = new ProbeContext(options);
    context.Children.Add(new ProbeChild { ParentId = 999, Name = "huerfano" });

    var exception = await Record.ExceptionAsync(() => context.SaveChangesAsync());
    Assert.Null(exception);
}

[Fact]
public void Sqlite_NoDebeUsarseParaValidarRowVersionDeSqlServer()
{
    var options = new DbContextOptionsBuilder<ProbeContext>()
        .UseSqlite("Data Source=:memory:")
        .Options;

    using var context = new ProbeContext(options);
    var property = context.Model.FindEntityType(typeof(ProbeParent))!
        .FindProperty(nameof(ProbeParent.Version))!;

    Assert.True(property.IsConcurrencyToken);
    Assert.Equal(ValueGenerated.OnAddOrUpdate, property.ValueGenerated);
    Assert.Equal("BLOB", property.GetColumnType());
}
```

#### Explicación línea a línea — ProviderBehaviorTests.cs

Línea 1: `[Fact]` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 2: `public async Task InMemory_NoImponeClaveForaneaRelacional()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 3: `{` → Delimita el bloque sintáctico correspondiente.

Línea 4: `var options = new DbContextOptionsBuilder<ProbeContext>()` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 5: `.UseInMemoryDatabase(Guid.NewGuid().ToString("N"))` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 6: `.Options;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 8: `await using var context = new ProbeContext(options);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 9: `context.Children.Add(new ProbeChild { ParentId = 999, Name = "huerfano" });` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 11: `var exception = await Record.ExceptionAsync(() => context.SaveChangesAsync());` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 12: `Assert.Null(exception);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 13: `}` → Delimita el bloque sintáctico correspondiente.

Línea 15: `[Fact]` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 16: `public void Sqlite_NoDebeUsarseParaValidarRowVersionDeSqlServer()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 17: `{` → Delimita el bloque sintáctico correspondiente.

Línea 18: `var options = new DbContextOptionsBuilder<ProbeContext>()` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 19: `.UseSqlite("Data Source=:memory:")` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 20: `.Options;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 22: `using var context = new ProbeContext(options);` → Importa el espacio de nombres necesario para los tipos y extensiones usados por el archivo, o declara un recurso con liberación automática cuando se usa `using var`.

Línea 23: `var property = context.Model.FindEntityType(typeof(ProbeParent))!` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 24: `.FindProperty(nameof(ProbeParent.Version))!;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 26: `Assert.True(property.IsConcurrencyToken);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 27: `Assert.Equal(ValueGenerated.OnAddOrUpdate, property.ValueGenerated);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 28: `Assert.Equal("BLOB", property.GetColumnType());` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 29: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `SqlServerDatabaseFixture.cs`

```csharp
public sealed class SqlServerDatabaseFixture : IAsyncLifetime
{
    private readonly string _databaseName =
        "AceriaDB_M5_11_Tests_" + Guid.NewGuid().ToString("N")[..8];
    private Respawner? _respawner;

    public string ConnectionString =>
        $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;";

    public DbContextOptions<AceriaDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<AceriaDbContext>()
            .UseSqlServer(ConnectionString,
                sql => sql.MigrationsAssembly(typeof(AceriaDbContext).Assembly.GetName().Name))
            .Options;

    public async Task InitializeAsync()
    {
        await using var context = new AceriaDbContext(CreateOptions());
        await context.Database.MigrateAsync();

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = [new Respawn.Graph.Table("__EFMigrationsHistory")]
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner is null) throw new InvalidOperationException("Respawn no inicializado.");
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }
}
```

#### Explicación línea a línea — SqlServerDatabaseFixture.cs

Línea 1: `public sealed class SqlServerDatabaseFixture : IAsyncLifetime` → Declara la clase que encapsula la responsabilidad mostrada en este ejemplo.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `private readonly string _databaseName =` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 4: `"AceriaDB_M5_11_Tests_" + Guid.NewGuid().ToString("N")[..8];` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 5: `private Respawner? _respawner;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 7: `public string ConnectionString =>` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 8: `$"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=false;";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 10: `public DbContextOptions<AceriaDbContext> CreateOptions() =>` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 11: `new DbContextOptionsBuilder<AceriaDbContext>()` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 12: `.UseSqlServer(ConnectionString,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 13: `sql => sql.MigrationsAssembly(typeof(AceriaDbContext).Assembly.GetName().Name))` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 14: `.Options;` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 16: `public async Task InitializeAsync()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 17: `{` → Delimita el bloque sintáctico correspondiente.

Línea 18: `await using var context = new AceriaDbContext(CreateOptions());` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 19: `await context.Database.MigrateAsync();` → Aplica la cadena real de migraciones al proveedor de pruebas o despliegue.

Línea 21: `await using var connection = new SqlConnection(ConnectionString);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 22: `await connection.OpenAsync();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 23: `_respawner = await Respawner.CreateAsync(connection, new RespawnerOptions` → Inicializa Respawn con el esquema SQL Server de la base de pruebas.

Línea 24: `{` → Delimita el bloque sintáctico correspondiente.

Línea 25: `DbAdapter = DbAdapter.SqlServer,` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 26: `TablesToIgnore = [new Respawn.Graph.Table("__EFMigrationsHistory")]` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 27: `});` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 28: `}` → Delimita el bloque sintáctico correspondiente.

Línea 30: `public async Task ResetAsync()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 31: `{` → Delimita el bloque sintáctico correspondiente.

Línea 32: `if (_respawner is null) throw new InvalidOperationException("Respawn no inicializado.");` → Evalúa una condición necesaria y detiene o desvía el flujo cuando no se cumple.

Línea 33: `await using var connection = new SqlConnection(ConnectionString);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 34: `await connection.OpenAsync();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 35: `await _respawner.ResetAsync(connection);` → Limpia los datos entre pruebas según la configuración de Respawn.

Línea 36: `}` → Delimita el bloque sintáctico correspondiente.

Línea 37: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Implementar y estudiar `SqlServerIntegrationTests.cs`

```csharp
[Fact]
public async Task RowVersionSqlServer_DetectaConflictoRealEntreDosContextos()
{
    await _fixture.ResetAsync();
    int id;
    await using (var seed = new AceriaDbContext(_fixture.CreateOptions()))
    {
        var orden = new OrdenFabricacion
        {
            NumeroOrden = "OF-TEST-ROWVERSION",
            Cliente = "Inicial",
            Estado = "Pendiente",
            FechaCreacion = DateTime.UtcNow
        };
        seed.OrdenesFabricacion.Add(orden);
        await seed.SaveChangesAsync();
        id = orden.Id;
        Assert.NotEmpty(orden.RowVersion);
    }

    await using var contextA = new AceriaDbContext(_fixture.CreateOptions());
    await using var contextB = new AceriaDbContext(_fixture.CreateOptions());
    var a = await contextA.OrdenesFabricacion.SingleAsync(x => x.Id == id);
    var b = await contextB.OrdenesFabricacion.SingleAsync(x => x.Id == id);

    a.Cliente = "A";
    await contextA.SaveChangesAsync();
    b.Cliente = "B";
    await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => contextB.SaveChangesAsync());
}
```

#### Explicación línea a línea — SqlServerIntegrationTests.cs

Línea 1: `[Fact]` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 2: `public async Task RowVersionSqlServer_DetectaConflictoRealEntreDosContextos()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 3: `{` → Delimita el bloque sintáctico correspondiente.

Línea 4: `await _fixture.ResetAsync();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 5: `int id;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 6: `await using (var seed = new AceriaDbContext(_fixture.CreateOptions()))` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 7: `{` → Delimita el bloque sintáctico correspondiente.

Línea 8: `var orden = new OrdenFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 9: `{` → Delimita el bloque sintáctico correspondiente.

Línea 10: `NumeroOrden = "OF-TEST-ROWVERSION",` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 11: `Cliente = "Inicial",` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 12: `Estado = "Pendiente",` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 13: `FechaCreacion = DateTime.UtcNow` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 14: `};` → Delimita el bloque sintáctico correspondiente.

Línea 15: `seed.OrdenesFabricacion.Add(orden);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 16: `await seed.SaveChangesAsync();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 17: `id = orden.Id;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 18: `Assert.NotEmpty(orden.RowVersion);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 19: `}` → Delimita el bloque sintáctico correspondiente.

Línea 21: `await using var contextA = new AceriaDbContext(_fixture.CreateOptions());` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 22: `await using var contextB = new AceriaDbContext(_fixture.CreateOptions());` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 23: `var a = await contextA.OrdenesFabricacion.SingleAsync(x => x.Id == id);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 24: `var b = await contextB.OrdenesFabricacion.SingleAsync(x => x.Id == id);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 26: `a.Cliente = "A";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 27: `await contextA.SaveChangesAsync();` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 28: `b.Cliente = "B";` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 29: `await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => contextB.SaveChangesAsync());` → Envía al proveedor los cambios rastreados y aplica las reglas de transacción/concurrencia configuradas.

Línea 30: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 6: Implementar y estudiar `ApiIntegrationTests.cs`

```csharp
[Fact]
public async Task WebApplicationFactory_UsaApiRealYSqlServerDePruebas()
{
    await _fixture.ResetAsync();
    await using var factory = new AceriaApiFactory(_fixture.ConnectionString);
    using var client = factory.CreateClient();

    var create = await client.PostAsJsonAsync("/api/ordenes", new
    {
        NumeroOrden = "OF-HTTP-511",
        Cliente = "Cliente HTTP"
    });
    Assert.Equal(HttpStatusCode.Created, create.StatusCode);

    var response = await client.GetAsync("/api/ordenes/count");
    response.EnsureSuccessStatusCode();
    var payload = await response.Content.ReadFromJsonAsync<CountResponse>();
    Assert.NotNull(payload);
    Assert.Equal(1, payload.Total);
}
```

#### Explicación línea a línea — ApiIntegrationTests.cs

Línea 1: `[Fact]` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 2: `public async Task WebApplicationFactory_UsaApiRealYSqlServerDePruebas()` → Declara la operación pública que ejecutará o verificará este comportamiento.

Línea 3: `{` → Delimita el bloque sintáctico correspondiente.

Línea 4: `await _fixture.ResetAsync();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 5: `await using var factory = new AceriaApiFactory(_fixture.ConnectionString);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 6: `using var client = factory.CreateClient();` → Importa el espacio de nombres necesario para los tipos y extensiones usados por el archivo, o declara un recurso con liberación automática cuando se usa `using var`.

Línea 8: `var create = await client.PostAsJsonAsync("/api/ordenes", new` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 9: `{` → Delimita el bloque sintáctico correspondiente.

Línea 10: `NumeroOrden = "OF-HTTP-511",` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 11: `Cliente = "Cliente HTTP"` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 12: `});` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 13: `Assert.Equal(HttpStatusCode.Created, create.StatusCode);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 15: `var response = await client.GetAsync("/api/ordenes/count");` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 16: `response.EnsureSuccessStatusCode();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 17: `var payload = await response.Content.ReadFromJsonAsync<CountResponse>();` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 18: `Assert.NotNull(payload);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 19: `Assert.Equal(1, payload.Total);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 20: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 7: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.11 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- Moq sustituye interfaces de Application sin mockear DbSet.
- InMemory muestra explícitamente que no impone semántica relacional.
- SQLite se usa para diferencias de proveedor, no para validar rowversion SQL Server.
- LocalDB ejecuta la cadena real de migraciones y concurrencia real.
- Respawn limpia datos y conserva __EFMigrationsHistory.
- WebApplicationFactory ejecuta endpoints reales contra la base de pruebas.

### Reto resuelto y ampliación

**Reto:** Añade un test HTTP que cree dos órdenes y verifique el count tras un Reset de Respawn.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Usar InMemory como sustituto general de SQL Server.
- Afirmar que SQLite reproduce tipos, collation, funciones y rowversion de SQL Server.
- Usar EnsureCreated en lugar de migraciones para la suite de integración.
- Compartir datos entre tests sin aislamiento.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.11 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.11, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.11 OK
```

### Conexión con el siguiente punto

El siguiente estado es `5.12` y parte directamente de este proyecto completo.

---

## Punto 5.12 — Buenas prácticas y anti-patrones en persistencia empresarial

### Contexto del proyecto

Este punto continúa directamente `M05/PROYECTO/5.11`. La carpeta `M05/PROYECTO/5.12` contiene la solución completa `AceriaData.sln` y conserva las capas y migraciones existentes. El modelo oficial no cambia en este punto; se conserva la última migración real ya existente.

### Objetivo práctico

Cerrar el módulo con refactorizaciones before/after medibles: N+1, over-fetching, tracking y traducción cliente/servidor.

### Paso 1: Abrir el estado completo del punto

```powershell
cd M05/PROYECTO/5.12
dotnet restore AceriaData.sln
```

La restauración se ejecuta sobre la solución local del punto. No es necesario copiar archivos desde otros puntos: la carpeta ya contiene el estado acumulativo completo.

### Paso 2: Identificar los archivos principales

En este punto se trabajan principalmente estos archivos:
- `src/AceriaData.Infrastructure/BuenasPracticasAntiPatronesM5Diagnostico.cs`
- `tests/AceriaData.Tests/Integration/BuenasPracticasAntiPatronesM5Tests.cs`

Los archivos se estudian dentro de su capa real. Application conserva sus abstracciones y casos de uso; Infrastructure contiene EF Core y acceso a proveedor; Console compone y ejecuta; los tests se ubican fuera de las capas de producción cuando aparecen en 5.9 y 5.11.

### Paso 3: Implementar y estudiar `BuenasPracticasAntiPatronesM5Diagnostico.cs`

```csharp
private NMasUnoRefactorM5Resultado MedirNMasUno()
{
    _context.ChangeTracker.Clear();
    SqlCommandCounterInterceptor.Instance.Reset();

    var cabeceras = _context.OrdenesFabricacion
        .AsNoTracking()
        .OrderBy(o => o.Id)
        .Select(o => new { o.Id, o.NumeroOrden })
        .ToList();

    var planchasNMasUno = 0;
    foreach (var orden in cabeceras)
    {
        planchasNMasUno += _context.PlanchasAcero
            .AsNoTracking()
            .Count(p => p.OrdenId == orden.Id);
    }
    var consultasNMasUno = checked((int)SqlCommandCounterInterceptor.Instance.Count);

    _context.ChangeTracker.Clear();
    SqlCommandCounterInterceptor.Instance.Reset();
    var conInclude = _context.OrdenesFabricacion
        .AsNoTracking()
        .Include(o => o.Planchas)
        .OrderBy(o => o.Id)
        .ToList();
    var consultasInclude = checked((int)SqlCommandCounterInterceptor.Instance.Count);

    _context.ChangeTracker.Clear();
    SqlCommandCounterInterceptor.Instance.Reset();
    var proyectadas = _context.OrdenesFabricacion
        .AsNoTracking()
        .OrderBy(o => o.Id)
        .Select(o => new { o.Id, o.NumeroOrden, TotalPlanchas = o.Planchas.Count })
        .ToList();
    var consultasProyeccion = checked((int)SqlCommandCounterInterceptor.Instance.Count);

    var equivalentes =
        cabeceras.Count == conInclude.Count &&
        conInclude.Count == proyectadas.Count &&
        planchasNMasUno == conInclude.Sum(o => o.Planchas.Count) &&
        planchasNMasUno == proyectadas.Sum(o => o.TotalPlanchas);

    return new NMasUnoRefactorM5Resultado(
        cabeceras.Count, planchasNMasUno,
        consultasNMasUno, consultasInclude, consultasProyeccion,
        0, 0, 0, equivalentes, string.Empty, string.Empty);
}
```

#### Explicación línea a línea — BuenasPracticasAntiPatronesM5Diagnostico.cs

Línea 1: `private NMasUnoRefactorM5Resultado MedirNMasUno()` → Declara una operación auxiliar usada para mantener aislada la lógica del ejemplo.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `_context.ChangeTracker.Clear();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 6: `var cabeceras = _context.OrdenesFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 7: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 8: `.OrderBy(o => o.Id)` → Añade orden determinista a la consulta antes de materializar.

Línea 9: `.Select(o => new { o.Id, o.NumeroOrden })` → Proyecta únicamente la forma de datos necesaria para el resultado.

Línea 10: `.ToList();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 12: `var planchasNMasUno = 0;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 13: `foreach (var orden in cabeceras)` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 14: `{` → Delimita el bloque sintáctico correspondiente.

Línea 15: `planchasNMasUno += _context.PlanchasAcero` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 16: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 17: `.Count(p => p.OrdenId == orden.Id);` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 18: `}` → Delimita el bloque sintáctico correspondiente.

Línea 19: `var consultasNMasUno = checked((int)SqlCommandCounterInterceptor.Instance.Count);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 21: `_context.ChangeTracker.Clear();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 22: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 23: `var conInclude = _context.OrdenesFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 24: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 25: `.Include(o => o.Planchas)` → Carga la navegación indicada en la consulta relacional para evitar accesos posteriores por fila.

Línea 26: `.OrderBy(o => o.Id)` → Añade orden determinista a la consulta antes de materializar.

Línea 27: `.ToList();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 28: `var consultasInclude = checked((int)SqlCommandCounterInterceptor.Instance.Count);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 30: `_context.ChangeTracker.Clear();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 31: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 32: `var proyectadas = _context.OrdenesFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 33: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 34: `.OrderBy(o => o.Id)` → Añade orden determinista a la consulta antes de materializar.

Línea 35: `.Select(o => new { o.Id, o.NumeroOrden, TotalPlanchas = o.Planchas.Count })` → Proyecta únicamente la forma de datos necesaria para el resultado.

Línea 36: `.ToList();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 37: `var consultasProyeccion = checked((int)SqlCommandCounterInterceptor.Instance.Count);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 39: `var equivalentes =` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 40: `cabeceras.Count == conInclude.Count &&` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 41: `conInclude.Count == proyectadas.Count &&` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 42: `planchasNMasUno == conInclude.Sum(o => o.Planchas.Count) &&` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 43: `planchasNMasUno == proyectadas.Sum(o => o.TotalPlanchas);` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 45: `return new NMasUnoRefactorM5Resultado(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 46: `cabeceras.Count, planchasNMasUno,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 47: `consultasNMasUno, consultasInclude, consultasProyeccion,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 48: `0, 0, 0, equivalentes, string.Empty, string.Empty);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 49: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 4: Implementar y estudiar `BuenasPracticasAntiPatronesM5Diagnostico.cs`

```csharp
private TraduccionWhereM5Resultado MedirTraduccion()
{
    _context.ChangeTracker.Clear();
    SqlCommandCounterInterceptor.Instance.Reset();

    var falloTraduccion = false;
    try
    {
        _ = _context.OrdenesFabricacion
            .AsNoTracking()
            .Where(o => EsPendiente(o.Estado))
            .ToList();
    }
    catch (InvalidOperationException)
    {
        falloTraduccion = true;
    }

    var comandosAntesCliente = SqlCommandCounterInterceptor.Instance.Count;

    SqlCommandCounterInterceptor.Instance.Reset();
    var pendientesCliente = _context.OrdenesFabricacion
        .AsNoTracking()
        .AsEnumerable()
        .Count(o => EsPendiente(o.Estado));
    var comandosCliente = checked((int)SqlCommandCounterInterceptor.Instance.Count);

    return new TraduccionWhereM5Resultado(
        falloTraduccion,
        checked((int)comandosAntesCliente),
        pendientesCliente > 0,
        comandosCliente,
        pendientesCliente);
}
```

#### Explicación línea a línea — BuenasPracticasAntiPatronesM5Diagnostico.cs

Línea 1: `private TraduccionWhereM5Resultado MedirTraduccion()` → Declara una operación auxiliar usada para mantener aislada la lógica del ejemplo.

Línea 2: `{` → Delimita el bloque sintáctico correspondiente.

Línea 3: `_context.ChangeTracker.Clear();` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 4: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 6: `var falloTraduccion = false;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 7: `try` → Abre el bloque en el que se espera que pueda producirse la excepción que se quiere estudiar.

Línea 8: `{` → Delimita el bloque sintáctico correspondiente.

Línea 9: `_ = _context.OrdenesFabricacion` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 10: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 11: `.Where(o => EsPendiente(o.Estado))` → Añade el predicado a la consulta; EF Core intentará traducirlo al proveedor antes de materializar.

Línea 12: `.ToList();` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 13: `}` → Delimita el bloque sintáctico correspondiente.

Línea 14: `catch (InvalidOperationException)` → Captura la excepción específica para aplicar la política de resolución prevista.

Línea 15: `{` → Delimita el bloque sintáctico correspondiente.

Línea 16: `falloTraduccion = true;` → Asigna el valor necesario para preparar o registrar el estado de esta operación.

Línea 17: `}` → Delimita el bloque sintáctico correspondiente.

Línea 19: `var comandosAntesCliente = SqlCommandCounterInterceptor.Instance.Count;` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 21: `SqlCommandCounterInterceptor.Instance.Reset();` → Reinicia el contador para que solo se midan los comandos de este escenario.

Línea 22: `var pendientesCliente = _context.OrdenesFabricacion` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 23: `.AsNoTracking()` → Ejecuta una lectura sin dejar entidades en el ChangeTracker.

Línea 24: `.AsEnumerable()` → Cruza de forma explícita la frontera de consulta remota a evaluación en memoria.

Línea 25: `.Count(o => EsPendiente(o.Estado));` → Materializa o agrega la consulta en este punto, provocando la ejecución contra el proveedor cuando procede.

Línea 26: `var comandosCliente = checked((int)SqlCommandCounterInterceptor.Instance.Count);` → Calcula o conserva un valor que forma parte de la preparación, ejecución o verificación del escenario.

Línea 28: `return new TraduccionWhereM5Resultado(` → Devuelve la evidencia recopilada por el escenario para que el caso de uso o el test pueda comprobarla.

Línea 29: `falloTraduccion,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 30: `checked((int)comandosAntesCliente),` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 31: `pendientesCliente > 0,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 32: `comandosCliente,` → Forma parte de la implementación concreta del comportamiento explicado en este paso.

Línea 33: `pendientesCliente);` → Ejecuta esta instrucción como parte del flujo del ejemplo.

Línea 34: `}` → Delimita el bloque sintáctico correspondiente.

### Paso 5: Implementar y estudiar `BuenasPracticasAntiPatronesM5Tests.cs`

```csharp
Assert.True(resultado.NMasUno.ResultadosEquivalentes);
Assert.True(resultado.NMasUno.ConsultasNMasUno > resultado.NMasUno.ConsultasInclude);
Assert.True(resultado.NMasUno.ConsultasNMasUno > resultado.NMasUno.ConsultasProyeccion);
Assert.Equal(0, resultado.NMasUno.TrackingInclude);
Assert.Equal(0, resultado.NMasUno.TrackingProyeccion);

Assert.True(resultado.OverFetching.ResultadosEquivalentes);
Assert.True(resultado.OverFetching.ColumnasEntidadCompleta > resultado.OverFetching.ColumnasProyeccion);
Assert.True(resultado.OverFetching.TrackingEntidadCompleta > 0);
Assert.Equal(0, resultado.OverFetching.TrackingProyeccion);
Assert.True(resultado.OverFetching.SqlProyeccionExcluyeRowVersion);

Assert.True(resultado.Traduccion.MetodoNoTraducibleFalla);
Assert.Equal(0, resultado.Traduccion.ComandosEmitidosAntesDelFallo);
Assert.True(resultado.Traduccion.EvaluacionClienteExplicitaFunciona);
Assert.Equal(1, resultado.Traduccion.ComandosEvaluacionCliente);
```

#### Explicación línea a línea — BuenasPracticasAntiPatronesM5Tests.cs

Línea 1: `Assert.True(resultado.NMasUno.ResultadosEquivalentes);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 2: `Assert.True(resultado.NMasUno.ConsultasNMasUno > resultado.NMasUno.ConsultasInclude);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 3: `Assert.True(resultado.NMasUno.ConsultasNMasUno > resultado.NMasUno.ConsultasProyeccion);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 4: `Assert.Equal(0, resultado.NMasUno.TrackingInclude);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 5: `Assert.Equal(0, resultado.NMasUno.TrackingProyeccion);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 7: `Assert.True(resultado.OverFetching.ResultadosEquivalentes);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 8: `Assert.True(resultado.OverFetching.ColumnasEntidadCompleta > resultado.OverFetching.ColumnasProyeccion);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 9: `Assert.True(resultado.OverFetching.TrackingEntidadCompleta > 0);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 10: `Assert.Equal(0, resultado.OverFetching.TrackingProyeccion);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 11: `Assert.True(resultado.OverFetching.SqlProyeccionExcluyeRowVersion);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 13: `Assert.True(resultado.Traduccion.MetodoNoTraducibleFalla);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 14: `Assert.Equal(0, resultado.Traduccion.ComandosEmitidosAntesDelFallo);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 15: `Assert.True(resultado.Traduccion.EvaluacionClienteExplicitaFunciona);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

Línea 16: `Assert.Equal(1, resultado.Traduccion.ComandosEvaluacionCliente);` → Expresa una condición observable que la prueba exige para considerar correcto el comportamiento.

### Paso 6: Compilar y ejecutar

```powershell
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes --project src/AceriaData.Infrastructure --startup-project src/AceriaData.Console --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj --configuration Release --no-build
```

La ejecución debe finalizar con `5.12 OK`. Si una comprobación interna falla, la aplicación lanza una excepción y el punto no se considera correctamente reproducido.

### Diagnóstico técnico

La práctica se considera correcta cuando se observan estas condiciones:
- N+1, Include y proyección producen resultados funcionalmente equivalentes.
- Se cuentan roundtrips reales.
- Include/proyección de lectura no dejan tracking.
- La proyección materializa menos columnas y excluye RowVersion.
- Un método no traducible en Where falla antes de emitir SQL.
- AsEnumerable hace explícita la frontera de evaluación cliente.

### Reto resuelto y ampliación

**Reto:** Añade una variante con AsSplitQuery para dos colecciones, mide sus roundtrips y explica el trade-off frente a una única consulta.

Para resolverlo, conserva la misma regla del módulo: primero define qué resultado funcional debe mantenerse; después captura SQL, número de comandos, tracking, historial de migraciones o estado transaccional según corresponda. No uses una medición temporal aislada como sustituto de la evidencia técnica.

### Errores comunes
- Aplicar Include siempre para resolver N+1.
- Aplicar AsSplitQuery automáticamente con varias colecciones.
- Tratar Data Annotations como anti-patrón por definición.
- Afirmar que un método personalizado en Where se filtra silenciosamente en memoria.
- Tratar Repository/UoW como requisito universal.

### Cierre acumulativo

1. Ejecuta `dotnet build` en la solución del punto.
2. Ejecuta la aplicación y comprueba el marcador `5.12 OK`.
3. Conserva las migraciones existentes; solo 5.2 añade el cambio de modelo oficial de M5.
4. Comprueba que Application continúa sin depender de Entity Framework Core ni de Infrastructure.
5. Antes de avanzar, relaciona el resultado con el SQL, la transacción, el historial o la prueba que lo demuestra.

### Resultado esperado

Al finalizar el punto 5.12, AceriaData debe compilar, ejecutarse sobre SQL Server LocalDB y demostrar de forma observable el comportamiento descrito. El marcador final es:

```text
5.12 OK
```

### Cierre del módulo

Con 5.12 se completa la secuencia práctica del Módulo 5. El proyecto final contiene concurrencia optimista, resolución de conflictos, transacciones, estrategias de despliegue, scripts idempotentes, trabajo en equipo con migraciones, Repository/UoW como decisión arquitectónica, observabilidad, una estrategia de testing multicapa y refactorizaciones before/after de anti-patrones de persistencia.
