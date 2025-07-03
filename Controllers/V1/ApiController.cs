// JaveragesLibrary/Controllers/V1/MangaController.cs
using MiMangaBot.Domain.Entities;
using MiMangaBot.Services.Features.Mangas; // ¡Necesitamos nuestro servicio!
using Microsoft.AspNetCore.Mvc;                 // Para [ApiController], [Route], IActionResult, etc.

namespace MiMangaBot.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class ApiController : ControllerBase
{
    private readonly MangaService _mangaService;

    public ApiController(MangaService mangaService)
    {
        _mangaService = mangaService;
    }

    [HttpDelete("mangas/{id}")]
    public async Task<IActionResult> DeleteManga(int id)
    {
        var success = await _mangaService.DeleteAsync(id);
        if (success)
            return Ok(new { message = "Manga eliminado correctamente." });
        return NotFound(new { message = "Manga no encontrado o no se pudo eliminar." });
    }

    [HttpPut("mangas/{id}")]
    public async Task<IActionResult> UpdateManga(int id, [FromBody] Manga manga)
    {
        var success = await _mangaService.UpdateAsync(id, manga);
        if (success)
            return Ok(new { message = "Manga actualizado correctamente." });
        return NotFound(new { message = "Manga no encontrado o no se pudo actualizar." });
    }

    [HttpGet("mangas")]
    public async Task<IActionResult> GetMangas([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var pagedResult = await _mangaService.GetAllPagedAsync(page, pageSize);
        return Ok(pagedResult);
    }

    [HttpPost("mangas")]
    public async Task<IActionResult> CreateManga([FromBody] Manga manga)
    {
        var success = await _mangaService.CreateAsync(manga);
        if (success)
            return Ok(new { message = "Manga creado correctamente." });

        return BadRequest(new { message = "Error al crear el manga." });
    }

    [HttpGet("mangas/{id}")]
    public async Task<IActionResult> GetMangaById(int id)
    {
        var manga = await _mangaService.GetByIdAsync(id);
        if (manga == null)
            return NotFound(new { message = "Manga no encontrado." });
        return Ok(manga);
    }
}