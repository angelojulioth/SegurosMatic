using Microsoft.AspNetCore.Mvc;
using SegurosMaticAPI.Modelos;
using SegurosMaticAPI.Servicios.Interfaces;

namespace SegurosMaticAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeguroController : ControllerBase
{
    private readonly ISeguroService _seguroService;
    private readonly ILogger<SeguroController> _logger;

    public SeguroController(ISeguroService seguroService, ILogger<SeguroController> logger)
    {
        _seguroService = seguroService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Seguro>>> ObtenerTodos()
    {
        try
        {
            var seguros = await _seguroService.ObtenerTodosAsync();
            return Ok(seguros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener seguros");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Seguro>> ObtenerPorId(int id)
    {
        try
        {
            var seguro = await _seguroService.ObtenerPorIdAsync(id);
            if (seguro == null)
                return NotFound();
            return Ok(seguro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost]
    public async Task<ActionResult<int>> Crear(Seguro seguro)
    {
        try
        {
            var id = await _seguroService.CrearAsync(seguro);
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, Seguro seguro)
    {
        try
        {
            if (id != seguro.Id)
                return BadRequest();

            await _seguroService.ActualizarAsync(seguro);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await _seguroService.EliminarAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }


    // Obtener asegurados (suscritos a seguro mediante código)
    [HttpGet("asegurados/{codigoSeguro}")]
    public async Task<ActionResult<IEnumerable<Asegurado>>> ObtenerAseguradosPorSeguro(string codigoSeguro)
    {
        try
        {
            var asegurados = await _seguroService.ObtenerAseguradosPorSeguroAsync(codigoSeguro);
            return Ok(asegurados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener asegurados por seguro");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
