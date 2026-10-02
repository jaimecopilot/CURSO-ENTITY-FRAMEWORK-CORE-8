# Módulo 5 — Persistencia empresarial

Este módulo continúa el proyecto acumulativo **AceriaData** desde el estado final del Módulo 4 y desarrolla concurrencia optimista, transacciones, migraciones para producción, trabajo en equipo, patrones de acceso a datos, observabilidad, testing y buenas prácticas con **.NET 8**, **Entity Framework Core 8** y **SQL Server LocalDB**.

## Material del módulo

- [Teoría — Markdown](TEORIA/M05_TEORIA.md)
- [Teoría — PDF](TEORIA/M05_TEORIA.pdf)
- [Prácticas — Markdown](PRACTICA/M05_PRACTICA.md)
- [Prácticas — PDF](PRACTICA/M05_PRACTICA.pdf)
- [Proyecto acumulativo 5.1 a 5.12](PROYECTO/README.md)

## Secuencia de contenidos

1. **5.1** — Concurrencia optimista: concepto y necesidad.
2. **5.2** — Configuración de tokens de concurrencia.
3. **5.3** — Resolución de conflictos de concurrencia.
4. **5.4** — Transacciones: `SaveChanges` y transacciones explícitas.
5. **5.5** — Transacciones ambientales y buenas prácticas.
6. **5.6** — Migraciones en entornos de producción.
7. **5.7** — Migraciones idempotentes y scripts SQL.
8. **5.8** — Migraciones en equipos: conflictos y buenas prácticas.
9. **5.9** — Repository y Unit of Work en aplicaciones empresariales.
10. **5.10** — Logging y diagnóstico en Entity Framework Core.
11. **5.11** — Testing con EF Core.
12. **5.12** — Buenas prácticas y anti-patrones en persistencia empresarial.

## Estado final del proyecto

Abrir en Visual Studio:

```text
M05/PROYECTO/5.12/AceriaData.sln
```

Desde PowerShell:

```powershell
cd M05/PROYECTO/5.12
dotnet restore AceriaData.sln
dotnet build AceriaData.sln --configuration Release
dotnet ef migrations has-pending-model-changes `
  --project src/AceriaData.Infrastructure `
  --startup-project src/AceriaData.Console `
  --configuration Release
dotnet run --project src/AceriaData.Console/AceriaData.Console.csproj `
  --configuration Release --no-build
dotnet test tests/AceriaData.Tests/AceriaData.Tests.csproj `
  --configuration Release --no-build
```

La última migración del modelo es **`M5_5_2_ConcurrencyTokens`**. Los puntos 5.3 a 5.12 conservan ese modelo y añaden comportamiento, despliegue, arquitectura, observabilidad y pruebas sin introducir migraciones vacías.

## Criterio de trabajo

Cada carpeta `5.n` contiene un estado completo y ejecutable del proyecto y continúa desde el punto anterior. Las prácticas del manual utilizan esos estados reales y relacionan el código con el comportamiento observado en SQL Server LocalDB.

## Regenerar los manuales PDF

Desde la raíz del repositorio:

```powershell
python scripts/generate_m5_pdfs.py
python scripts/audit_m5_pdf_pages.py
python scripts/render_m5_pdf_pages.py
```

La generación produce los PDF de teoría y prácticas a partir de sus Markdown. Los dos scripts posteriores comprueban la composición de todas las páginas y generan una imagen de cada página para su revisión visual.
