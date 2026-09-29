from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
M3 = ROOT / "M03" / "PROYECTO"
THEORY = ROOT / "M03" / "TEORIA" / "M03_TEORIA.md"
PRACTICE = ROOT / "M03" / "PRACTICA" / "M03_PRACTICA.md"

USE = {
    1: "ConsultasLinqUseCase.cs", 2: "ConsultasBasicasUseCase.cs",
    3: "ProyeccionesUseCase.cs", 4: "ProyeccionesDtoUseCase.cs",
    5: "AgregacionesUseCase.cs", 6: "AgrupacionesUseCase.cs",
    7: "JoinsUseCase.cs", 8: "CargaEagerUseCase.cs",
    9: "CargaLazyUseCase.cs", 10: "CargaExplicitaUseCase.cs",
    11: "ComposicionConsultasUseCase.cs", 12: "BuenasPracticasUseCase.cs",
}
required_projects = (
    "src/AceriaData.Domain/AceriaData.Domain.csproj",
    "src/AceriaData.Application/AceriaData.Application.csproj",
    "src/AceriaData.Infrastructure/AceriaData.Infrastructure.csproj",
    "src/AceriaData.Console/AceriaData.Console.csproj",
)
def source_file_set(root: Path) -> set[str]:
    """Archivos versionables del checkpoint, excluyendo artefactos de compilación."""
    return {
        p.relative_to(root).as_posix()
        for p in root.rglob("*")
        if p.is_file() and "bin" not in p.parts and "obj" not in p.parts
    }

baseline_m2 = ROOT / "M02" / "PROYECTO" / "2.12"
baseline_files = source_file_set(baseline_m2)
previous_files = None
for n in range(1, 13):
    checkpoint = M3 / f"3.{n}"
    checkpoint_files = source_file_set(checkpoint)
    missing_baseline = sorted(baseline_files - checkpoint_files)
    if missing_baseline:
        raise RuntimeError(
            f"3.{n}: regresión frente a M2/2.12; faltan archivos heredados: {missing_baseline}"
        )
    if previous_files is not None:
        lost = sorted(previous_files - checkpoint_files)
        if lost:
            raise RuntimeError(
                f"3.{n-1}->3.{n}: el estado acumulativo perdió archivos: {lost}"
            )
    previous_files = checkpoint_files

for n in range(1, 13):
    d = M3 / f"3.{n}"
    if not (d / "AceriaData.sln").is_file():
        raise RuntimeError(f"3.{n}: falta AceriaData.sln")
    for rel in required_projects:
        if not (d / rel).is_file():
            raise RuntimeError(f"3.{n}: falta {rel}")
    if any(p.name in {"bin","obj"} for p in d.rglob("*") if p.is_dir()):
        raise RuntimeError(f"3.{n}: bin/obj no deben versionarse")
    program = (d / "src/AceriaData.Console/Program.cs").read_text(encoding="utf-8")
    if "EnsureCreated" in program:
        raise RuntimeError(f"3.{n}: EnsureCreated no permitido")
    for token in ("Database.Migrate()", f'Console.WriteLine("3.{n} OK")', "ChangeTracker.Clear()"):
        if token not in program:
            raise RuntimeError(f"3.{n}: falta {token}")
    mdir = d / "src/AceriaData.Infrastructure/Migrations"
    if not (mdir / "AceriaDbContextModelSnapshot.cs").is_file():
        raise RuntimeError(f"3.{n}: falta snapshot")
    if not any("M2_2_12_Architecture" in p.name for p in mdir.glob("*.cs")):
        raise RuntimeError(f"3.{n}: no conserva la migración final de M2")
    app = d / "src/AceriaData.Application"
    for cs in app.glob("*.cs"):
        if "Microsoft.EntityFrameworkCore" in cs.read_text(encoding="utf-8"):
            raise RuntimeError(f"3.{n}: Application depende de EF Core en {cs.name}")

# AutoInclude curricular: se demuestra desde 3.8, se mantiene hasta 3.11 y se retirará en 3.12.
for n in range(8, 12):
    cfg = (M3 / f"3.{n}" / "src/AceriaData.Infrastructure/Persistence/Configurations/OrdenFabricacionConfiguration.cs").read_text(encoding="utf-8")
    repo = (M3 / f"3.{n}" / "src/AceriaData.Infrastructure/Repositories/Repositories.cs").read_text(encoding="utf-8")
    interfaces = (M3 / f"3.{n}" / "src/AceriaData.Application/Interfaces.cs").read_text(encoding="utf-8")
    if "Navigation(x => x.Planchas).AutoInclude()" not in cfg:
        raise RuntimeError(f"3.{n}: falta AutoInclude curricular sobre Planchas")
    for token in ("ObtenerOrdenesAutoInclude", "ObtenerOrdenesIgnorandoAutoInclude"):
        if token not in repo or token not in interfaces:
            raise RuntimeError(f"3.{n}: falta demostración acumulativa de {token}")

repo38 = (M3 / "3.8" / "src/AceriaData.Infrastructure/Repositories/Repositories.cs").read_text(encoding="utf-8")
use38 = (M3 / "3.8" / "src/AceriaData.Application/CargaEagerUseCase.cs").read_text(encoding="utf-8")
for token in ("ObtenerOrdenesAutoInclude", "ObtenerOrdenesIgnorandoAutoInclude", "OrdenesAleaciones"):
    if token not in use38:
        raise RuntimeError(f"3.8: el E2E no valida {token}")
for token in ("IgnoreAutoIncludes", "OrdenesAleaciones", "ThenInclude", "AsSplitQuery"):
    if token not in repo38:
        raise RuntimeError(f"3.8: falta cobertura real de {token}")

d39 = M3 / "3.9"
if "Microsoft.EntityFrameworkCore.Proxies" not in (d39 / "src/AceriaData.Infrastructure/AceriaData.Infrastructure.csproj").read_text(encoding="utf-8"):
    raise RuntimeError("3.9: falta paquete Proxies")
if "UseLazyLoadingProxies" not in (d39 / "src/AceriaData.Infrastructure/DependencyInjection.cs").read_text(encoding="utf-8"):
    raise RuntimeError("3.9: Lazy Loading no está habilitado")
if "virtual List<PlanchaAcero> Planchas" not in (d39 / "src/AceriaData.Domain/Entities.cs").read_text(encoding="utf-8"):
    raise RuntimeError("3.9: navegación Planchas no es virtual")
if "IgnoreAutoIncludes" not in (d39 / "src/AceriaData.Infrastructure/Repositories/Repositories.cs").read_text(encoding="utf-8"):
    raise RuntimeError("3.9: la demostración Lazy no neutraliza AutoInclude")

for n in range(10, 13):
    di = (M3 / f"3.{n}" / "src/AceriaData.Infrastructure/DependencyInjection.cs").read_text(encoding="utf-8")
    if "UseLazyLoadingProxies" in di:
        raise RuntimeError(f"3.{n}: Lazy Loading debía quedar desactivado")

cfg312 = (M3 / "3.12" / "src/AceriaData.Infrastructure/Persistence/Configurations/OrdenFabricacionConfiguration.cs").read_text(encoding="utf-8")
repo312 = (M3 / "3.12" / "src/AceriaData.Infrastructure/Repositories/Repositories.cs").read_text(encoding="utf-8")
if "AutoInclude()" in cfg312:
    raise RuntimeError("3.12: AutoInclude debía retirarse del modelo final")
for token in ("Include(o => o.Planchas)", "Include(o => o.OrdenesAleaciones)", "ThenInclude(oa => oa.Aleacion)", "AsSplitQuery"):
    if token not in repo312:
        raise RuntimeError(f"3.12: SplitQuery final no cubre {token}")

i312 = (M3 / "3.12" / "src/AceriaData.Application/Interfaces.cs").read_text(encoding="utf-8")
if "IQueryable<OrdenFabricacion>" in i312 or "Consulta();" in i312:
    raise RuntimeError("3.12: el puerto final sigue exponiendo IQueryable")

theory = THEORY.read_text(encoding="utf-8")
practice = PRACTICE.read_text(encoding="utf-8")

def assert_no_escaped_code(label: str, markdown: str) -> None:
    in_fence = False
    suspicious = []
    for line_no, raw in enumerate(markdown.splitlines(), 1):
        stripped = raw.strip()
        if stripped.startswith("```"):
            in_fence = not in_fence
            continue
        if in_fence:
            continue
        if (
            re.match(r"^####\s+(?:o|x|op|FROM|INNER JOIN|Espesor)", stripped)
            or re.match(r"^(?:o|x|op)\.[A-Za-z_]", stripped)
            or re.match(r"^(?:where|select)\b", stripped)
            or re.match(r"^(?:connectionString|sqlOptions)\b", stripped)
            or re.match(r"^-- Consulta \d", stripped)
            or re.match(r"^(?:SELECT|FROM|INNER JOIN|LEFT JOIN)\b", stripped)
            or re.match(r"^\.ToList\(\);?$", stripped)
            or re.match(r"^\}\)?[,]?$", stripped)
        ):
            suspicious.append(f"{line_no}: {raw}")
    if in_fence:
        raise RuntimeError(f"{label}: bloque Markdown sin cierre")
    if re.search(r"```(?:csharp|sql)\s*\n\s*```", markdown):
        raise RuntimeError(f"{label}: bloque de código vacío")
    if suspicious:
        raise RuntimeError(
            f"{label}: líneas de código fuera de bloque Markdown: " + " | ".join(suspicious[:12])
        )

def assert_no_prose_inside_code(label: str, markdown: str) -> None:
    in_fence = False
    language = ""
    suspicious = []
    for line_no, raw in enumerate(markdown.splitlines(), 1):
        stripped = raw.strip()
        match = re.match(r"^```(\w*)\s*$", stripped)
        if match:
            if in_fence:
                in_fence = False
                language = ""
            else:
                in_fence = True
                language = match.group(1).lower()
            continue
        if (
            in_fence
            and language in {"sql", "csharp", "bash"}
            and re.match(r"^(?:La|El|Las|Los|Esta|Este|Estas|Estos)\b", stripped)
            and re.search(r"[.!?]$", stripped)
        ):
            suspicious.append(f"{line_no}: {raw}")
    if suspicious:
        raise RuntimeError(
            f"{label}: prosa explicativa dentro de bloque de código: " + " | ".join(suspicious[:12])
        )

assert_no_escaped_code("TEORIA M3", theory)
assert_no_escaped_code("PRACTICA M3", practice)
assert_no_prose_inside_code("TEORIA M3", theory)
assert_no_prose_inside_code("PRACTICA M3", practice)

semantic_bans = (
    "si se llama a Count después de ToList, se ejecutan dos consultas",
    "AutoInclude estudiado sin activarlo globalmente",
    "EF Core ejecuta múltiples consultas | Cargar los datos en una sola consulta",
    "N+1 en proyecciones | Se proyecta una colección sin ToList",
    "si se proyecta antes de filtrar, EF Core puede no poder optimizar la consulta",
    "si se proyecta antes de aplicar los filtros, EF Core puede no poder optimizar la consulta",
    "si el DTO tiene propiedades calculadas o lógica en el constructor, EF Core puede no poder traducir",
    "si el constructor del DTO tiene lógica adicional, como validaciones o cálculos, EF Core puede no poder traducir",
    "si se llama a Load sin comprobar IsLoaded, EF Core puede ejecutar una consulta innecesaria",
    "Se debe marcar todas las propiedades de navegación como virtual",
    "La carga Lazy es adecuada en prototipos y en aplicaciones de escritorio, pero no en aplicaciones web",
    "La segunda es más eficiente.",
    "se usa Count() > 0 en lugar de Any(), se recorre toda la tabla",
)
for bad in semantic_bans:
    if bad.lower() in theory.lower() or bad.lower() in practice.lower():
        raise RuntimeError(f"M3: formulación técnica obsoleta o incorrecta detectada: {bad}")
if len(re.findall(r"(?m)^## Punto 3\.\d+", theory)) != 12:
    raise RuntimeError("TEORIA M3: se esperaban 12 puntos")
if len(re.findall(r"(?m)^## Punto 3\.\d+", practice)) != 12:
    raise RuntimeError("PRACTICA M3: se esperaban 12 puntos")
generic_explanations = (
    "Participa en la composición o validación concreta del flujo de este punto.",
    "Importa el espacio de nombres necesario para resolver tipos o extensiones usados por este archivo.",
    "Abre o cierra el bloque sintáctico correspondiente.",
)
for generic in generic_explanations:
    if generic in practice:
        raise RuntimeError(f"PRACTICA M3: explicación línea a línea demasiado genérica: {generic}")

for bad in ("The user wants","We need to","Let me think","Esperando confirmación para continuar","material fuente"):
    if bad.lower() in theory.lower() or bad.lower() in practice.lower():
        raise RuntimeError(f"M3: metacontenido detectado: {bad}")

matches = list(re.finditer(r"(?m)^## Punto (3\.\d+)\b.*$", practice))
for idx, m in enumerate(matches):
    end = matches[idx+1].start() if idx+1 < len(matches) else len(practice)
    section = practice[m.start():end]
    point = m.group(1)
    steps = [int(x) for x in re.findall(r"(?m)^### Paso (\d+):", section)]
    if steps != list(range(1,11)):
        raise RuntimeError(f"{point}: pasos incompletos: {steps}")
    n = int(point.split(".")[1])
    program = (M3 / point / "src/AceriaData.Console/Program.cs").read_text(encoding="utf-8").strip()
    use = (M3 / point / "src/AceriaData.Application" / USE[n]).read_text(encoding="utf-8").strip()
    if program not in section:
        raise RuntimeError(f"{point}: Program.cs real no está trazado literalmente")
    if use not in section:
        raise RuntimeError(f"{point}: caso de uso real no está trazado literalmente")
    if section.count("#### Explicación línea a línea") < 2:
        raise RuntimeError(f"{point}: faltan explicaciones línea a línea")

tokens = {
1: ("IQueryable","ToQueryString"), 2: ("Where","ThenByDescending"),
3: ("Select","OrdenResumenDto"), 4: ("OrdenCompletaDto","Planchas"),
5: ("Average","GroupBy"), 6: ("HAVING","dos consultas acotadas"),
7: ("LEFT JOIN","DefaultIfEmpty"), 8: ("Filtered Include","AsSplitQuery","AutoInclude","IgnoreAutoIncludes"),
9: ("UseLazyLoadingProxies","N+1"), 10: ("IsLoaded","Query()"),
11: ("Skip","Take"), 12: ("AsNoTracking","IQueryable"),
}
for n in range(1, 13):
    point = f"3.{n}"
    start = practice.index(f"## Punto {point}")
    end = practice.index(f"## Punto 3.{n+1}", start) if n < 12 else len(practice)
    psec = practice[start:end]
    for heading in (
        f"### Laboratorio adicional del punto {point}",
        "#### Diagnóstico técnico",
        "#### Reto resuelto y verificación adicional",
        "#### Errores comunes revisados",
        "#### Analogía operativa",
    ):
        if heading not in psec:
            raise RuntimeError(f"{point}: falta ampliación práctica: {heading}")

for n, reqs in tokens.items():
    tsec = theory[theory.index(f"## Punto 3.{n}"): theory.index(f"## Punto 3.{n+1}") if n < 12 else len(theory)]
    psec = practice[practice.index(f"## Punto 3.{n}"): practice.index(f"## Punto 3.{n+1}") if n < 12 else len(practice)]
    for token in reqs:
        if token.lower() not in (tsec + psec).lower():
            raise RuntimeError(f"3.{n}: falta contenido {token}")


root_readme = (ROOT / "README.md").read_text(encoding="utf-8")
module_readme_path = ROOT / "M03" / "README.md"
project_readme_path = M3 / "README.md"
practice_readme_path = ROOT / "M03" / "PRACTICA" / "README.md"
if "## Módulo 3 - Consultas con LINQ" not in root_readme or "M03/PROYECTO/README.md" not in root_readme:
    raise RuntimeError("README raíz: falta el índice de M3")
for path, label in (
    (module_readme_path, "README M3"),
    (project_readme_path, "README proyecto M3"),
    (practice_readme_path, "README prácticas M3"),
):
    if not path.is_file():
        raise RuntimeError(f"{label}: archivo ausente")
module_readme = module_readme_path.read_text(encoding="utf-8")
project_readme = project_readme_path.read_text(encoding="utf-8")
practice_readme = practice_readme_path.read_text(encoding="utf-8")
for n in range(1, 13):
    if f"PROYECTO/3.{n}/AceriaData.sln" not in module_readme:
        raise RuntimeError(f"README M3: falta enlace a solución 3.{n}")
    if f"3.{n}/README.md" not in project_readme:
        raise RuntimeError(f"README proyecto M3: falta checkpoint 3.{n}")
    if f"../PROYECTO/3.{n}/AceriaData.sln" not in practice_readme:
        raise RuntimeError(f"README prácticas M3: falta solución 3.{n}")

print("AUDITORÍA M3 PASS: 12 estados acumulativos, documentación, migraciones, arquitectura y trazabilidad verificadas.")
