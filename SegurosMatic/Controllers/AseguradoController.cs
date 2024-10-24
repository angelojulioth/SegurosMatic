using Microsoft.AspNetCore.Mvc;
using SegurosMaticAPI.Modelos;
using SegurosMaticAPI.Servicios.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AseguradoController : ControllerBase
{
    private readonly IAseguradoService _aseguradoService;
    private readonly ILogger<AseguradoController> _logger;

    public AseguradoController(IAseguradoService aseguradoService, ILogger<AseguradoController> logger)
    {
        _aseguradoService = aseguradoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asegurado>>> ObtenerTodos()
    {
        try
        {
            var asegurados = await _aseguradoService.ObtenerTodosAsync();
            return Ok(asegurados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asegurados");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Asegurado>> ObtenerPorId(int id)
    {
        try
        {
            var asegurado = await _aseguradoService.ObtenerPorIdAsync(id);
            if (asegurado == null)
                return NotFound();
            return Ok(asegurado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asegurado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("cedula/{cedula}")]
    public async Task<ActionResult<Asegurado>> ObtenerPorCedula(string cedula)
    {
        try
        {
            var asegurado = await _aseguradoService.ObtenerPorCedulaAsync(cedula);
            if (asegurado == null)
                return NotFound();
            return Ok(asegurado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asegurado por cédula");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> Crear(Asegurado asegurado)
    {
        try
        {
            var id = await _aseguradoService.CrearAsync(asegurado);
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear asegurado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, Asegurado asegurado)
    {
        try
        {
            if (id != asegurado.Id)
                return BadRequest();

            await _aseguradoService.ActualizarAsync(asegurado);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar asegurado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _aseguradoService.EliminarAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asegurado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("{aseguradoId}/seguros/{seguroId}")]
    public async Task<IActionResult> AsignarSeguro(int aseguradoId, int seguroId)
    {
        try
        {
            await _aseguradoService.AsignarSeguroAsync(aseguradoId, seguroId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    
    [HttpDelete("{aseguradoId}/seguros")]
    public async Task<IActionResult> BorrarSegurosDeAsegurado(int aseguradoId)
    {
        try
        {
            await _aseguradoService.BorrarSegurosDeAsegurado(aseguradoId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("carga-masiva")]
    public async Task<IActionResult> CargarMasivo(IFormFile archivo)
    {
        try
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se ha proporcionado ningún archivo");

            using var stream = archivo.OpenReadStream();
            await _aseguradoService.CargarAseguradosMasivoAsync(stream);
            return Ok("Archivo procesado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar archivo de carga masiva");
            return StatusCode(500, "Error al procesar el archivo");
        }
    }
}