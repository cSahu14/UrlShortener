using UrlShortener.Interfaces;
using UrlShortener.Schema;

namespace UrlShortener.Services;

public class UrlShortenerService(
   IIdGenerator idGenerator, IShortCodeGenerator shortCodeGenerator, IUrlRepository urlRepository
) : IUrlShortenerService
{
    public async Task<string> ShortenUrl(string url)
    {
        Console.WriteLine($"url {url}");
        var id = idGenerator.GenerateId();
        var shortCode = shortCodeGenerator.GenerateShortCode(id);

        var newUrl = new Urls()
        {
            Id = id,
            ShortCode = shortCode,
            OriginalUrl = url
        };

        var result = await urlRepository.Save(newUrl);

        if (!result)
            throw new InvalidDataException("Can't generate short url.");

        return shortCode;
    }

    public async Task<string?> ResolveUrl(string shortCode)
    {
        var result = await urlRepository.GetByShortCode(shortCode);

        if (result is null)
            return null;

        await urlRepository.IncrementClick(shortCode);

        return result.OriginalUrl;
    }
}