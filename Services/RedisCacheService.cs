using StackExchange.Redis;
using UrlShortener.Interfaces;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConfiguration configuration)
    {
        var connectionString = configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException("Redis connection string not configured.");

        var config = ConfigurationOptions.Parse(connectionString);
        config.AbortOnConnectFail = false;
        config.Ssl = true;
        config.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        var redis = ConnectionMultiplexer.Connect(config);
        _db = redis.GetDatabase();

    }

    public async Task<string?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }

    public async Task SetAsync(string key, string value, TimeSpan expiry)
    {
        await _db.StringSetAsync(key, value, expiry);
    }

    public async Task DeleteAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}