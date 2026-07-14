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
        var id = idGenerator.GenerateId();
        var shortCode = shortCodeGenerator.GenerateShortCode(id);

        var newUrl = new Urls() { Id = id, ShortCode = shortCode, OriginalUrl = url };
        var result = await urlRepository.Save(newUrl);

        if (!result)
            throw new InvalidDataException("Can't generate short url.");

        try { await cacheService.SetAsync(shortCode, url, CacheExpiry); }
        catch (Exception ex) { Log.Warning("Cache unavailable: {Error}", ex.Message); }

        return shortCode;
    }

    public async Task<string?> ResolveUrl(string shortCode)
    {
        try
        {
            var cached = await cacheService.GetAsync(shortCode);
            if (cached != null)
            {
                await urlRepository.IncrementClick(shortCode);
                return cached;
            }
        }
        catch (Exception ex) { Log.Warning("Cache unavailable: {Error}", ex.Message); }

        var result = await urlRepository.GetByShortCode(shortCode);
        if (result is null) return null;

        try { await cacheService.SetAsync(shortCode, result.OriginalUrl, CacheExpiry); }
        catch (Exception ex) { Log.Warning("Cache unavailable: {Error}", ex.Message); }

        await urlRepository.IncrementClick(shortCode);
        return result.OriginalUrl;
    }
}