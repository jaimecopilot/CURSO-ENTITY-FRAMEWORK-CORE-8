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

El módulo conserva la profundidad y los objetivos docentes de 4.1–4.12, adaptados al comportamiento real de EF Core 8 y al estado acumulativo de AceriaData. La matriz `TRAZABILIDAD_M04.md` relaciona cada punto con su implementación y evidencia observable.

Entre las correcciones técnicas verificadas destacan:

- los predicados no traducibles en `Where` no se presentan como evaluación cliente silenciosa en EF Core 8;
- `AsNoTrackingWithIdentityResolution` se demuestra con una entidad realmente compartida;
- Split Query se explica en términos de comandos, roundtrips y aislamiento, no como una transacción independiente por subconsulta;
- `EF.CompileQuery` no se confunde con el plan de ejecución de SQL Server;
- la paginación keyset usa orden y cursor compuestos `FechaCreacion + Id`.

## Estado validado

Los doce estados 4.1–4.12 fueron restaurados, compilados y ejecutados sobre SQL Server LocalDB conservando la continuidad desde M3.12. Se verificaron las migraciones heredadas, la frontera arquitectónica, la correspondencia entre teoría/práctica/código y la maquetación completa de los PDF finales.

- [Trazabilidad M4](TRAZABILIDAD_M04.md)
