# Módulo 1 — Fundamentos de Entity Framework Core

Este módulo conserva el contenido docente suministrado y lo separa en **teoría** y **práctica** sin resumirlo. Se han eliminado únicamente restos conversacionales y se han aplicado correcciones técnicas necesarias para obtener una evolución acumulativa, compilable y verificable de AceriaData.

## Recorrido

1.1 ORM y EF Core → 1.2 arquitectura → 1.3 componentes/migraciones → 1.4 DbContext → 1.5 ciclo de vida → 1.6 DbSet/CRUD → 1.7 Change Tracker → 1.8 gestión de entidades → 1.9 SaveChanges/UoW → 1.10 configuración/logging → 1.11 proveedores (práctica SQL Server) → 1.12 DI/AddDbContext.

## Resultado técnico

Al finalizar M1, AceriaData contiene `OrdenFabricacion`, `PlanchaAcero`, `Aleacion` y `EstadoOrden`, tres migraciones acumulativas, configuración externa, logging, repositorio, servicio de negocio e inyección de dependencias sobre SQL Server LocalDB.

## Documentos

- [Teoría](TEORIA/M01_TEORIA.md)
- [Práctica](PRACTICA/M01_PRACTICA.md)
- [Trazabilidad](TRAZABILIDAD_M01.md)
- [Correcciones técnicas](CORRECCIONES_TECNICAS.md)
