using Npgsql;

namespace UrlShortener.Data;

public abstract class BaseRepository(string connectionString)
{
    private readonly string _connectionString = connectionString;

    protected NpgsqlConnection CreateConnection() => new(_connectionString);
}