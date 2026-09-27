using Microsoft.EntityFrameworkCore;
namespace AceriaData.ConsoleApp;
public class OrdenFabricacion { public int Id {get;set;} public string NumeroOrden {get;set;}=string.Empty; public string Cliente {get;set;}=string.Empty; public DateTime FechaCreacion {get;set;} }
public class AceriaDbContext:DbContext { public DbSet<OrdenFabricacion> OrdenesFabricacion=>Set<OrdenFabricacion>(); protected override void OnConfiguring(DbContextOptionsBuilder b)=>b.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AceriaDB_CP02;Trusted_Connection=True;TrustServerCertificate=True;"); }
public static class Program { public static void Main(){ using var c=new AceriaDbContext(); c.Database.EnsureDeleted(); c.Database.EnsureCreated(); global::System.Console.WriteLine(c.Database.ProviderName); } }
