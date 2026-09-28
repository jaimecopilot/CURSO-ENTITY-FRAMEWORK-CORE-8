# Curso Profesional de Entity Framework Core 8

# Módulo 3 — Consultas con LINQ

**Autor: JAIME GALLO**

Este módulo continúa el proyecto AceriaData desde el cierre de M2.12. Se conserva Clean Architecture, SQL Server LocalDB y la historia real de migraciones. Las consultas se contrastan con el SQL generado por EF Core 8 y con resultados E2E reproducibles.

## Punto 3.1 — Fundamentos de LINQ to Entities

### Objetivos de aprendizaje

- Comprender qué es LINQ y qué aporta LINQ to Entities.
- Diferenciar IEnumerable<T> e IQueryable<T>.
- Comprender ejecución diferida y materialización.
- Reconocer expresiones traducibles a SQL.
- Inspeccionar el SQL sin ejecutar la consulta.

### Teoría

#### LINQ y LINQ to Entities

LINQ forma parte del lenguaje C# y permite expresar filtros, proyecciones, ordenaciones, agrupaciones y agregaciones sobre diferentes orígenes. Cuando el origen es un DbSet de EF Core, el proveedor recibe un árbol de expresión y trata de traducirlo a SQL. La consulta no debe confundirse con una colección ya materializada.

#### IEnumerable frente a IQueryable

IEnumerable describe una secuencia que se recorre en .NET. IQueryable añade un proveedor y un árbol de expresión. Si se llama a ToList antes del filtro, el resto del pipeline se ejecuta sobre objetos en memoria; si se mantiene IQueryable, Where y OrderBy pueden trasladarse al servidor.

#### Ejecución diferida

Construir una consulta no significa ejecutarla. Operadores como Where, Select y OrderBy componen el árbol. La ejecución llega al materializar con ToList, FirstOrDefault, Count, Any o al enumerar. Esta separación permite añadir condiciones de forma incremental antes de enviar SQL.

#### Materialización y número de consultas

Cada operación terminal puede provocar una consulta independiente. Llamar sucesivamente a ToList, Count y Any sobre el mismo IQueryable no reutiliza automáticamente el resultado. En producción se decide si conviene una única materialización o agregaciones independientes en servidor.

#### Traducción a SQL

EF Core 8 traduce un conjunto amplio de operadores y métodos, pero no cualquier método C#. Cuando una expresión no tiene traducción, debe reformularse o materializarse de forma explícita sabiendo que el procesamiento pasará al cliente.

#### ToQueryString

ToQueryString permite observar el SQL previsto sin materializar resultados. En AceriaData se encapsula en Infrastructure para que Application no tenga una referencia a EF Core. El contrato IQueryable se conserva temporalmente en 3.1 como seam docente y se elimina del contrato final en 3.12.

#### Ejemplo representativo

```csharp
var consulta = context.OrdenesFabricacion
    .Where(o => o.Cliente == "Constructora del Norte")
    .OrderBy(o => o.FechaCreacion);

var sql = consulta.ToQueryString();
var ordenes = consulta.ToList();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.1`, el caso de uso **ConsultasLinqUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Fundamentos de LINQ to Entities**.
- Métodos o técnicas trazados en código: `Consulta`, `ObtenerSqlFundamentos`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.1 OK`.

---

## Punto 3.2 — Consultas básicas: Where, OrderBy y ThenBy

### Objetivos de aprendizaje

- Aplicar Where con condiciones simples y compuestas.
- Ordenar con OrderBy y OrderByDescending.
- Añadir criterios secundarios con ThenBy y ThenByDescending.
- Filtrar rangos de fechas sin materializar prematuramente.
- Analizar WHERE y ORDER BY generados.

### Teoría

#### Where y parametrización

Where añade predicados al árbol de consulta. EF Core parametriza los valores capturados y genera WHERE en SQL. Los operadores && y || se convierten en AND y OR conservando la semántica booleana.

#### Cadenas y fechas

StartsWith, EndsWith y Contains tienen traducciones conocidas en SQL Server. Las comparaciones de DateTime se parametrizan. Conviene evitar transformaciones innecesarias de la columna, como aplicar funciones que impidan el uso eficiente de índices.

#### OrderBy

OrderBy inicia el ordenamiento y OrderByDescending lo hace en sentido descendente. Un segundo OrderBy reemplaza el anterior; para ampliar el orden se usa ThenBy o ThenByDescending.

#### ThenBy

ThenBy solo tiene sentido después de un orden primario. El SQL resultante presenta varias expresiones en ORDER BY. El checkpoint combina estado, cliente y fecha para que el resultado sea determinista.

#### Materialización al final

La consulta debe permanecer como IQueryable hasta que filtros y ordenaciones estén compuestos. ToList al principio carga filas innecesarias y mueve trabajo al proceso .NET.

#### Encapsulación

El repositorio incorpora métodos específicos para las consultas de negocio. Así, Application pide 'pendientes por cliente' o 'órdenes por rango' sin conocer DbContext ni depender de Entity Framework Core.

#### Ejemplo representativo

```csharp
var resultado = context.OrdenesFabricacion
    .Where(o => o.Estado == "Pendiente")
    .OrderBy(o => o.Cliente)
    .ThenByDescending(o => o.FechaCreacion)
    .ToList();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.2`, el caso de uso **ConsultasBasicasUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Consultas básicas: Where, OrderBy y ThenBy**.
- Métodos o técnicas trazados en código: `ObtenerPendientesPorCliente`, `ObtenerPorEstadoOrdenadasPorFecha`, `ObtenerPorRangoDeFechas`, `ObtenerPorClienteYRangoDeFechas`, `ObtenerSqlConsultaBasica`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.2 OK`.

---

## Punto 3.3 — Proyecciones con Select y tipos anónimos

### Objetivos de aprendizaje

- Comprender la proyección con Select.
- Usar tipos anónimos en ámbitos locales.
- Crear DTOs con nombre para cruzar capas.
- Aplicar Distinct sobre proyecciones.
- Reducir columnas y materialización innecesaria.

### Teoría

#### Qué es proyectar

Select transforma cada fila lógica en la forma necesaria para el consumidor. En EF Core, una proyección bien traducida permite que SQL Server devuelva solo las columnas necesarias.

#### Tipos anónimos

Los tipos anónimos son útiles dentro de un método o para inspección local. No son adecuados como contrato entre capas porque el tipo no puede nombrarse públicamente.

#### DTOs con nombre

AceriaData coloca los DTOs en Application. Infrastructure proyecta directamente desde la consulta hacia esos DTOs, evitando cargar una entidad completa para después copiar sus propiedades.

#### Distinct

Select seguido de Distinct produce SELECT DISTINCT cuando la traducción lo permite. En el ejemplo se obtiene la lista de clientes únicos y se ordena de forma determinista.

#### Proyección de agregados

Una proyección puede incluir subconsultas agregadas, como el número de planchas o la suma de pesos. Se usan tipos anulables y coalescencia cuando una colección puede estar vacía.

#### Lectura y tracking

Las proyecciones puras no necesitan entidades modificables. AsNoTracking comunica la intención de lectura y evita trabajo del Change Tracker cuando se materializan entidades o shapes que lo requieran.

#### Ejemplo representativo

```csharp
var resumenes = context.OrdenesFabricacion
    .AsNoTracking()
    .Select(o => new OrdenResumenDto
    {
        NumeroOrden = o.NumeroOrden,
        Cliente = o.Cliente,
        Estado = o.Estado,
        FechaCreacion = o.FechaCreacion
    })
    .ToList();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.3`, el caso de uso **ProyeccionesUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Proyecciones con Select y tipos anónimos**.
- Métodos o técnicas trazados en código: `ObtenerClientesUnicos`, `ObtenerResumenes`, `ObtenerResumenesPorEstado`, `ObtenerOrdenesConTotales`, `ObtenerSqlProyeccion`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.3 OK`.

---

## Punto 3.4 — Proyecciones a DTOs

### Objetivos de aprendizaje

- Profundizar en DTOs inicializables.
- Proyectar colecciones de navegación.
- Construir DTOs anidados.
- Tratar relaciones opcionales.
- Entender cuándo SelectMany aplana una colección.

### Teoría

#### DTO como contrato de lectura

Un DTO representa el shape que necesita un caso de uso. No tiene por qué coincidir con la entidad persistida y ayuda a evitar que capas superiores dependan del modelo de EF Core.

#### Colecciones anidadas

EF Core 8 puede materializar una colección proyectada dentro del DTO principal. En AceriaData, `OrdenCompletaDto` es el ejemplo acumulativo: combina los datos básicos de la orden, una colección de `PlanchaDto` y un `DetalleDto` opcional. El SQL y el shaper colaboran para reconstruir la jerarquía sin convertirla en una entidad rastreada.

#### Relaciones opcionales

Detalle es opcional. La proyección comprueba null antes de crear DetalleDto. La nulabilidad del DTO debe reflejar la cardinalidad real del modelo.

#### SelectMany

SelectMany aplana colecciones. Es útil cuando el consumidor quiere una fila por combinación orden-plancha; no sustituye una proyección jerárquica cuando se quiere conservar la agrupación por orden.

#### Constructor o inicializador

EF Core puede proyectar a constructores sencillos, pero el curso usa inicializadores para hacer visible cada mapeo. La lógica de negocio no debe esconderse dentro del constructor de un DTO de lectura.

#### SQL observable

El método ObtenerSqlProyeccionNavegacion permite inspeccionar la traducción que EF Core 8 produce para la colección relacionada en lugar de afirmar un SQL fijo escrito a mano.

#### Ejemplo representativo

```csharp
var datos = context.OrdenesFabricacion
    .Select(o => new OrdenConPlanchasDto
    {
        NumeroOrden = o.NumeroOrden,
        Cliente = o.Cliente,
        Planchas = o.Planchas
            .Select(p => new PlanchaDto { Espesor = p.Espesor, Peso = p.Peso })
            .ToList()
    })
    .ToList();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.4`, el caso de uso **ProyeccionesDtoUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Proyecciones a DTOs**.
- Métodos o técnicas trazados en código: `ObtenerOrdenesConPlanchas`, `ObtenerOrdenesConDetalle`, `ObtenerOrdenesCompletas`, `ObtenerSqlProyeccionNavegacion`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.4 OK`.

---

## Punto 3.5 — Consultas de agregación: Count, Sum, Average, Min y Max

### Objetivos de aprendizaje

- Usar Count, Any y All.
- Aplicar Sum, Average, Min y Max.
- Tratar colecciones vacías.
- Agrupar para obtener agregados por clave.
- Relacionar LINQ con funciones SQL.

### Teoría

#### Agregaciones escalares

Count, Sum, Average, Min y Max son operaciones terminales que EF Core intenta resolver en el servidor. La aplicación recibe un valor escalar en lugar de todas las filas.

#### Any frente a Count

Cuando solo se necesita saber si existe alguna fila, Any expresa mejor la intención y normalmente se traduce a EXISTS. Count se reserva para cuando el número exacto es el dato requerido.

#### Colecciones vacías

Sum, Average, Min y Max pueden necesitar tratamiento especial cuando no hay filas. El repositorio proyecta a decimal nullable y usa coalescencia para obtener un valor definido.

#### GroupBy con agregados

GroupBy es especialmente traducible cuando la proyección contiene la clave y agregados. AceriaData obtiene resúmenes por cliente, estado y mes sin cargar entidades completas.

#### Tipos numéricos

Peso es decimal y las operaciones mantienen decimal para no introducir conversiones binarias innecesarias. La decisión de tipo forma parte del contrato del dominio.

#### Coste y plan de ejecución

Que una consulta se traduzca no garantiza que sea barata. Índices, cardinalidad y filtros influyen. El Módulo 4 reutilizará estas consultas para estudiar rendimiento.

#### Ejemplo representativo

```csharp
var total = context.PlanchasAcero
    .Select(p => (decimal?)p.Peso)
    .Sum() ?? 0m;

var existe = context.OrdenesFabricacion.Any();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.5`, el caso de uso **AgregacionesUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Consultas de agregación: Count, Sum, Average, Min y Max**.
- Métodos o técnicas trazados en código: `ContarOrdenes`, `ContarOrdenesPorEstado`, `ExisteAlgunaOrden`, `ObtenerPesoTotalDePlanchas`, `ObtenerPesoPromedioDePlanchas`, `ObtenerResumenPorCliente`, `ObtenerResumenPorEstado`, `ObtenerResumenMensual`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.5 OK`.

---

## Punto 3.6 — Agrupaciones con proyección

### Objetivos de aprendizaje

- Agrupar por una y varias claves.
- Aplicar HAVING con Where después de GroupBy.
- Proyectar grupos a DTOs.
- Combinar resumen y detalle sin N+1 accidental.
- Inspeccionar GROUP BY real generado por EF Core.

### Teoría

#### Agrupación con proyección

GroupBy seguido de Select permite convertir cada grupo en un resumen. La clave puede ser una propiedad o un objeto anónimo con varias claves.

#### HAVING

Un Where aplicado al grupo después de GroupBy y basado en un agregado se traduce a HAVING cuando el proveedor puede hacerlo. En el ejercicio se conservan únicamente los grupos cliente-estado con más de una orden.

#### Varias claves

Agrupar por new { Cliente, Estado } genera una clave compuesta lógica. La proyección recupera g.Key.Cliente y g.Key.Estado.

#### Colecciones internas

Una colección interna no debe describirse automáticamente como N+1. En este checkpoint el resumen con órdenes usa dos consultas acotadas: una para cabeceras agrupadas y otra para las filas proyectadas; la composición final se hace en memoria de forma explícita.

#### Peso por grupo

Para mantener la traducción robusta, el peso se calcula con una consulta agrupada sobre el aplanado orden-plancha y se combina con los grupos de órdenes. Se evita depender de una forma de traducción más frágil.

#### Verificación de SQL

ObtenerSqlAgrupacionClienteEstado usa una consulta puramente agregada y ToQueryString. La práctica analiza el SQL que EF Core 8 produce realmente en el entorno de prueba.

#### Ejemplo representativo

```csharp
var consulta = context.OrdenesFabricacion
    .GroupBy(o => new { o.Cliente, o.Estado })
    .Where(g => g.Count() > 1)
    .Select(g => new { g.Key.Cliente, g.Key.Estado, Total = g.Count() });
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.6`, el caso de uso **AgrupacionesUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Agrupaciones con proyección**.
- Métodos o técnicas trazados en código: `ObtenerResumenPorClienteConOrdenes`, `ObtenerResumenPorClienteYEstado`, `ObtenerResumenPorClienteYEstadoConFiltro`, `ObtenerResumenMensualConOrdenes`, `ObtenerSqlAgrupacionClienteEstado`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.6 OK`.

---

## Punto 3.7 — Joins y navegación en consultas

### Objetivos de aprendizaje

- Diferenciar Join explícito y navegación.
- Construir INNER JOIN y LEFT JOIN.
- Usar GroupJoin/DefaultIfEmpty conceptualmente.
- Combinar relaciones sin perder cardinalidad.
- Comparar SQL generado.

### Teoría

#### Join explícito

Join relaciona dos secuencias por claves y se traduce normalmente a INNER JOIN. Solo aparecen combinaciones con coincidencia.

#### LEFT JOIN

El patrón GroupJoin más DefaultIfEmpty permite conservar la fila principal cuando no hay elemento relacionado. El DTO usa propiedades anulables para representar la ausencia de plancha.

#### Navegaciones

Cuando el modelo tiene propiedades de navegación, una proyección sobre ellas suele ser más legible que escribir las claves manualmente. EF Core conoce las relaciones definidas en el modelo.

#### Joins de varias relaciones

Las relaciones uno a uno y muchos a muchos se proyectan sin exponer DbContext fuera de Infrastructure. AceriaData combina órdenes con detalle y aleaciones.

#### Cardinalidad y duplicación

Una orden con dos planchas genera dos filas en un join plano. Esa duplicación no es un error: es la cardinalidad relacional. El consumidor decide si quiere forma plana o jerárquica.

#### Comparación de SQL

El checkpoint imprime ToQueryString para el Join explícito. Las afirmaciones sobre INNER/LEFT JOIN se contrastan con la salida real del proveedor SQL Server.

#### Ejemplo representativo

```csharp
var consulta = context.OrdenesFabricacion
    .Join(context.PlanchasAcero,
        o => o.Id,
        p => p.OrdenId,
        (o, p) => new { o.NumeroOrden, p.Espesor, p.Peso });
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.7`, el caso de uso **JoinsUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Joins y navegación en consultas**.
- Métodos o técnicas trazados en código: `ObtenerJoinOrdenesPlanchas`, `ObtenerLeftJoinOrdenesPlanchas`, `ObtenerOrdenesConDetalleJoin`, `ObtenerOrdenesConAleaciones`, `ObtenerSqlJoinExplicito`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.7 OK`.

---

## Punto 3.8 — Eager Loading con Include y ThenInclude

### Objetivos de aprendizaje

- Cargar relaciones con Include.
- Encadenar ThenInclude.
- Aplicar Filtered Include.
- Usar AsSplitQuery cuando interesa dividir la carga.
- Comprender AutoInclude y sus implicaciones.

### Teoría

#### Eager Loading

Include pide a EF Core que cargue una navegación junto con la entidad principal. Es adecuado cuando la relación se necesitará inmediatamente y se quieren evitar accesos posteriores impredecibles.

#### ThenInclude

ThenInclude continúa la ruta desde una navegación incluida. En AceriaData se usa OrdenesAleaciones → Aleacion para materializar la relación many-to-many explícita.

#### Filtered Include

Una colección incluida puede filtrar y ordenar un subconjunto compatible. El checkpoint usa AsNoTracking para evitar que navigation fix-up mezcle entidades ya rastreadas con el filtro.

#### AsSplitQuery

Cuando una consulta incluye relaciones que pueden multiplicar filas, AsSplitQuery separa la carga en varias sentencias coordinadas. Se intercambia un único result set grande por varias consultas controladas.

#### AutoInclude

AutoInclude puede configurarse en el modelo para cargar siempre una navegación. Se estudia como opción, pero AceriaData no lo activa globalmente en este módulo para que cada consulta haga explícito el coste de carga.

#### SQL y shape

ToQueryString permite observar el SQL de Include. Con AsSplitQuery, la cadena puede mostrar advertencias o la primera sentencia; por eso la validación E2E comprueba además la forma materializada.

#### Ejemplo representativo

```csharp
var ordenes = context.OrdenesFabricacion
    .Include(o => o.OrdenesAleaciones)
    .ThenInclude(oa => oa.Aleacion)
    .AsSplitQuery()
    .ToList();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.8`, el caso de uso **CargaEagerUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Eager Loading con Include y ThenInclude**.
- Métodos o técnicas trazados en código: `ObtenerOrdenesConPlanchasInclude`, `ObtenerOrdenesConAleacionesInclude`, `ObtenerOrdenesConPlanchasPesadasInclude`, `ObtenerOrdenesConPlanchasYDetalleSplitQuery`, `ObtenerSqlInclude`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.8 OK`.

---

## Punto 3.9 — Lazy Loading: configuración, funcionamiento y riesgos

### Objetivos de aprendizaje

- Configurar Lazy Loading con proxies.
- Marcar navegaciones como virtual.
- Comprender carga bajo demanda.
- Demostrar el riesgo N+1.
- Limitar Lazy Loading a un contexto docente controlado.

### Teoría

#### Carga Lazy

Lazy Loading retrasa la consulta de una navegación hasta que el código accede a ella. Con proxies, EF Core crea tipos derivados que interceptan propiedades virtuales.

#### Configuración

El checkpoint 3.9 añade Microsoft.EntityFrameworkCore.Proxies y UseLazyLoadingProxies. Las navegaciones del dominio se vuelven virtuales únicamente a partir de este estado.

#### N+1

Si primero se cargan N órdenes y después se accede a una colección Lazy en un bucle, puede ejecutarse una consulta por orden además de la consulta inicial. Ese patrón debe medirse y evitarse cuando escala.

#### Ciclo de vida del DbContext

El proxy necesita un contexto vivo para cargar la relación. Acceder después de disponer el DbContext falla o no puede consultar. Esto hace que Lazy Loading sea especialmente delicado en aplicaciones web.

#### Serialización

Navegaciones bidireccionales y carga automática pueden provocar grafos grandes o ciclos durante la serialización. Los DTOs de lectura reducen ese riesgo.

#### Decisión del curso

3.9 conserva Lazy Loading solo como demostración. En 3.10 se desactiva la configuración de proxies y se vuelve a una carga explícita y observable.

#### Ejemplo representativo

```csharp
services.AddDbContext<AceriaDbContext>(o =>
    o.UseLazyLoadingProxies()
     .UseSqlServer(connectionString));

// La navegación debe ser virtual.
public virtual List<PlanchaAcero> Planchas { get; set; } = new();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.9`, el caso de uso **CargaLazyUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

La configuración con proxies existe únicamente en 3.9. El punto 3.10 vuelve a desactivar `UseLazyLoadingProxies()` para que la carga explícita no quede contaminada por accesos automáticos.

### Resumen de la teoría

- Concepto central: **Lazy Loading: configuración, funcionamiento y riesgos**.
- Métodos o técnicas trazados en código: `ObtenerTodasSinInclude`, `UseLazyLoadingProxies`, `virtual`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.9 OK`.

---

## Punto 3.10 — Explicit Loading

### Objetivos de aprendizaje

- Cargar colecciones con Entry().Collection().Load().
- Cargar referencias con Entry().Reference().Load().
- Consultar IsLoaded antes de cargar.
- Filtrar con Query() antes de Load().
- Contrastar carga explícita con Eager y Lazy.

### Teoría

#### Carga explícita

Explicit Loading mantiene el control en código: primero se obtiene la entidad y después se decide qué navegación cargar. No hay acceso automático por proxy.

#### Collection y Reference

Collection representa una navegación de colección y Reference una navegación escalar. Load ejecuta la consulta necesaria y EF Core realiza relationship fix-up sobre la entidad rastreada.

#### IsLoaded

IsLoaded permite comprobar si la navegación ya está materializada y evita una carga redundante. El repositorio usa esta comprobación antes de Load en el camino completo.

#### Query

Collection(...).Query() expone la consulta asociada a esa navegación para añadir Where, OrderBy u otros operadores antes de Load. El ejemplo carga solo planchas que superan un peso mínimo.

#### Tracking

La carga explícita trabaja naturalmente con entidades rastreadas. Para consultas de solo lectura masivas, proyección o Eager Loading con AsNoTracking puede ser más apropiado.

#### Comparación

Eager carga de antemano, Lazy carga al acceder y Explicit carga cuando el código lo ordena. No hay un ganador universal; la elección depende del patrón de acceso y del coste aceptable.

#### Ejemplo representativo

```csharp
var entry = context.Entry(orden);
if (!entry.Collection(o => o.Planchas).IsLoaded)
    entry.Collection(o => o.Planchas).Load();

entry.Collection(o => o.Planchas)
    .Query()
    .Where(p => p.Peso >= 300m)
    .Load();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.10`, el caso de uso **CargaExplicitaUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Explicit Loading**.
- Métodos o técnicas trazados en código: `ObtenerConCargaExplicita`, `ObtenerConPlanchasPesadasExplicitas`, `IsLoaded`, `Query().Where`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.10 OK`.

---

## Punto 3.11 — Composición de consultas y ejecución diferida

### Objetivos de aprendizaje

- Construir consultas incrementales.
- Aplicar filtros condicionales.
- Ordenar dinámicamente.
- Paginar con Skip y Take.
- Conservar ejecución diferida hasta la materialización.

### Teoría

#### Composición incremental

Una consulta base puede almacenarse en una variable IQueryable dentro de Infrastructure y ampliarse según parámetros opcionales. Cada asignación construye una expresión nueva; no ejecuta SQL por sí sola.

#### Filtros condicionales

Cliente, estado y fecha se agregan solo si el parámetro está presente. El resultado es una única sentencia con los filtros aplicables.

#### Ordenación dinámica

Un switch selecciona la expresión de orden según una clave permitida. No se concatenan nombres de columna en SQL; se construyen expresiones LINQ tipadas.

#### Paginación

Skip y Take se aplican después de un orden determinista. SQL Server traduce el patrón a OFFSET/FETCH u otra forma equivalente según la consulta.

#### Proyección antes de materializar

El pipeline proyecta a OrdenResumenDto antes de ToList, de modo que solo se transportan las columnas necesarias.

#### SQL y resultado juntos

El repositorio devuelve un DTO que contiene ToQueryString y los elementos materializados. Así la práctica puede contrastar exactamente la consulta prevista con el resultado E2E.

#### Ejemplo representativo

```csharp
IQueryable<OrdenFabricacion> consulta = context.OrdenesFabricacion.AsNoTracking();
if (cliente is not null) consulta = consulta.Where(o => o.Cliente == cliente);
consulta = consulta.OrderByDescending(o => o.FechaCreacion);
var pagina = consulta.Skip(0).Take(10).ToList();
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.11`, el caso de uso **ComposicionConsultasUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

### Resumen de la teoría

- Concepto central: **Composición de consultas y ejecución diferida**.
- Métodos o técnicas trazados en código: `BuscarOrdenes`, `Skip`, `Take`, `ToQueryString`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.11 OK`.

---

## Punto 3.12 — Buenas prácticas en el acceso a datos y composición de consultas

### Objetivos de aprendizaje

- Consolidar decisiones de acceso a datos.
- Preferir proyección y AsNoTracking para lecturas.
- Usar Any para existencia.
- Aplicar AsSplitQuery de forma consciente.
- Encapsular IQueryable dentro de Infrastructure.

### Teoría

#### Contrato final

El seam IQueryable que se introdujo con fines docentes en 3.1 deja de formar parte de IOrdenRepositorio en 3.12. La composición flexible sigue existiendo, pero vive dentro de Infrastructure.

#### AsNoTracking

Cuando la aplicación no va a modificar las entidades, AsNoTracking reduce el trabajo del Change Tracker. En proyecciones se combina con un shape mínimo de DTO.

#### Any para existencia

Any expresa una pregunta booleana y evita contar filas cuando solo interesa saber si hay al menos una coincidencia.

#### Proyección

La lectura optimizada de pendientes selecciona NumeroOrden, Cliente, Estado y FechaCreacion. No se cargan columnas ni relaciones que el caso de uso no consume.

#### Split Query

El método de lectura con planchas y detalle usa AsSplitQuery para controlar la expansión del result set. Esta decisión debe medirse en escenarios reales, porque añade round-trips.

#### FirstOrDefault

Cuando 'no encontrado' forma parte del flujo normal, FirstOrDefault permite representar la ausencia con null y obliga al consumidor a manejarla.

#### Cierre del módulo

M3 termina con consultas encapsuladas, SQL observable, E2E determinista y separación de capas. El Módulo 4 puede estudiar índices, tracking, compilación y rendimiento sobre una base ya coherente.

#### Ejemplo representativo

```csharp
var resumenes = context.OrdenesFabricacion
    .AsNoTracking()
    .Where(o => o.Estado == "Pendiente")
    .Select(o => new OrdenResumenDto { NumeroOrden = o.NumeroOrden, Cliente = o.Cliente })
    .ToList();

var existe = context.OrdenesFabricacion.Any(o => o.Estado == "Pendiente");
```

#### Aplicación a AceriaData

En el checkpoint `M03/PROYECTO/3.12`, el caso de uso **BuenasPracticasUseCase** ejecuta la materia del punto contra una base LocalDB reconstruida con `Database.Migrate()`. Los datos de demostración son deterministas y el proceso limpia el Change Tracker antes de la consulta para que el resultado no dependa de entidades sembradas todavía rastreadas.

El contrato final de Application ya no expone `IQueryable<OrdenFabricacion>`. La flexibilidad de composición del punto 3.11 se conserva dentro del adaptador de Infrastructure.

### Resumen de la teoría

- Concepto central: **Buenas prácticas en el acceso a datos y composición de consultas**.
- Métodos o técnicas trazados en código: `ObtenerResumenesPendientesOptimizado`, `ExisteAlgunaOrdenPendiente`, `ObtenerOrdenesConPlanchasYDetalleSinProductoCartesiano`, `ObtenerPorNumeroOptimizado`.
- La ejecución usa SQL Server LocalDB y filtros globales heredados de M2.
- El punto termina con un marcador E2E `3.12 OK`.

---

