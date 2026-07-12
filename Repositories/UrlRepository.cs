using Dapper;
using Npgsql;
using UrlShortener.Data;
using UrlShortener.Interfaces;
using UrlShortener.Schema;
using UrlShortener.Utilities;

namespace UrlShortener.Repositories;

public class UrlRepository(IConfiguration configuration) :
    BaseRepository(configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not configured.")), IUrlRepository
{
    public async Task<bool> Save(Urls urls)
    {
        using var connection = CreateConnection();

        var query = @$"INSERT INTO {TableNames.Urls} (id, short_code, original_url, created_at, click_count) VALUES (@Id, @ShortCode, @OriginalUrl, @CreatedAt, @ClickCount) ";

        var rows = await connection.ExecuteAsync(query, urls);

        return rows > 0;
    }

    public async Task<Urls?> GetByShortCode(string shortCode)
    {
        var query = @$"SELECT * FROM {TableNames.Urls} WHERE short_code = @shortCode";

        using var connection = CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<Urls>(query, new { shortCode });
    }

    public async Task<bool> IncrementClick(string shortCode)
    {
        var query = @$"UPDATE {TableNames.Urls} SET click_count = click_count + 1 WHERE short_code = @shortCode";

        using var connection = CreateConnection();

        var rows = await connection.ExecuteAsync(query, new { shortCode });

        return rows > 0;
    }
}