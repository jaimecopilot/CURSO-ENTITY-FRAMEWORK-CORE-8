from pathlib import Path
import re, shutil, subprocess, tempfile, sys

ROOT = Path(__file__).resolve().parents[1]
PRACTICE = ROOT / "M01" / "PRACTICA" / "M01_PRACTICA.md"
TEXT = PRACTICE.read_text(encoding="utf-8")

def section(n):
    m = re.search(rf"^## Punto 1\.{n}\b.*$", TEXT, re.M)
    if not m:
        raise RuntimeError(f"No existe el punto 1.{n}")
    if n < 12:
        nxt = re.search(rf"^## Punto 1\.{n+1}\b.*$", TEXT[m.end():], re.M)
        end = m.end() + nxt.start() if nxt else len(TEXT)
    else:
        end = len(TEXT)
    return TEXT[m.start():end]

def fenced(sec):
    return re.findall(r"```([^\n]*)\n(.*?)```", sec, re.S)

def full_program(n):
    candidates = [
        code for lang, code in fenced(section(n))
        if lang.strip().lower() in ("csharp", "cs")
        and "public class Program" in code
        and "using Microsoft.EntityFrameworkCore;" in code
    ]
    if not candidates:
        return None
    return max(candidates, key=len)

def challenge_cs(n):
    sec = section(n)
    h = re.search(r"^### Reto resuelto:.*$", sec, re.M)
    if not h:
        return []
    return [
        code for lang, code in fenced(sec[h.end():])
        if lang.strip().lower() in ("csharp", "cs")
    ]

def block_end(src, open_brace):
    depth = 0
    for i in range(open_brace, len(src)):
        if src[i] == "{":
            depth += 1
        elif src[i] == "}":
            depth -= 1
            if depth == 0:
                return i
    raise RuntimeError("Llaves desbalanceadas")

def program_position(src):
    positions = [src.find(x) for x in ("public static class Program", "public class Program") if src.find(x) >= 0]
    if not positions:
        raise RuntimeError("No se encuentra la clase Program")
    return min(positions)

def inject_program_method_and_call(src, method, call):
    pc = program_position(src)
    p_open = src.index("{", pc)
    p_end = block_end(src, p_open)
    src = src[:p_end] + "\n\n    " + method.strip().replace("\n", "\n    ") + "\n" + src[p_end:]
    pc = program_position(src)
    main_markers = ("public static void Main()", "public static async Task Main()")
    pos = next((src.find(x, pc) for x in main_markers if src.find(x, pc) >= 0), -1)
    if pos < 0:
        raise RuntimeError("No se encuentra Main")
    m_open = src.index("{", pos)
    m_end = block_end(src, m_open)
    src = src[:m_end] + "\n        " + call.strip().replace("\n", "\n        ") + "\n" + src[m_end:]
    return src

def replace_onconfiguring(src, replacement):
    marker = "protected override void OnConfiguring"
    start = src.index(marker)
    open_brace = src.index("{", start)
    end = block_end(src, open_brace)
    return src[:start] + replacement.strip() + src[end+1:]

def run(cmd, cwd):
    print("+", " ".join(cmd))
    p = subprocess.run(cmd, cwd=cwd, text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    print(p.stdout)
    if p.returncode:
        raise RuntimeError(f"Falló {' '.join(cmd)} en {cwd}")
    return p.stdout

# 1) La práctica contiene exactamente los 12 puntos.
for n in range(1, 13):
    section(n)

# 2) Trazabilidad mínima práctica -> checkpoint.
criteria = {
    1: ["net8.0", "Microsoft.EntityFrameworkCore.SqlServer"],
    2: ["OrdenFabricacion", "AceriaDbContext", "EnsureCreated", "UseSqlServer"],
    3: ["PlanchaAcero", "Aleacion", "InitialCreate", "AddAleacion"],
    4: ["EstadoOrden", "AddEstadoOrden"],
    5: ["AceriaDbContextFactory", "InsertarOrden", "ListarOrdenes"],
    6: ["InsertarOrden", "ListarOrdenes", "ActualizarCliente", "EliminarOrden", "BuscarPorId", "ExisteOrden", "ContarOrdenes", "InsertarPlancha"],
    7: ["ChangeTracker", "DetectChanges", "OriginalValues", "Clear()"],
    8: ["AddRange", "Attach", "Entry(", "Remove("],
    9: ["SaveChanges", "SaveChangesAsync", "DbUpdateException"],
    10:["appsettings.json", "ConfigurationBuilder", "LogTo", "EnableDetailedErrors"],
    11:["ProviderName", "ToQueryString", "UseSqlServer"],
    12:["AddDbContext", "IOrdenRepositorio", "OrdenRepositorio", "IServicioOrdenes", "ServicioOrdenes"],
}
for n, tokens in criteria.items():
    cp = ROOT / "checkpoints" / f"M1-CP{n:02d}"
    if not cp.is_dir():
        raise RuntimeError(f"Falta {cp}")
    cp_text = "\n".join(
        p.read_text(encoding="utf-8", errors="ignore")
        for p in cp.rglob("*") if p.is_file() and p.suffix.lower() in (".cs", ".csproj", ".json", ".md")
    )
    sec = section(n)
    missing_cp = [t for t in tokens if t not in cp_text]
    missing_doc = [t for t in tokens if t not in sec and t not in ("InitialCreate","AddAleacion","AddEstadoOrden")]
    if missing_cp:
        raise RuntimeError(f"1.{n}: el checkpoint no cubre {missing_cp}")
    if missing_doc:
        raise RuntimeError(f"1.{n}: la práctica no documenta {missing_doc}")
    print(f"TRACE PASS 1.{n}: práctica -> M1-CP{n:02d}")

# 3) No hay proveedores ejecutables alternativos en M1.
code = "\n".join(
    p.read_text(encoding="utf-8", errors="ignore")
    for base in (ROOT/"src", ROOT/"checkpoints")
    for p in base.rglob("*") if p.is_file() and p.suffix.lower() in (".cs",".csproj")
)
for forbidden in ("UseSqlite(", "UseNpgsql(", "UseInMemoryDatabase("):
    if forbidden in code:
        raise RuntimeError(f"Proveedor alternativo ejecutable en M1: {forbidden}")

# 4) Compilar y ejecutar directamente código publicado en la práctica.
#    1.1 usa top-level statements; 1.2+ emplean Program.cs completo.
with tempfile.TemporaryDirectory(prefix="m1-intro-") as intro_tmp:
    intro_tmp = Path(intro_tmp)
    blocks_11 = [code for lang, code in fenced(section(1)) if lang.strip().lower() in ("csharp","cs")]
    if not blocks_11:
        raise RuntimeError("1.1: no se encontraron bloques C#")
    intro = max(blocks_11, key=len)
    target = intro_tmp / "M1-CP01"
    shutil.copytree(ROOT/"checkpoints"/"M1-CP01", target)
    (target/"Program.cs").write_text(intro.rstrip()+"\n", encoding="utf-8")
    run(["dotnet","restore","AceriaData.Console.csproj"], target)
    run(["dotnet","build","AceriaData.Console.csproj","--configuration","Release","--no-restore"], target)
    output = run(["dotnet","run","--project","AceriaData.Console.csproj","--configuration","Release","--no-build"], target)
    for evidence in ("ACER", "AceriaData", ".NET"):
        if evidence not in output:
            raise RuntimeError(f"1.1: falta evidencia {evidence}")
    print("PRACTICE E2E PASS 1.1")

# 4) Compilar y ejecutar directamente los Program.cs completos publicados en la práctica.
#    Esto valida que el código docente del MD no sea sólo ilustrativo.
points = [2, 3, 5, 6, 7, 8, 9, 10]
expected_output = {
    2: ["Base de datos AceriaDB creada correctamente"],
    3: ["Microsoft.EntityFrameworkCore.SqlServer"],
    5: ["OF-003", "Planchas insertadas:"],
    6: ["Plancha insertada: True"],
    7: ["Entidades modificadas: 2", "Cambios guardados."],
    8: ["Cliente actualizado para la orden 5"],
    9: ["Error capturado:", "Error de base de datos:", "Filas afectadas con SaveChangesAsync:"],
    10: ["OF-001", "OF-002"],
}
with tempfile.TemporaryDirectory(prefix="m1-practice-") as tmp:
    tmp = Path(tmp)
    for n in points:
        src = full_program(n)
        if not src:
            raise RuntimeError(f"1.{n}: no se encontró Program.cs completo en la práctica")
        reto = challenge_cs(n)
        if n in (5,6,7,8,9) and len(reto) >= 2:
            src = inject_program_method_and_call(src, reto[0], reto[1])
        target = tmp / f"M1-CP{n:02d}"
        shutil.copytree(ROOT/"checkpoints"/f"M1-CP{n:02d}", target)
        (target/"Program.cs").write_text(src, encoding="utf-8")
        run(["dotnet","restore","AceriaData.Console.csproj"], target)
        run(["dotnet","build","AceriaData.Console.csproj","--configuration","Release","--no-restore"], target)
        output = run(["dotnet","run","--project","AceriaData.Console.csproj","--configuration","Release","--no-build"], target)
        missing = [x for x in expected_output[n] if x not in output]
        if missing:
            raise RuntimeError(f"1.{n}: faltan evidencias de ejecución {missing}")
        print(f"PRACTICE E2E PASS 1.{n}: evidencias {expected_output[n]}")

    # Reto 1.10: variante específica de OnConfiguring con logging a archivo.
    reto10 = challenge_cs(10)
    if reto10:
        src = replace_onconfiguring(full_program(10), reto10[0])
        target = tmp / "M1-CP10-RETO"
        shutil.copytree(ROOT/"checkpoints"/"M1-CP10", target)
        (target/"Program.cs").write_text(src, encoding="utf-8")
        run(["dotnet","restore","AceriaData.Console.csproj"], target)
        run(["dotnet","build","AceriaData.Console.csproj","--configuration","Release","--no-restore"], target)
        run(["dotnet","run","--project","AceriaData.Console.csproj","--configuration","Release","--no-build"], target)
        print("PRACTICE E2E PASS 1.10 RETO")

# 5) Validar el reto 1.11 sobre un contenedor DI real.
with tempfile.TemporaryDirectory(prefix="m1-provider-") as provider_tmp:
    provider_tmp = Path(provider_tmp)
    target = provider_tmp / "M1-CP11-RETO"
    shutil.copytree(ROOT/"checkpoints"/"M1-CP12", target)
    src = (target/"Program.cs").read_text(encoding="utf-8")
    reto11 = challenge_cs(11)
    if len(reto11) < 2:
        raise RuntimeError("1.11: reto resuelto incompleto")
    src = inject_program_method_and_call(src, reto11[0], reto11[1])
    (target/"Program.cs").write_text(src, encoding="utf-8")
    run(["dotnet","restore","AceriaData.Console.csproj"], target)
    run(["dotnet","build","AceriaData.Console.csproj","--configuration","Release","--no-restore"], target)
    output = run(["dotnet","run","--project","AceriaData.Console.csproj","--configuration","Release","--no-build"], target)
    for evidence in ("Microsoft.EntityFrameworkCore.SqlServer", "--- SQL generado ---", "OrdenFabricacion", "PlanchaAcero"):
        if evidence not in output:
            raise RuntimeError(f"1.11: falta evidencia {evidence}")
    print("PRACTICE E2E PASS 1.11")

# 6) El estado final y CP12 deben coincidir en los artefactos ejecutables clave.
for rel in ("Program.cs","AceriaData.Console.csproj","AceriaDesignTimeDbContextFactory.cs","appsettings.json"):
    a = (ROOT/"src"/"AceriaData.Console"/rel).read_bytes()
    b = (ROOT/"checkpoints"/"M1-CP12"/rel).read_bytes()
    if a != b:
        raise RuntimeError(f"CP12 diverge del estado final: {rel}")

print("AUDITORÍA M1 PASS: práctica, checkpoints, código fuente y E2E trazados.")
