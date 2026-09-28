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
if len(re.findall(r"(?m)^## Punto 2\.\d+", practice)) != 12:
    raise RuntimeError("PRACTICA M2: no contiene exactamente 12 puntos")

for bad in ("The user wants", "We need to", "Let me think", "Esperando confirmación para continuar", "material fuente"):
    if bad.lower() in theory.lower() or bad.lower() in practice.lower():
        raise RuntimeError(f"M2: metacontenido no permitido: {bad}")


def audit_markdown_structure(label, content):
    lines = content.splitlines()
    in_fence = False
    previous_heading = None
    previous_heading_line = None

    code_outside_patterns = (
        re.compile(r"^using\s+[A-Za-z_]"),
        re.compile(r"^namespace\s+[A-Za-z_]"),
        re.compile(r"^(public|private|protected|internal)\s+(?:sealed\s+|static\s+)?(?:class|interface|record|struct)\b"),
        re.compile(r"^var\s+\w+\s*="),
        re.compile(r"^(context|modelBuilder|migrationBuilder|restaurable)\."),
        re.compile(r"^orden[!.]"),
        re.compile(r"^(ALTER TABLE|ADD CONSTRAINT|FOREIGN KEY|CREATE (?:UNIQUE |CLUSTERED |NONCLUSTERED )?INDEX|DROP INDEX)\b", re.I),
    )

    for line_no, raw in enumerate(lines, 1):
        stripped = raw.strip()

        if stripped.startswith(chr(96) * 3):
            in_fence = not in_fence
            continue

        if re.match(r"^#{1,6}\s+", stripped):
            if (
                previous_heading == stripped
                and previous_heading_line is not None
                and line_no == previous_heading_line + 1
            ):
                raise RuntimeError(
                    f"{label}: título duplicado consecutivo en línea {line_no}: {stripped}"
                )
            previous_heading = stripped
            previous_heading_line = line_no

        if not in_fence and stripped:
            for pattern in code_outside_patterns:
                if pattern.search(stripped):
                    raise RuntimeError(
                        f"{label}: código fuera de bloque en línea {line_no}: {stripped}"
                    )

    if in_fence:
        raise RuntimeError(f"{label}: bloque de código Markdown sin cerrar")


audit_markdown_structure("TEORIA M2", theory)
audit_markdown_structure("PRACTICA M2", practice)


# En teoría, Objetivos de aprendizaje es un único bloque editorial: no contiene subtítulos internos.
for match in re.finditer(
    r"(?ms)^### Objetivos de aprendizaje\s*(.*?)^### Teoría\s*$",
    theory,
):
    objective_block = match.group(1)
    if re.search(r"(?m)^####\s+", objective_block):
        raise RuntimeError(
            "TEORIA M2: hay subtítulos #### dentro de Objetivos de aprendizaje"
        )

for obsolete in (
    "EF Core no expone directamente la opción de crear un índice agrupado o no agrupado.",
    'migrationBuilder.Sql("CREATE CLUSTERED INDEX IX_OrdenesFabricacion_FechaCreacion',
    '.HasColumnType("text")',
    "la entidad dependiente exista de forma independiente",
    "la collation por defecto en SQL Server es SQL_Latin1_General_CP1_CI_AS",
    "IPlanchaRepositorio",
    "IAleacionRepositorio",
    "PlanchaRepositorio",
    "AleacionRepositorio",
):
    if obsolete in theory:
        raise RuntimeError(f"TEORIA M2: contenido obsoleto o no trazable: {obsolete}")

for required in (
    ".IsClustered(false)",
    ".IsClustered()",
    "DatabaseGenerated(DatabaseGeneratedOption.Computed)",
    "no crea por sí solo una fórmula ni un mecanismo automático",
    '.HasColumnType("nvarchar(max)")',
    "un DateTime de última modificación necesita una estrategia explícita",
    "no incorpora todavía las claves alternativas de 2.8",
    "los índices/restricciones específicos de 2.9",
    "los filtros globales de 2.10",
):
    if required not in theory:
        raise RuntimeError(f"TEORIA M2: falta corrección técnica esperada: {required}")

# Secuencia completa de pasos y formato de explicaciones Línea N en la práctica.
point_matches = list(re.finditer(r"(?m)^## Punto (2\.\d+)\b.*$", practice))
if len(point_matches) != 12:
    raise RuntimeError("PRACTICA M2: no se pudieron aislar exactamente los 12 puntos")

practice_sections = {}
for idx, match in enumerate(point_matches):
    end = point_matches[idx + 1].start() if idx + 1 < len(point_matches) else len(practice)
    practice_sections[match.group(1)] = practice[match.start():end]

for n in range(1, 13):
    point = f"2.{n}"
    section = practice_sections.get(point)
    if section is None:
        raise RuntimeError(f"PRACTICA M2: falta sección {point}")

    expected_steps = list(range(1, 9)) if n == 12 else list(range(1, 7))
    actual_steps = [
        int(x)
        for x in re.findall(r"(?m)^### Paso (\d+):", section)
    ]
    if actual_steps != expected_steps:
        raise RuntimeError(
            f"{point}: secuencia de pasos inválida: {actual_steps} != {expected_steps}"
        )

    explanation_lines = [
        line for line in section.splitlines()
        if re.match(r"^Línea(?:s)?\s+\d+:", line)
    ]
    minimum = 15 if n == 12 else 5
    if len(explanation_lines) < minimum:
        raise RuntimeError(
            f"{point}: faltan explicaciones Línea N ({len(explanation_lines)} < {minimum})"
        )

    for line in explanation_lines:
        if " → " not in line or chr(96) not in line:
            raise RuntimeError(
                f"{point}: explicación Línea N sin formato código → explicación: {line}"
            )

explanation_tokens = {
    "2.1": ("BuildServiceProvider", "CreateScope", "GetRequiredService<AceriaDbContext>", "GetEntityTypes", "FindPrimaryKey"),
    "2.2": ("FechaEntrega", "Observaciones", "HasPrecision(18, 3)", "HasDefaultValue(true)", "HasMaxLength"),
    "2.3": ("HasOne", "WithMany", "HasForeignKey", "DeleteBehavior.Cascade", "IsRequired"),
    "2.4": ("HasOne", "WithOne", "HasForeignKey<DetalleOrden>", "IsRequired", "DetalleOrden? Detalle"),
    "2.5": ("HasKey", "OrdenesAleaciones", "DeleteBehavior.Cascade", "DeleteBehavior.Restrict", "CantidadUtilizada"),
    "2.6": ("[Table(", "[Key]", "[Required]", "[MaxLength", "[PrimaryKey("),
    "2.7": ("modelBuilder.Entity", "Property(", "IsRequired", "HasMaxLength", "HasDefaultValueSql"),
    "2.8": ("HasKey", "HasAlternateKey", "HasName", "OrdenFabricacionId", "GetKeys"),
    "2.9": ("HasIndex", "FechaCreacion", "HasFilter", "IncludeProperties", "HasCheckConstraint"),
    "2.10": ("HasQueryFilter", 'Estado != "Cancelada"', "p.Activa", "IgnoreQueryFilters", "Count()"),
    "2.11": ("IsDeleted", "DeletedAt", "!o.IsDeleted", "IgnoreQueryFilters", "restaurable.IsDeleted = false"),
    "2.12": ("IOrdenRepositorio", "IUnidadDeTrabajo", "ApplyConfigurationsFromAssembly", "OrdenRepositorio : IOrdenRepositorio", "UnidadDeTrabajo : IUnidadDeTrabajo", "IDesignTimeDbContextFactory<AceriaDbContext>"),
}
for point, tokens in explanation_tokens.items():
    section = practice_sections[point]
    for token in tokens:
        if token not in section:
            raise RuntimeError(f"{point}: falta concepto explicado/trazado: {token}")

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
    "src/AceriaData.Infrastructure/Persistence/Configurations/OrdenFabricacionConfiguration.cs",
    "src/AceriaData.Infrastructure/Persistence/Configurations/PlanchaAceroConfiguration.cs",
    "src/AceriaData.Infrastructure/Persistence/Configurations/ModeloConfiguration.cs",
    "src/AceriaData.Infrastructure/Repositories/Repositories.cs",
    "src/AceriaData.Infrastructure/DependencyInjection.cs",
    "src/AceriaData.Infrastructure/Persistence/AceriaDesignTimeDbContextFactory.cs",
    "src/AceriaData.Console/Program.cs",
):
    source = (d12 / rel).read_text(encoding="utf-8", errors="ignore").strip()
    if source not in practice:
        raise RuntimeError(f"2.12: {rel} no está trazado literalmente en la práctica")

for token in ("Punto 2.10", "HasQueryFilter", "IgnoreQueryFilters", "Punto 2.11", "Soft Delete", "IsDeleted", "DeletedAt", "Punto 2.12", "Arquitectura Hexagonal"):
    if token not in theory or token not in practice:
        raise RuntimeError(f"M2: falta contenido canónico en teoría/práctica: {token}")

# Coherencia editorial y arquitectónica de los puntos finales.
p211 = theory.index("## Punto 2.11")
p212 = theory.index("## Punto 2.12")
if p211 < 0 or p212 < 0 or p212 <= p211:
    raise RuntimeError("TEORIA M2: no se pudieron aislar los puntos 2.11 y 2.12")

theory_211 = theory[p211:p212]
theory_212 = theory[p212:]

if theory_211.count("### Resumen de la teoría") != 1:
    raise RuntimeError("2.11: debe existir un único Resumen de la teoría")

for heading, expected in (
    ("### Objetivos de aprendizaje", 1),
    ("### Teoría", 1),
    ("### Resumen de la teoría", 1),
):
    actual = theory_212.count(heading)
    if actual != expected:
        raise RuntimeError(
            f"2.12: jerarquía editorial inválida para {heading}: {actual} != {expected}"
        )

for required_heading in (
    "#### Alcance: Repositorio y Unidad de Trabajo",
    "#### Síntesis: Repositorio y Unidad de Trabajo",
    "#### Alcance: Clean Architecture y Arquitectura Hexagonal",
):
    if required_heading not in theory_212:
        raise RuntimeError(f"2.12: falta jerarquía editorial: {required_heading}")

for obsolete in (
    "Colocar las entidades y las interfaces de repositorio en la capa de dominio.",
    "El proyecto de dominio contiene las entidades y las interfaces de repositorio.",
    "El dominio contiene las entidades y las interfaces de repositorio.",
    "La presentación depende de la aplicación.",
):
    if obsolete in theory_212:
        raise RuntimeError(f"2.12: afirmación arquitectónica obsoleta: {obsolete}")

for required in (
    "AceriaData.Domain contiene las entidades del dominio.",
    "AceriaData.Application contiene los casos de uso y los puertos de persistencia",
    "IOrdenRepositorio e IUnidadDeTrabajo",
    "AceriaData.Infrastructure contiene el DbContext, las configuraciones, las migraciones y los adaptadores",
    "AceriaData.Console contiene el punto de entrada y actúa como composition root",
    "Console actúa también como composition root",
):
    if required not in theory_212:
        raise RuntimeError(f"2.12: falta alineación teoría-código: {required}")

print("AUDITORÍA M2 PASS: estructura, documentación, código, migraciones y E2E 2.1->2.12 trazados.")
