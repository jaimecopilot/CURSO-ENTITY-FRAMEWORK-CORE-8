using Microsoft.EntityFrameworkCore;
namespace AceriaData.ConsoleApp;
public class OrdenFabricacion { public int Id{get;set;} public string NumeroOrden{get;set;}=string.Empty; public string Cliente{get;set;}=string.Empty; public DateTime FechaCreacion{get;set;} public List<PlanchaAcero> Planchas{get;set;}=new(); }
public class PlanchaAcero { public int Id{get;set;} public int OrdenId{get;set;} public double Espesor{get;set;} public double Ancho{get;set;} public double Largo{get;set;} public OrdenFabricacion Orden{get;set;}=null!; }
public class Aleacion { public int Id{get;set;} public string Nombre{get;set;}=string.Empty; public double PorcentajeCarbono{get;set;} public double PorcentajeManganeso{get;set;} }
public class EstadoOrden { public int Id{get;set;} public string Nombre{get;set;}=string.Empty; public string Descripcion{get;set;}=string.Empty; }
public class AceriaDbContext:DbContext { public DbSet<OrdenFabricacion> OrdenesFabricacion=>Set<OrdenFabricacion>(); public DbSet<PlanchaAcero> PlanchasAcero=>Set<PlanchaAcero>(); public DbSet<Aleacion> Aleaciones=>Set<Aleacion>(); public DbSet<EstadoOrden> EstadosOrden=>Set<EstadoOrden>(); protected override void OnConfiguring(DbContextOptionsBuilder b)=>b.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB;Trusted_Connection=True;TrustServerCertificate=True;"); }
public static class Program { public static void Main(){ using var c=new AceriaDbContext(); c.Database.EnsureDeleted(); c.Database.Migrate(); global::System.Console.WriteLine(string.Join(", ",c.Model.GetEntityTypes().Select(x=>x.ClrType.Name))); } }
