from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]

MODULES = {
    1: "TRAZABILIDAD_M01.md",
    2: "TRAZABILIDAD_M02.md",
    3: "TRAZABILIDAD_M03.md",
    4: "TRAZABILIDAD_FUENTE_M04.md",
    5: "TRAZABILIDAD_M05.md",
}

for n, trace_name in MODULES.items():
    mod = ROOT / f"M0{n}"
    required = [
        mod / "README.md",
        mod / "TEORIA" / f"M0{n}_TEORIA.md",
        mod / "TEORIA" / f"M0{n}_TEORIA.pdf",
        mod / "PRACTICA" / f"M0{n}_PRACTICA.md",
        mod / "PRACTICA" / f"M0{n}_PRACTICA.pdf",
        mod / "PROYECTO" / "README.md",
        mod / trace_name,
    ]
    for path in required:
        if not path.exists():
            raise SystemExit(f"Falta entregable M{n}: {path.relative_to(ROOT)}")

    theory = (mod / "TEORIA" / f"M0{n}_TEORIA.md").read_text(encoding="utf-8")
    practice = (mod / "PRACTICA" / f"M0{n}_PRACTICA.md").read_text(encoding="utf-8")
    tpoints = len(re.findall(rf"(?m)^## Punto {n}\.\d+\b", theory))
    ppoints = len(re.findall(rf"(?m)^## Punto {n}\.\d+\b", practice))
    if tpoints != 12:
        raise SystemExit(f"M{n}: teoría contiene {tpoints} puntos; se esperaban 12")
    if ppoints != 12:
        raise SystemExit(f"M{n}: práctica contiene {ppoints} puntos; se esperaban 12")

    for p in range(1, 13):
        state = mod / "PROYECTO" / f"{n}.{p}"
        if not state.is_dir():
            raise SystemExit(f"M{n}: falta estado acumulativo {n}.{p}")
        if not (state / "AceriaData.sln").exists():
            raise SystemExit(f"M{n}: falta AceriaData.sln en {n}.{p}")

student_forbidden = (
    "the user wants",
    "we need to",
    "let me think",
    "esperando confirmación para continuar",
    "material fuente",
    "corrección técnica:",
    "qa interno",
    "source routes",
    "checksum",
)
for n in MODULES:
    for kind in ("TEORIA", "PRACTICA"):
        path = ROOT / f"M0{n}" / kind / f"M0{n}_{kind}.md"
        low = path.read_text(encoding="utf-8").lower()
        for token in student_forbidden:
            if token in low:
                raise SystemExit(f"Metacontenido interno en {path.relative_to(ROOT)}: {token}")

for path in ROOT.rglob("*"):
    rel = path.relative_to(ROOT).as_posix()
    parts = path.relative_to(ROOT).parts
    if "bin" in parts or "obj" in parts:
        raise SystemExit(f"Residuo de compilación versionado: {rel}")
    if "QA_RENDER" in parts:
        raise SystemExit(f"Render QA versionado: {rel}")

for rel in (
    "M05/QUALITY_CONTRACT.md",
    "M02/.MIGRATIONS_READY",
    ".github/workflows/bootstrap-m2.yml",
    "M05/AUDITORIA_TECNICA_FUENTE_M05.md",
):
    if (ROOT / rel).exists():
        raise SystemExit(f"Residuo interno presente: {rel}")

if (ROOT / "M06").exists():
    raise SystemExit("Existe M06 aunque el temario definitivo finaliza en M5")

root_readme = (ROOT / "README.md").read_text(encoding="utf-8")
for n in MODULES:
    if f"## Módulo {n}" not in root_readme:
        raise SystemExit(f"README raíz no indexa M{n}")

print("COURSE STATIC AUDIT PASS: M1-M5, 60 puntos, entregables y repositorio limpio.")
