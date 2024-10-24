using SegurosMaticAPI.Modelos;

namespace SegurosMaticAPI.Servicios.Interfaces;

public interface IAseguradoService
{
    Task<IEnumerable<Asegurado>> ObtenerTodosAsync();
    Task<Asegurado> ObtenerPorIdAsync(int id);
    Task<Asegurado> ObtenerPorCedulaAsync(string cedula);
    Task<int> CrearAsync(Asegurado asegurado);
    Task ActualizarAsync(Asegurado asegurado);
    Task EliminarAsync(int id);
    Task AsignarSeguroAsync(int aseguradoId, int seguroId);
    Task CargarAseguradosMasivoAsync(Stream archivo);
    Task BorrarSegurosDeAsegurado(int aseguradoId);
}

