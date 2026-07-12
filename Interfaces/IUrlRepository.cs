using UrlShortener.Schema;

namespace UrlShortener.Interfaces;

public interface IUrlRepository
{
    Task<bool> Save(Urls Urls);
    Task<Urls?> GetByShortCode(string shortCode);
    Task<bool> IncrementClick(string shortCode);
}
