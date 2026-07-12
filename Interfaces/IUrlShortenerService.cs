namespace UrlShortener.Interfaces;

public interface IUrlShortenerService
{
    Task<string> ShortenUrl(string url);
    Task<string?> ResolveUrl(string shortCode);
}