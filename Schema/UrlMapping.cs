namespace UrlShortener.Schema;

public class UrlMapping
{
    public long Id { get; init; }
    public string ShortCode { get; init; } = string.Empty;
    public required string OriginalUrl { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public int ClickCount { get; set; } = 0;
}