namespace MiMangaBot.Domain.Entities;

public class Prestamo
{
    public int Id { get; set; }
    public int MangaId { get; set; }
    public string MangaNombre { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public string PrestadoA { get; set; } = string.Empty;
}
