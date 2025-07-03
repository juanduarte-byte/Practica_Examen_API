using MiMangaBot.Domain.Entities;
using MySqlConnector;
using System.Data;

namespace MiMangaBot.Services.Features.Mangas;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public int? NextPage { get; set; }
    public int? PreviousPage { get; set; }
}

public class MangaService
{
    private readonly string _connectionString;


    public MangaService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM Manga WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> UpdateAsync(int id, Manga manga)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            UPDATE Manga SET
                Title = @Title,
                Author = @Author,
                Genre = @Genre,
                PublicationDate = @PublicationDate,
                Volumes = @Volumes,
                IsOngoing = @IsOngoing
            WHERE Id = @Id;", connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Title", manga.Title);
        command.Parameters.AddWithValue("@Author", manga.Author);
        command.Parameters.AddWithValue("@Genre", manga.Genre ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@PublicationDate", manga.PublicationDate);
        command.Parameters.AddWithValue("@Volumes", manga.Volumes);
        command.Parameters.AddWithValue("@IsOngoing", manga.IsOngoing);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<PagedResult<Manga>> GetAllPagedAsync(int page, int pageSize)
    {
        var result = new PagedResult<Manga>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        // Obtener el total de registros
        var countCommand = new MySqlCommand("SELECT COUNT(*) FROM Manga", connection);
        var totalRecords = Convert.ToInt32(await countCommand.ExecuteScalarAsync());

        // Calcular paginación
        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        int offset = (page - 1) * pageSize;

        var command = new MySqlCommand($"SELECT * FROM Manga LIMIT @PageSize OFFSET @Offset", connection);
        command.Parameters.AddWithValue("@PageSize", pageSize);
        command.Parameters.AddWithValue("@Offset", offset);

        using var reader = await command.ExecuteReaderAsync();
        var mangas = new List<Manga>();
        while (await reader.ReadAsync())
        {
            mangas.Add(new Manga
            {
                Id = reader.GetInt32("Id"),
                Title = reader.GetString("Title"),
                Author = reader.GetString("Author"),
                Genre = reader.IsDBNull("Genre") ? null : reader.GetString("Genre"),
                PublicationDate = reader.GetDateTime("PublicationDate"),
                Volumes = reader.GetInt32("Volumes"),
                IsOngoing = reader.GetBoolean("IsOngoing")
            });
        }

        result.Items = mangas;
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.TotalPages = totalPages;
        result.TotalRecords = totalRecords;
        result.NextPage = page < totalPages ? page + 1 : (int?)null;
        result.PreviousPage = page > 1 ? page - 1 : (int?)null;

        return result;
    }

    public async Task<bool> CreateAsync(Manga manga)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
        INSERT INTO Manga (Title, Author, Genre, PublicationDate, Volumes, IsOngoing)
        VALUES (@Title, @Author, @Genre, @PublicationDate, @Volumes, @IsOngoing);", connection);

        command.Parameters.AddWithValue("@Title", manga.Title);
        command.Parameters.AddWithValue("@Author", manga.Author);
        command.Parameters.AddWithValue("@Genre", manga.Genre ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@PublicationDate", manga.PublicationDate);
        command.Parameters.AddWithValue("@Volumes", manga.Volumes);
        command.Parameters.AddWithValue("@IsOngoing", manga.IsOngoing);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<Manga?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT * FROM Manga WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Manga
            {
                Id = reader.GetInt32("Id"),
                Title = reader.GetString("Title"),
                Author = reader.GetString("Author"),
                Genre = reader.IsDBNull("Genre") ? null : reader.GetString("Genre"),
                PublicationDate = reader.GetDateTime("PublicationDate"),
                Volumes = reader.GetInt32("Volumes"),
                IsOngoing = reader.GetBoolean("IsOngoing")
            };
        }

        return null;
    }

}