# Correcciones técnicas aplicadas a M1

El objetivo de esta auditoría no es resumir el material, sino conservarlo y corregir únicamente aquello que impedía su coherencia o ejecución.

## Correcciones estructurales

- Se eliminaron restos de conversación (`ok`, instrucciones internas, mensajes de continuidad) que no forman parte del curso.
- Se separó cada uno de los puntos 1.1–1.12 en teoría y práctica manteniendo ejemplos, pasos, errores comunes, retos y soluciones.
- Se alineó la evolución con la guía AceriaData: 1.2 introduce sólo el primer contexto/Orden; 1.3 consolida PlanchaAcero y Aleacion; 1.4 incorpora EstadoOrden; 1.5 se centra en ciclo de vida/fábrica.
- Desde 1.3 se evita mezclar `EnsureCreated` con migraciones; `EnsureCreated` queda como introducción temporal de 1.2 y después se elimina/recrea la base mediante migraciones.
- El namespace de código se cambió de `AceriaData.Console` a `AceriaData.ConsoleApp` para evitar la colisión con `System.Console` y el error CS0234.

## Correcciones de EF Core

- Los paquetes de EF Core se fijan en la rama 8.x (`8.0.31`) para impedir que un comando sin versión instale otra versión mayor.
- `SaveChanges` se describe por su garantía de atomicidad y por `AutoTransactionBehavior.WhenNeeded`; no se afirma que cada llamada emita necesariamente una transacción explícita.
- El valor devuelto por `SaveChanges` se trata como número de entradas de estado escritas, no como contador universal de filas SQL.
- Se corrigió la explicación de resolución de identidad: una consulta LINQ puede volver a ejecutar SQL aunque materialice la misma instancia rastreada; `Find` sí consulta primero las entidades rastreadas.
- Se corrigió `AcceptAllChanges`: `SaveChanges()` lo invoca por defecto; la llamada manual tiene sentido en escenarios como `SaveChanges(false)`.
- La concurrencia optimista no se presenta como detección automática de cualquier edición concurrente antes de configurar tokens/condiciones de concurrencia.

## Regla de proveedor

AceriaData usa SQL Server LocalDB en las prácticas de M1. El antiguo ejercicio de 1.11 que alternaba SQL Server/SQLite se sustituyó por una práctica de auditoría profunda del proveedor SQL Server, conservando la profundidad docente. InMemory y SQLite in-memory quedan reservados para el punto 5.10, tal como exige el temario actualizado.

## Corrección de tooling

Al pasar a `AddDbContext` en 1.12 se añade `IDesignTimeDbContextFactory<AceriaDbContext>` para que `dotnet ef` pueda construir el contexto de diseño sin ejecutar la lógica de negocio del programa.
