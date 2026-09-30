from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
M4 = ROOT / "M04" / "PROYECTO"
BASE = ROOT / "M03" / "PROYECTO" / "3.12"

def source_files(root: Path) -> set[str]:
    return {
        p.relative_to(root).as_posix()
        for p in root.rglob("*")
        if p.is_file() and "bin" not in p.parts and "obj" not in p.parts
    }

baseline = source_files(BASE)
previous = baseline

for n in range(1, 13):
    point = f"4.{n}"
    d = M4 / point

    if not d.is_dir():
        raise RuntimeError(f"{point}: falta checkpoint")

    current = source_files(d)
    missing_baseline = sorted(baseline - current)
    if missing_baseline:
        raise RuntimeError(
            f"{point}: regresion frente a M03/3.12; faltan archivos heredados: {missing_baseline}"
        )

    lost = sorted(previous - current)
    if lost:
        raise RuntimeError(
            f"4.{n-1}->4.{n}: el estado acumulativo perdio archivos: {lost}"
        )
    previous = current

    for rel in (
        "AceriaData.sln",
        "src/AceriaData.Domain/AceriaData.Domain.csproj",
        "src/AceriaData.Application/AceriaData.Application.csproj",
        "src/AceriaData.Infrastructure/AceriaData.Infrastructure.csproj",
        "src/AceriaData.Console/AceriaData.Console.csproj",
    ):
        if not (d / rel).is_file():
            raise RuntimeError(f"{point}: falta {rel}")

    if any(p.name in {"bin", "obj"} for p in d.rglob("*") if p.is_dir()):
        raise RuntimeError(f"{point}: bin/obj no deben versionarse")

    program = (d / "src/AceriaData.Console/Program.cs").read_text(encoding="utf-8")
    if "EnsureCreated" in program:
        raise RuntimeError(f"{point}: EnsureCreated no permitido")
    for token in (
        "Database.Migrate()",
        "ChangeTracker.Clear()",
        f'Console.WriteLine("{point} OK")',
    ):
        if token not in program:
            raise RuntimeError(f"{point}: falta {token}")

    app = d / "src/AceriaData.Application"
    for cs in app.glob("*.cs"):
        if "Microsoft.EntityFrameworkCore" in cs.read_text(encoding="utf-8"):
            raise RuntimeError(f"{point}: Application depende de EF Core en {cs.name}")

    interfaces = (app / "Interfaces.cs").read_text(encoding="utf-8")
    if "IQueryable<OrdenFabricacion>" in interfaces:
        raise RuntimeError(f"{point}: Application vuelve a exponer IQueryable")

    migrations = d / "src/AceriaData.Infrastructure/Migrations"
    if not (migrations / "AceriaDbContextModelSnapshot.cs").is_file():
        raise RuntimeError(f"{point}: falta snapshot heredado")
    if not any("M2_2_12_Architecture" in p.name for p in migrations.glob("*.cs")):
        raise RuntimeError(f"{point}: no conserva la migracion final heredada")

point_checks = {
    1: (
        "AnalisisSqlUseCase",
        "ObtenerSqlPendientesOrdenadasM4",
        "ToQueryString",
        "LogTo",
    ),
    2: (
        "TrackingUseCase",
        "AsTracking",
        "AsNoTracking",
        "EntidadesRastreadas",
    ),
    3: (
        "IdentityResolutionUseCase",
        "AsNoTrackingWithIdentityResolution",
        "Select(oa => oa.Aleacion)",
        "ReferenceEqualityComparer.Instance",
    ),
    4: (
        "NMasUnoUseCase",
        "SqlCommandCounterInterceptor",
        "metrica.Ordenes + 1",
    ),
    5: (
        "SolucionesNMasUnoUseCase",
        "EjecutarIncludeContraNMasUnoM4",
        "EjecutarProyeccionContraNMasUnoM4",
        "EjecutarSplitQueryContraNMasUnoM4",
    ),
    6: (
        "OverFetchingUseCase",
        "ObtenerSqlPendientesEntidadCompletaM4",
        "ObtenerSqlPendientesProyectadasM4",
        "Observaciones",
    ),
    7: (
        "TraduccionConsultasUseCase",
        "catch (InvalidOperationException)",
        ".AsEnumerable()",
        "LOWER",
    ),
    8: (
        "SplitQueriesUseCase",
        ".AsSingleQuery()",
        ".AsSplitQuery()",
        "split.ConsultasSql != 3",
    ),
    9: (
        "CompiledQueriesUseCase",
        "EF.CompileQuery",
        "Medicion observacional",
    ),
    10: (
        "PaginacionUseCase",
        ".Skip(",
        "ultimaFecha",
        "ThenBy(o => o.Id)",
    ),
    11: (
        "DiagnosticoRendimientoUseCase",
        'TagWith("M4.11-DIAGNOSTICO")',
        "ConsultasSql",
        "EntidadesRastreadas",
    ),
    12: (
        "ChecklistRendimientoUseCase",
        'TagWith("M4.12-CHECKLIST-FINAL")',
        "DecisionCompiledQuery",
        "DecisionLoading",
    ),
}

for n, tokens in point_checks.items():
    d = M4 / f"4.{n}"
    haystack = "\n".join(
        p.read_text(encoding="utf-8")
        for p in d.rglob("*.cs")
    )
    for token in tokens:
        if token not in haystack:
            raise RuntimeError(f"4.{n}: falta evidencia contractual {token}")

# Salvaguardas semanticas concretas frente a errores detectados en la fuente.
r43 = (M4 / "4.3" / "src/AceriaData.Infrastructure/Repositories/Rendimiento43.cs").read_text(encoding="utf-8")
if "Select(oa => oa.Aleacion)" not in r43:
    raise RuntimeError("4.3: identity resolution no usa una entidad compartida real")

r47 = (M4 / "4.7" / "src/AceriaData.Infrastructure/Repositories/Rendimiento47.cs").read_text(encoding="utf-8")
if "catch (InvalidOperationException)" not in r47 or ".AsEnumerable()" not in r47:
    raise RuntimeError(
        "4.7: falta separar fallo de traduccion de evaluacion cliente explicita"
    )

r49 = (M4 / "4.9" / "src/AceriaData.Application/CompiledQueriesUseCase.cs").read_text(encoding="utf-8")
if "throw" in "\n".join(
    line for line in r49.splitlines() if "Elapsed" in line or "ticks" in line.lower()
):
    raise RuntimeError("4.9: no se debe convertir la micro-medicion en umbral de rendimiento")

print("M4 CODE TRACEABILITY OK")
