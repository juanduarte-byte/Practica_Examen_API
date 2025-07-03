using MiMangaBot.Domain.Entities;
using MySqlConnector;

namespace MiMangaBot.Services.Features.Prestamos;

public class PrestamoService
{
    private readonly string _connectionString;

    public PrestamoService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand("DELETE FROM Prestamo WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<bool> UpdateAsync(int id, Prestamo prestamo)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            UPDATE Prestamo SET
                MangaId = @MangaId,
                MangaNombre = @MangaNombre,
                FechaPrestamo = @FechaPrestamo,
                PrestadoA = @PrestadoA
            WHERE Id = @Id;", connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@MangaId", prestamo.MangaId);
        command.Parameters.AddWithValue("@MangaNombre", prestamo.MangaNombre);
        command.Parameters.AddWithValue("@FechaPrestamo", prestamo.FechaPrestamo);
        command.Parameters.AddWithValue("@PrestadoA", prestamo.PrestadoA);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<Prestamo?> GetByIdAsync(int id)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT * FROM Prestamo WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Prestamo
            {
                Id = reader.GetInt32("Id"),
                MangaId = reader.GetInt32("MangaId"),
                MangaNombre = reader.GetString("MangaNombre"),
                FechaPrestamo = reader.GetDateTime("FechaPrestamo"),
                PrestadoA = reader.GetString("PrestadoA")
            };
        }
        return null;
    }
    
    public async Task<bool> CreateAsync(Prestamo prestamo)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand(@"
            INSERT INTO Prestamo (MangaId, MangaNombre, FechaPrestamo, PrestadoA)
            VALUES (@MangaId, @MangaNombre, @FechaPrestamo, @PrestadoA);", connection);

        command.Parameters.AddWithValue("@MangaId", prestamo.MangaId);
        command.Parameters.AddWithValue("@MangaNombre", prestamo.MangaNombre);
        command.Parameters.AddWithValue("@FechaPrestamo", prestamo.FechaPrestamo);
        command.Parameters.AddWithValue("@PrestadoA", prestamo.PrestadoA);

        int rows = await command.ExecuteNonQueryAsync();
        return rows > 0;
    }

    public async Task<List<Prestamo>> GetAllAsync()
    {
        var prestamos = new List<Prestamo>();
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = new MySqlCommand("SELECT * FROM Prestamo", connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            prestamos.Add(new Prestamo
            {
                Id = reader.GetInt32("Id"),
                MangaId = reader.GetInt32("MangaId"),
                MangaNombre = reader.GetString("MangaNombre"),
                FechaPrestamo = reader.GetDateTime("FechaPrestamo"),
                PrestadoA = reader.GetString("PrestadoA")
            });
        }
        return prestamos;
    }
}
