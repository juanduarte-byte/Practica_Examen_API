using MiMangaBot.Domain.Entities;
using MiMangaBot.Services.Features.Prestamos;
using Microsoft.AspNetCore.Mvc;

namespace MiMangaBot.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class PrestamoController : ControllerBase
{
    private readonly PrestamoService _prestamoService;

    public PrestamoController(PrestamoService prestamoService)
    {
        _prestamoService = prestamoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var prestamos = await _prestamoService.GetAllAsync();
        return Ok(prestamos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var prestamo = await _prestamoService.GetByIdAsync(id);
        if (prestamo == null)
            return NotFound(new { message = "Préstamo no encontrado." });
        return Ok(prestamo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Prestamo prestamo)
    {
        var success = await _prestamoService.CreateAsync(prestamo);
        if (success)
            return Ok(new { message = "Préstamo creado correctamente." });
        return BadRequest(new { message = "Error al crear el préstamo." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Prestamo prestamo)
    {
        var success = await _prestamoService.UpdateAsync(id, prestamo);
        if (success)
            return Ok(new { message = "Préstamo actualizado correctamente." });
        return NotFound(new { message = "Préstamo no encontrado o no se pudo actualizar." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _prestamoService.DeleteAsync(id);
        if (success)
            return Ok(new { message = "Préstamo eliminado correctamente." });
        return NotFound(new { message = "Préstamo no encontrado o no se pudo eliminar." });
    }
}
