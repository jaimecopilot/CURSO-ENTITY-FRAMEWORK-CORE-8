from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
M2 = ROOT / "M02" / "PROYECTO"

def text(n):
    p = M2 / f"2.{n}" / "Program.cs"
    if not p.is_file():
        raise RuntimeError(f"2.{n}: falta Program.cs")
    return p.read_text(encoding="utf-8", errors="ignore")

# Estructura autónoma 2.1-2.11.
for n in range(1, 12):
    d = M2 / f"2.{n}"
    for rel in ("AceriaData.sln", "AceriaData.Console.csproj", "Program.cs", "README.md", "appsettings.json"):
        if not (d / rel).is_file():
            raise RuntimeError(f"2.{n}: falta {rel}")
    sln = (d / "AceriaData.sln").read_text(encoding="utf-8", errors="ignore")
    if '"AceriaData.Console.csproj"' not in sln:
        raise RuntimeError(f"2.{n}: la solución no referencia el proyecto local")
    if "../" in sln or "..\\" in sln or "M01\\" in sln:
        raise RuntimeError(f"2.{n}: la solución referencia fuera de su carpeta")

# 2.12 es multiproyecto y debe mantener EF Core sólo en Infrastructure.
d12 = M2 / "2.12"
for rel in (
    "AceriaData.sln",
    "src/AceriaData.Domain/AceriaData.Domain.csproj",
    "src/AceriaData.Application/AceriaData.Application.csproj",
    "src/AceriaData.Infrastructure/AceriaData.Infrastructure.csproj",
    "src/AceriaData.Console/AceriaData.Console.csproj",
):
    if not (d12 / rel).is_file():
        raise RuntimeError(f"2.12: falta {rel}")

domain_csproj = (d12 / "src/AceriaData.Domain/AceriaData.Domain.csproj").read_text(encoding="utf-8")
app_csproj = (d12 / "src/AceriaData.Application/AceriaData.Application.csproj").read_text(encoding="utf-8")
infra_csproj = (d12 / "src/AceriaData.Infrastructure/AceriaData.Infrastructure.csproj").read_text(encoding="utf-8")
if "EntityFrameworkCore" in domain_csproj or "EntityFrameworkCore" in app_csproj:
    raise RuntimeError("2.12: Domain/Application no deben depender de EF Core")
if "Microsoft.EntityFrameworkCore.SqlServer" not in infra_csproj:
    raise RuntimeError("2.12: Infrastructure debe contener el proveedor de EF Core")

# Cronología pedagógica.
for n in (1,2,3):
    if "class DetalleOrden" in text(n):
        raise RuntimeError(f"2.{n}: DetalleOrden se adelanta antes de 2.4")
for n in (1,2,3,4):
    if "class OrdenAleacion" in text(n):
        raise RuntimeError(f"2.{n}: OrdenAleacion se adelanta antes de 2.5")
for n in range(1,6):
    if '[Table("OrdenesFabricacion")]' in text(n):
        raise RuntimeError(f"2.{n}: Data Annotations se adelantan antes de 2.6")
for n in range(1,8):
    if "HasAlternateKey" in text(n):
        raise RuntimeError(f"2.{n}: claves alternativas se adelantan antes de 2.8")
for n in range(1,9):
    if "HasIndex(" in text(n) or "HasCheckConstraint" in text(n) or "[Index(" in text(n):
        raise RuntimeError(f"2.{n}: índices/restricciones de 2.9 se adelantan")
for n in range(1,10):
    if "HasQueryFilter" in text(n):
        raise RuntimeError(f"2.{n}: filtros globales se adelantan antes de 2.10")
for n in range(1,11):
    if "IsDeleted" in text(n):
        raise RuntimeError(f"2.{n}: Soft Delete se adelanta antes de 2.11")

# Presencia de conceptos en su punto.
required = {
    2: ("FechaEntrega", "Observaciones", "Peso", "Codigo"),
    3: (".WithMany(o => o.Planchas)",),
    4: ("class DetalleOrden", ".WithOne(o => o.Detalle)", "class CertificadoCalidad"),
    5: ("class OrdenAleacion", "OrdenesAleaciones", "HasKey(x => new { x.OrdenFabricacionId, x.AleacionId })"),
    6: ('[Table("OrdenesFabricacion")]', "[PrimaryKey(nameof(OrdenFabricacionId), nameof(AleacionId))]"),
    7: ("HasPrecision(18, 3)", "HasDefaultValueSql"),
    8: ("HasAlternateKey",),
    9: ("HasCheckConstraint", "IX_OrdenesFabricacion_Cliente_FechaCreacion"),
    10: ("HasQueryFilter", "IgnoreQueryFilters"),
    11: ("IsDeleted", "DeletedAt", "IgnoreQueryFilters"),
}
for n, tokens in required.items():
    c = text(n)
    for token in tokens:
        if token not in c:
            raise RuntimeError(f"2.{n}: falta evidencia de {token}")

# Migraciones acumulativas.
for n in range(1,12):
    mdir = M2 / f"2.{n}" / "Migrations"
    if not mdir.is_dir():
        raise RuntimeError(f"2.{n}: falta carpeta Migrations")
    if not (mdir / "AceriaDbContextModelSnapshot.cs").is_file():
        raise RuntimeError(f"2.{n}: falta snapshot")
    expected = 3 + max(0, n - 1)
    migrations = [p for p in mdir.glob("*.cs") if not p.name.endswith(".Designer.cs") and p.name != "AceriaDbContextModelSnapshot.cs"]
    if len(migrations) < expected:
        raise RuntimeError(f"2.{n}: historial de migraciones incompleto ({len(migrations)} < {expected})")

m12 = d12 / "src/AceriaData.Infrastructure/Migrations"
if not m12.is_dir() or not (m12 / "AceriaDbContextModelSnapshot.cs").is_file():
    raise RuntimeError("2.12: faltan migraciones/snapshot en Infrastructure")
if not any("M2_2_12_Architecture" in p.name for p in m12.glob("*.cs")):
    raise RuntimeError("2.12: falta migración de arquitectura")

# Prohibir EnsureCreated: el flujo de M2 usa Migrations.
for n in range(1,12):
    if "EnsureCreated" in text(n):
        raise RuntimeError(f"2.{n}: no debe mezclar EnsureCreated con Migrations")
for p in d12.rglob("*.cs"):
    if "EnsureCreated" in p.read_text(encoding="utf-8", errors="ignore"):
        raise RuntimeError(f"2.12: EnsureCreated no permitido: {p}")


# Documentación y trazabilidad práctica -> código.
theory_path = ROOT / "M02" / "TEORIA" / "M02_TEORIA.md"
practice_path = ROOT / "M02" / "PRACTICA" / "M02_PRACTICA.md"
if not theory_path.is_file() or not practice_path.is_file():
    raise RuntimeError("M2: faltan documentos Markdown de teoría o práctica")

theory = theory_path.read_text(encoding="utf-8", errors="ignore")
practice = practice_path.read_text(encoding="utf-8", errors="ignore")

if len(re.findall(r"(?m)^## Punto 2\.\d+", theory)) != 12:
    raise RuntimeError("TEORIA M2: no contiene exactamente 12 puntos")
if len(re.findall(r"(?m)^## Punto 2\\.\\d+", practice)) != 12:
    raise RuntimeError("PRACTICA M2: no contiene exactamente 12 puntos")

for bad in ("The user wants", "We need to", "Let me think", "Esperando confirmación para continuar", "material fuente"):
    if bad.lower() in theory.lower() or bad.lower() in practice.lower():
        raise RuntimeError(f"M2: metacontenido no permitido: {bad}")

# La práctica incluye el Program.cs real de cada estado 2.1-2.11.
for n in range(1, 12):
    source = (M2 / f"2.{n}" / "Program.cs").read_text(encoding="utf-8", errors="ignore").strip()
    if source not in practice:
        raise RuntimeError(f"2.{n}: Program.cs no está trazado literalmente en la práctica")

# En 2.12 se trazan los archivos esenciales de cada capa.
for rel in (
    "src/AceriaData.Domain/Entities.cs",
    "src/AceriaData.Application/Interfaces.cs",
    "src/AceriaData.Application/CrearOrdenUseCase.cs",
    "src/AceriaData.Infrastructure/Persistence/AceriaDbContext.cs",
    "src/AceriaData.Infrastructure/DependencyInjection.cs",
    "src/AceriaData.Console/Program.cs",
):
    source = (d12 / rel).read_text(encoding="utf-8", errors="ignore").strip()
    if source not in practice:
        raise RuntimeError(f"2.12: {rel} no está trazado literalmente en la práctica")

for token in ("Punto 2.10", "HasQueryFilter", "IgnoreQueryFilters", "Punto 2.11", "Soft Delete", "IsDeleted", "DeletedAt", "Punto 2.12", "Arquitectura Hexagonal"):
    if token not in theory or token not in practice:
        raise RuntimeError(f"M2: falta contenido canónico en teoría/práctica: {token}")

print("AUDITORÍA M2 PASS: estructura, documentación, código, migraciones y E2E 2.1->2.12 trazados.")
