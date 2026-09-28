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

# AutoInclude curricular: se demuestra realmente en 3.8 y se propagará hasta 3.11.
cfg38 = (M3 / "3.8" / "src/AceriaData.Infrastructure/Persistence/Configurations/OrdenFabricacionConfiguration.cs").read_text(encoding="utf-8")
repo38 = (M3 / "3.8" / "src/AceriaData.Infrastructure/Repositories/Repositories.cs").read_text(encoding="utf-8")
use38 = (M3 / "3.8" / "src/AceriaData.Application/CargaEagerUseCase.cs").read_text(encoding="utf-8")
interfaces38 = (M3 / "3.8" / "src/AceriaData.Application/Interfaces.cs").read_text(encoding="utf-8")
if "Navigation(x => x.Planchas).AutoInclude()" not in cfg38:
    raise RuntimeError("3.8: falta AutoInclude real sobre Planchas")
for token in ("ObtenerOrdenesAutoInclude", "ObtenerOrdenesIgnorandoAutoInclude"):
    if token not in repo38 or token not in interfaces38 or token not in use38:
        raise RuntimeError(f"3.8: falta demostración ejecutable de {token}")
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

for n in range(10, 13):
    di = (M3 / f"3.{n}" / "src/AceriaData.Infrastructure/DependencyInjection.cs").read_text(encoding="utf-8")
    if "UseLazyLoadingProxies" in di:
        raise RuntimeError(f"3.{n}: Lazy Loading debía quedar desactivado")

i312 = (M3 / "3.12" / "src/AceriaData.Application/Interfaces.cs").read_text(encoding="utf-8")
if "IQueryable<OrdenFabricacion>" in i312 or "Consulta();" in i312:
    raise RuntimeError("3.12: el puerto final sigue exponiendo IQueryable")

theory = THEORY.read_text(encoding="utf-8")
practice = PRACTICE.read_text(encoding="utf-8")
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
for n, reqs in tokens.items():
    tsec = theory[theory.index(f"## Punto 3.{n}"): theory.index(f"## Punto 3.{n+1}") if n < 12 else len(theory)]
    psec = practice[practice.index(f"## Punto 3.{n}"): practice.index(f"## Punto 3.{n+1}") if n < 12 else len(practice)]
    for token in reqs:
        if token.lower() not in (tsec + psec).lower():
            raise RuntimeError(f"3.{n}: falta contenido {token}")

print("AUDITORÍA M3 PASS: 12 estados acumulativos, documentación, migraciones, arquitectura y trazabilidad verificadas.")
