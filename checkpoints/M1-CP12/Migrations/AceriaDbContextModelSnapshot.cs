using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AceriaData.ConsoleApp.Migrations;

[DbContext(typeof(AceriaDbContext))]
public sealed class AceriaDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.31");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 128);
        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("AceriaData.ConsoleApp.Aleacion", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<string>("Nombre").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<double>("PorcentajeCarbono").HasColumnType("float");
            b.Property<double>("PorcentajeManganeso").HasColumnType("float");
            b.HasKey("Id");
            b.ToTable("Aleaciones");
        });

        modelBuilder.Entity("AceriaData.ConsoleApp.EstadoOrden", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<string>("Descripcion").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<string>("Nombre").IsRequired().HasColumnType("nvarchar(max)");
            b.HasKey("Id");
            b.ToTable("EstadosOrden");
        });

        modelBuilder.Entity("AceriaData.ConsoleApp.OrdenFabricacion", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<string>("Cliente").IsRequired().HasColumnType("nvarchar(max)");
            b.Property<DateTime>("FechaCreacion").HasColumnType("datetime2");
            b.Property<string>("NumeroOrden").IsRequired().HasColumnType("nvarchar(max)");
            b.HasKey("Id");
            b.ToTable("OrdenesFabricacion");
        });

        modelBuilder.Entity("AceriaData.ConsoleApp.PlanchaAcero", b =>
        {
            b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
            b.Property<double>("Ancho").HasColumnType("float");
            b.Property<double>("Espesor").HasColumnType("float");
            b.Property<double>("Largo").HasColumnType("float");
            b.Property<int>("OrdenId").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("OrdenId");
            b.ToTable("PlanchasAcero");
        });

        modelBuilder.Entity("AceriaData.ConsoleApp.PlanchaAcero", b =>
        {
            b.HasOne("AceriaData.ConsoleApp.OrdenFabricacion", "Orden")
                .WithMany("Planchas")
                .HasForeignKey("OrdenId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Orden");
        });

        modelBuilder.Entity("AceriaData.ConsoleApp.OrdenFabricacion", b => b.Navigation("Planchas"));
    }
}
