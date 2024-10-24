using SegurosMaticAPI.Modelos;

namespace SegurosMaticAPI.Servicios.Interfaces;

public interface ISeguroService
{
    Task<IEnumerable<Seguro>> ObtenerTodosAsync();
    Task<Seguro> ObtenerPorIdAsync(int id);
    Task<int> CrearAsync(Seguro seguro);
    Task ActualizarAsync(Seguro seguro);
    Task EliminarAsync(int id);
    Task<IEnumerable<Asegurado>> ObtenerAseguradosPorSeguroAsync(string codigoSeguro);
}

