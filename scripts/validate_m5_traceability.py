from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
TRACE = (ROOT / "M05" / "TRAZABILIDAD_M05.md").read_text(encoding="utf-8")
THEORY = (ROOT / "M05" / "TEORIA" / "M05_TEORIA.md").read_text(encoding="utf-8")
PRACTICE = (ROOT / "M05" / "PRACTICA" / "M05_PRACTICA.md").read_text(encoding="utf-8")
WORKFLOW = (ROOT / ".github" / "workflows" / "validate-m5.yml").read_text(encoding="utf-8")

REQUIREMENTS = {
    "5.1": ("M05/PROYECTO/5.1/src/AceriaData.Infrastructure/Repositories/ConcurrenciaOptimistaM5Repositorio.cs", "Run 5.1 against SQL Server LocalDB"),
    "5.2": ("M05/PROYECTO/5.2/src/AceriaData.Infrastructure/Repositories/TokensConcurrenciaM5Repositorio.cs", "Run 5.2 against SQL Server LocalDB"),
    "5.3": ("M05/PROYECTO/5.3/src/AceriaData.Infrastructure/Repositories/ResolucionConflictosM5Repositorio.cs", "Run 5.3 against SQL Server LocalDB"),
    "5.4": ("M05/PROYECTO/5.4/src/AceriaData.Infrastructure/Repositories/TransaccionesM5Repositorio.cs", "Run 5.4 against SQL Server LocalDB"),
    "5.5": ("M05/PROYECTO/5.5/src/AceriaData.Infrastructure/Repositories/TransaccionesAmbientalesM5Repositorio.cs", "Run 5.5 against SQL Server LocalDB"),
    "5.6": ("M05/PROYECTO/5.6/src/AceriaData.Infrastructure/Repositories/MigracionesProduccionM5Repositorio.cs", "Run 5.6 with IMigrator against SQL Server LocalDB"),
    "5.7": ("M05/PROYECTO/5.7/deployment/validate-idempotent-scripts.ps1", "Validate 5.7 model and idempotent SQL scripts"),
    "5.8": ("M05/PROYECTO/5.8/team-migrations/validate-team-migrations.ps1", "Validate 5.8 team migration workflow"),
    "5.9": ("M05/PROYECTO/5.9/src/AceriaData.Infrastructure/Repositories/RepositoryPattern.cs", "Validate 5.9 Repository and Unit of Work"),
    "5.10": ("M05/PROYECTO/5.10/src/AceriaData.Console/Diagnostics/EfDiagnosticObserver.cs", "Validate 5.10 logging and diagnostics"),
    "5.11": ("M05/PROYECTO/5.11/tests/AceriaData.Tests/Integration/SqlServerIntegrationTests.cs", "Validate 5.11 EF Core testing stack"),
    "5.12": ("M05/PROYECTO/5.12/src/AceriaData.Infrastructure/BuenasPracticasAntiPatronesM5Diagnostico.cs", "Validate 5.12 good practices before-after"),
}

for point, (path, gate) in REQUIREMENTS.items():
    if f"## {point} " not in TRACE and f"## {point} —" not in TRACE:
        raise SystemExit(f"Falta {point} en TRAZABILIDAD_M05.md")
    if f"## Punto {point} " not in THEORY and f"## Punto {point} —" not in THEORY:
        raise SystemExit(f"Falta Punto {point} en teoría")
    if f"## Punto {point} " not in PRACTICE and f"## Punto {point} —" not in PRACTICE:
        raise SystemExit(f"Falta Punto {point} en práctica")
    target = ROOT / path
    if not target.exists():
        raise SystemExit(f"No existe el archivo trazado de {point}: {path}")
    if path not in TRACE:
        raise SystemExit(f"La matriz no cita el archivo principal de {point}: {path}")
    if gate not in TRACE or gate not in WORKFLOW:
        raise SystemExit(f"Gate de CI no trazado para {point}: {gate}")
    program = ROOT / "M05" / "PROYECTO" / point / "src" / "AceriaData.Console" / "Program.cs"
    if not program.exists() or f"{point} OK" not in program.read_text(encoding="utf-8"):
        raise SystemExit(f"El estado {point} no conserva su marcador final")

required_tests = [
    "M05/PROYECTO/5.12/tests/AceriaData.Tests/RepositoryPatternTests.cs",
    "M05/PROYECTO/5.12/tests/AceriaData.Tests/ProviderBehavior/ProviderBehaviorTests.cs",
    "M05/PROYECTO/5.12/tests/AceriaData.Tests/Integration/SqlServerIntegrationTests.cs",
    "M05/PROYECTO/5.12/tests/AceriaData.Tests/Integration/ApiIntegrationTests.cs",
    "M05/PROYECTO/5.12/tests/AceriaData.Tests/Integration/BuenasPracticasAntiPatronesM5Tests.cs",
]
for path in required_tests:
    if not (ROOT / path).exists():
        raise SystemExit(f"Falta test final trazado: {path}")

for internal in ("AUDITORIA_TECNICA_FUENTE_M05.md", "QUALITY_CONTRACT.md"):
    if (ROOT / "M05" / internal).exists():
        raise SystemExit(f"El documento interno {internal} no debe formar parte del módulo final")

print("M5 TRACEABILITY PASS: 12/12 puntos y requisitos transversales verificados.")
