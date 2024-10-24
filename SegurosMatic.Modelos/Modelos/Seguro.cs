using System.ComponentModel.DataAnnotations;

namespace SegurosMaticAPI.Modelos;

public class Seguro
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(20, ErrorMessage = "El código no puede exceder los 20 caracteres")]
    public string Codigo { get; set; }

    [Required(ErrorMessage = "La suma asegurada es requerida")]
    [Range(0, double.MaxValue, ErrorMessage = "La suma asegurada debe ser mayor a 0")]
    public decimal SumaAsegurada { get; set; }

    [Required(ErrorMessage = "La prima es requerida")]
    [Range(0, double.MaxValue, ErrorMessage = "La prima debe ser mayor a 0")]
    public decimal Prima { get; set; }

    [Required(ErrorMessage = "La edad mínima es requerida")]
    [Range(0, 120, ErrorMessage = "La edad debe ser mayor a 0 y menor a 120")]
    public int EdadMinima { get; set; }

    [Required(ErrorMessage = "La edad máxima es requerida")]
    [Range(0, 120, ErrorMessage = "La edad debe ser mayor a 0 y menor a 120")]
    public int EdadMaxima { get; set; }
}
