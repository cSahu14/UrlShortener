using UrlShortener.Interfaces;
using UrlShortener.Schema;
using Serilog;

namespace UrlShortener.Services;

public class UrlShortenerService(
   IIdGenerator idGenerator, IShortCodeGenerator shortCodeGenerator, IUrlRepository urlRepository, ICacheService cacheService
) : IUrlShortenerService
{
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(24);

    public async Task<string> ShortenUrl(string url)
    {
        Console.WriteLine($"url {url}");
        var id = idGenerator.GenerateId();
        var shortCode = shortCodeGenerator.GenerateShortCode(id);
        Log.Information("Generated short code: {ShortCode}", shortCode);

        var newUrl = new Urls()
        {
            Id = id,
            ShortCode = shortCode,
            OriginalUrl = url
        };

        var result = await urlRepository.Save(newUrl);

        if (!result)
            throw new InvalidDataException("Can't generate short url.");

        Log.Information("Shortening URL: {Url}", url);

        await cacheService.SetAsync(shortCode, url, CacheExpiry);

        return shortCode;
    }

    public async Task<string?> ResolveUrl(string shortCode)
    {
        var cached = await cacheService.GetAsync(shortCode);
        if (cached != null)
        {
            await urlRepository.IncrementClick(shortCode);

            Log.Information("Cache hit for short code: {ShortCode}", shortCode);

            return cached;
        }

        var result = await urlRepository.GetByShortCode(shortCode);

        if (result is null)
            return null;

        await cacheService.SetAsync(shortCode, result.OriginalUrl, CacheExpiry);

        Log.Information("Cache miss for short code: {ShortCode}", shortCode);

        await urlRepository.IncrementClick(shortCode);

        return result.OriginalUrl;
    }
}