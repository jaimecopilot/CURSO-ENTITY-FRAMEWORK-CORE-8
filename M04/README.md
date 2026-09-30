# Módulo 4 - Optimización y rendimiento

**12 puntos · 6 horas · AceriaData · .NET 8 · Entity Framework Core 8 · SQL Server LocalDB**

M4 continúa directamente desde `M03/PROYECTO/3.12`. Cada checkpoint `4.1 -> 4.12` es un estado completo y ejecutable de AceriaData y conserva la solución, las cuatro capas y la historia real de migraciones.

## Material docente

- [Teoría - Markdown](TEORIA/M04_TEORIA.md)
- [Teoría - PDF](TEORIA/M04_TEORIA.pdf)
- [Práctica - Markdown](PRACTICA/M04_PRACTICA.md)
- [Práctica - PDF](PRACTICA/M04_PRACTICA.pdf)
- [Proyecto acumulativo 4.1 a 4.12](PROYECTO/README.md)
- [Trazabilidad de la fuente docente](TRAZABILIDAD_FUENTE_M04.md)
- [Fuente original conservada](SOURCE/README.md)

## Criterio de construcción

La fuente docente original se utiliza como especificación temática y de profundidad. Cuando una afirmación o ejercicio no coincide con EF Core 8 o con el estado real de AceriaData, se conserva el objetivo docente y se corrige/adapta la implementación. La matriz `TRAZABILIDAD_FUENTE_M04.md` documenta esa relación punto por punto.

Entre las correcciones técnicas verificadas destacan:

- los predicados no traducibles en `Where` no se presentan como evaluación cliente silenciosa en EF Core 8;
- `AsNoTrackingWithIdentityResolution` se demuestra con una entidad realmente compartida;
- Split Query se explica en términos de comandos, roundtrips y aislamiento, no como una transacción independiente por subconsulta;
- `EF.CompileQuery` no se confunde con el plan de ejecución de SQL Server;
- la paginación keyset usa orden y cursor compuestos `FechaCreacion + Id`.

## Validación

La pipeline `Validate M4` exige antes de aceptar el módulo:

- auditoría de continuidad física desde M3.12;
- restore y build de `4.1` a `4.12`;
- cadena de migraciones heredada sobre SQL Server LocalDB;
- E2E de los 12 checkpoints con marcador `4.x OK`;
- límite arquitectónico final sin dependencia de EF Core desde Application;
- auditoría de Markdown contra código validado;
- cobertura de fuente **12/12**;
- generación PDF con el sistema visual heredado de M3;
- QA automática página a página;
- render de todas las páginas para QA visual.

Los Markdown y PDF definitivos se regeneran desde la misma fuente de contenido y la pipeline revisa todas las páginas antes de publicarlos. La trazabilidad de fuente se conserva en un fichero separado y no forma parte del material PDF entregado al alumno.
