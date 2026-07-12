namespace UrlShortener.Interfaces;

public interface IShortCodeGenerator
{
    string GenerateShortCode(long id);
}