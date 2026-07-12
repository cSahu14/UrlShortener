using System.Text;
using UrlShortener.Interfaces;

namespace UrlShortener.Services;

public class Base62CodeGenerator : IShortCodeGenerator
{
    private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public string GenerateShortCode(long id)
    {
        var result = new StringBuilder();
        while (id > 0)
        {
            int remainder = (int)(id % 62);
            result.Append(Chars[remainder]);
            id /= 62;
        }

        return new string([.. result.ToString().Reverse()]);
    }
}