using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// DOS entidades relacionadas: E2.E1Id es FK de E1.Id.
// Una sola conexion SQLite en memoria y un solo DbContext.
using var conexion = new SqliteConnection("Data Source=:memory:");
conexion.Open();
var opciones = new DbContextOptionsBuilder<Datos>()
    .UseSqlite(conexion).Options;
using var db = new Datos(opciones);
db.Database.EnsureCreated();

var e1A = new E1 { Estado = "Anterior" };
var e1B = new E1 { Estado = "Otro" };
db.E1s.AddRange(e1A, e1B);
var e2 = new E2 { E1 = e1A };
db.E2s.Add(e2);
db.SaveChanges();

// Caso 1: cambiar la FK y LEER LA NAVEGACION inmediatamente.
Console.WriteLine("\nCASO 1: E2.E1Id = B; leer E2.E1.Id");
e2.E1Id = e1B.Id;                              // 1. Asignar FK
var lectura1 = e2.E1.Id;                       // 2. Leer navegacion
Console.WriteLine($"Antes DetectChanges: FK={e2.E1Id}, navegación.Id={lectura1}");
Exigir(lectura1 == e1A.Id, "Antes DetectChanges: navegación conserva A");
db.ChangeTracker.DetectChanges();
Console.WriteLine($"Después DetectChanges: FK={e2.E1Id}, navegación.Id={e2.E1.Id}");
Exigir(e2.E1.Id == e1B.Id, "Después DetectChanges: navegación apunta B");

// Caso 2: cambiar la NAVEGACION y LEER LA FK inmediatamente.
Console.WriteLine("\nCASO 2: E2.E1 = A; leer E2.E1Id");
e2.E1 = e1A;                                   // 1. Asignar navegacion
var lectura2 = e2.E1Id;                        // 2. Leer FK
Console.WriteLine($"Antes DetectChanges: FK={lectura2}, navegación.Id={e2.E1.Id}");
Exigir(lectura2 == e1B.Id, "Antes DetectChanges: FK conserva B");
db.ChangeTracker.DetectChanges();
Console.WriteLine($"Después DetectChanges: FK={e2.E1Id}, navegación.Id={e2.E1.Id}");
Exigir(e2.E1Id == e1A.Id, "Después DetectChanges: FK apunta A");

// Caso 3: modificar un dato NORMAL y consultarlo por dos caminos.
Console.WriteLine("\nCASO 3: E2.E1.Estado = Nuevo; leer en memoria vs SQL");
e2.E1.Estado = "Nuevo";                        // 1. Asignar dato de negocio
var lectura3 = e2.E1.Estado;                   // 2. Leer mismo objeto
Console.WriteLine($"Lectura objeto: {lectura3}");
Exigir(lectura3 == "Nuevo", "Un dato normal cambia al instante");

var valorConsultaSql = db.E1s
    .Where(x => x.Id == e1A.Id)
    .Select(x => x.Estado).Single();           // Proyección SQL: valor almacenado
Console.WriteLine($"Proyección SQL sin guardar: {valorConsultaSql}");
Exigir(valorConsultaSql == "Anterior", "La BD conserva el valor anterior");

var valorEntidadRastreada = db.E1s
    .Single(x => x.Id == e1A.Id).Estado;        // Identidad rastreada: valor de memoria
Console.WriteLine($"Consulta entidad rastreada: {valorEntidadRastreada}");
Exigir(valorEntidadRastreada == "Nuevo", "El DbContext devuelve la entidad ya rastreada");

db.SaveChanges();
var valorGuardado = db.E1s
    .Where(x => x.Id == e1A.Id)
    .Select(x => x.Estado).Single();
Console.WriteLine($"Proyección SQL después de SaveChanges: {valorGuardado}");
Exigir(valorGuardado == "Nuevo", "SaveChanges persiste el nuevo estado");
Console.WriteLine("\nRESULTADO FINAL: 3 casos reproducidos correctamente.");

static void Exigir(bool ok, string descripcion)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")}: {descripcion}");
    if (!ok) throw new InvalidOperationException(descripcion);
}

public class E1
{
    public int Id { get; set; }
    public string Estado { get; set; } = "";
    public List<E2> Relaciones { get; set; } = new();
}

public class E2
{
    public int Id { get; set; }
    public int E1Id { get; set; }
    public E1 E1 { get; set; } = null!;
}

public class Datos(DbContextOptions<Datos> opciones) : DbContext(opciones)
{
    public DbSet<E1> E1s => Set<E1>();
    public DbSet<E2> E2s => Set<E2>();

    protected override void OnModelCreating(ModelBuilder modelo) =>
        modelo.Entity<E2>()
            .HasOne(e => e.E1)
            .WithMany(e => e.Relaciones)
            .HasForeignKey(e => e.E1Id);
}