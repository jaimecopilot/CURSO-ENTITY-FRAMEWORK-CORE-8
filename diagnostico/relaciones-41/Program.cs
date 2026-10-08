using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Collections.ObjectModel;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
var options = new DbContextOptionsBuilder<TestDb>().UseSqlite(connection).Options;
using var db = new TestDb(options);
db.Database.EnsureCreated();
var orden1 = new OrdenFabricacion { NumeroOrden = "OF-1" };
var orden2 = new OrdenFabricacion { NumeroOrden = "OF-2" };
db.OrdenesFabricacion.AddRange(orden1, orden2);
db.SaveChanges();
var plancha1 = new PlanchaAcero { OrdenId = orden1.Id };
var plancha2 = new PlanchaAcero { OrdenId = orden1.Id };
var plancha3 = new PlanchaAcero { OrdenId = orden2.Id };
db.PlanchasAcero.AddRange(plancha1, plancha2, plancha3);
Console.WriteLine("ANTES:\n" + db.ChangeTracker.DebugView.LongView);
plancha1.Orden = orden2;
db.ChangeTracker.DetectChanges();
Check("nav => FK", plancha1.OrdenId == orden2.Id);
plancha2.OrdenId = orden2.Id;
db.ChangeTracker.DetectChanges();
Check("FK => nav", plancha2.Orden is not null && plancha2.Orden.Id == orden2.Id);
Console.WriteLine("DESPUES DE DETECTCHANGES:\n" + db.ChangeTracker.DebugView.LongView);
db.SaveChanges();
Check("persistencia FK 1", db.PlanchasAcero.AsNoTracking().Single(p => p.Id == plancha1.Id).OrdenId == orden2.Id);
Check("persistencia FK 2", db.PlanchasAcero.AsNoTracking().Single(p => p.Id == plancha2.Id).OrdenId == orden2.Id);
Console.WriteLine("PASS: ambos cambios de relación sincronizados y persistidos.");

static void Check(string name, bool ok) {
 Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
 if (!ok) throw new Exception("Fallo: " + name);
}
public class OrdenFabricacion {
 public int Id { get; set; }
 public string NumeroOrden { get; set; } = "";
 public ObservableCollection<PlanchaAcero> Planchas { get; set; } = new();
}
public class PlanchaAcero : INotifyPropertyChanging {
 public int Id { get; set; }
 public int OrdenId { get; set; }
 public OrdenFabricacion Orden { get; set; } = null!;
 public event PropertyChangingEventHandler? PropertyChanging;
}
public class TestDb(DbContextOptions<TestDb> options) : DbContext(options) {
 public DbSet<OrdenFabricacion> OrdenesFabricacion => Set<OrdenFabricacion>();
 public DbSet<PlanchaAcero> PlanchasAcero => Set<PlanchaAcero>();
 protected override void OnModelCreating(ModelBuilder modelBuilder) {
  modelBuilder.Entity<OrdenFabricacion>().HasKey(x => x.Id);
  modelBuilder.Entity<PlanchaAcero>().HasKey(x => x.Id);
  modelBuilder.Entity<PlanchaAcero>().HasOne(x => x.Orden).WithMany(x => x.Planchas).HasForeignKey(x => x.OrdenId).IsRequired();
 }
}