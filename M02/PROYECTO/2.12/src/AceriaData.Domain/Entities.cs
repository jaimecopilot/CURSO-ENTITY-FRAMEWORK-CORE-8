namespace AceriaData.Domain.Entities;

public class OrdenFabricacion
{
    public int Id { get; set; }
    public string NumeroOrden { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string? Observaciones { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public List<PlanchaAcero> Planchas { get; set; } = new();
    public DetalleOrden? Detalle { get; set; }
    public CertificadoCalidad? Certificado { get; set; }
    public List<OrdenAleacion> OrdenesAleaciones { get; set; } = new();
}

public class PlanchaAcero
{
    public int Id { get; set; }
    public int OrdenId { get; set; }
    public double Espesor { get; set; }
    public double Ancho { get; set; }
    public double Largo { get; set; }
    public decimal Peso { get; set; }
    public bool Activa { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public OrdenFabricacion Orden { get; set; } = null!;
}

public class Aleacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public double PorcentajeCarbono { get; set; }
    public double PorcentajeManganeso { get; set; }
    public string? Descripcion { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public List<OrdenAleacion> OrdenesAleaciones { get; set; } = new();
}

public class EstadoOrden
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class DetalleOrden
{
    public int Id { get; set; }
    public int OrdenId { get; set; }
    public string ComposicionQuimica { get; set; } = string.Empty;
    public double TemperaturaColada { get; set; }
    public string? Notas { get; set; }
    public OrdenFabricacion Orden { get; set; } = null!;
}

public class CertificadoCalidad
{
    public int Id { get; set; }
    public int OrdenId { get; set; }
    public string NumeroCertificado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string OrganismoCertificador { get; set; } = string.Empty;
    public OrdenFabricacion Orden { get; set; } = null!;
}

public class OrdenAleacion
{
    public int OrdenFabricacionId { get; set; }
    public int AleacionId { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public decimal CantidadUtilizada { get; set; }
    public string EstadoRelacion { get; set; } = "Activa";
    public OrdenFabricacion Orden { get; set; } = null!;
    public Aleacion Aleacion { get; set; } = null!;
}

// ============================================================================
// EJEMPLO DEL PASO 2
// Dominio independiente de EF Core.
// ACTIVACIÓN PEDAGÓGICA:
// En una copia de trabajo, comenta temporalmente la implementación activa de
// este archivo y elimina el prefijo "// " de esta copia para reconstruir el
// fragmento del PASO 2 exactamente en su ubicación arquitectónica.
// ----------------------------------------------------------------------------
// namespace AceriaData.Domain.Entities;
// 
// public class OrdenFabricacion
// {
//     public int Id { get; set; }
//     public string NumeroOrden { get; set; } = string.Empty;
//     public string Cliente { get; set; } = string.Empty;
//     public DateTime FechaCreacion { get; set; }
//     public DateTime? FechaEntrega { get; set; }
//     public string Estado { get; set; } = "Pendiente";
//     public string? Observaciones { get; set; }
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
//     public List<PlanchaAcero> Planchas { get; set; } = new();
//     public DetalleOrden? Detalle { get; set; }
//     public CertificadoCalidad? Certificado { get; set; }
//     public List<OrdenAleacion> OrdenesAleaciones { get; set; } = new();
// }
// 
// public class PlanchaAcero
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public double Espesor { get; set; }
//     public double Ancho { get; set; }
//     public double Largo { get; set; }
//     public decimal Peso { get; set; }
//     public bool Activa { get; set; } = true;
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
//     public OrdenFabricacion Orden { get; set; } = null!;
// }
// 
// public class Aleacion
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public string Codigo { get; set; } = string.Empty;
//     public double PorcentajeCarbono { get; set; }
//     public double PorcentajeManganeso { get; set; }
//     public string? Descripcion { get; set; }
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
//     public List<OrdenAleacion> OrdenesAleaciones { get; set; } = new();
// }
// 
// public class EstadoOrden
// {
//     public int Id { get; set; }
//     public string Nombre { get; set; } = string.Empty;
//     public string Descripcion { get; set; } = string.Empty;
//     public bool Activo { get; set; } = true;
//     public bool IsDeleted { get; set; }
//     public DateTime? DeletedAt { get; set; }
// }
// 
// public class DetalleOrden
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public string ComposicionQuimica { get; set; } = string.Empty;
//     public double TemperaturaColada { get; set; }
//     public string? Notas { get; set; }
//     public OrdenFabricacion Orden { get; set; } = null!;
// }
// 
// public class CertificadoCalidad
// {
//     public int Id { get; set; }
//     public int OrdenId { get; set; }
//     public string NumeroCertificado { get; set; } = string.Empty;
//     public DateTime FechaEmision { get; set; }
//     public string OrganismoCertificador { get; set; } = string.Empty;
//     public OrdenFabricacion Orden { get; set; } = null!;
// }
// 
// public class OrdenAleacion
// {
//     public int OrdenFabricacionId { get; set; }
//     public int AleacionId { get; set; }
//     public DateTime FechaAsignacion { get; set; } = DateTime.Now;
//     public decimal CantidadUtilizada { get; set; }
//     public string EstadoRelacion { get; set; } = "Activa";
//     public OrdenFabricacion Orden { get; set; } = null!;
//     public Aleacion Aleacion { get; set; } = null!;
// }
// ============================================================================
