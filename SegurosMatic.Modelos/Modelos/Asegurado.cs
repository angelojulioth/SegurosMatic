using System.ComponentModel.DataAnnotations;

namespace SegurosMaticAPI.Modelos;

public class Asegurado
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La cédula es requerida")]
    [StringLength(20, ErrorMessage = "La cédula no puede exceder los 20 caracteres")]
    public string Cedula { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El teléfono es requerido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "La edad es requerida")]
    [Range(0, 150, ErrorMessage = "La edad debe estar entre 0 y 150 años")]
    public int Edad { get; set; }

    public List<Seguro> Seguros { get; set; } = new List<Seguro>();
}
