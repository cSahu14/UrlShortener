using Microsoft.AspNetCore.Mvc;
using UrlShortener.Interfaces;

namespace UrlShortener.Controllers;


[ApiController]
[Route("api/v1")]
public class UrlController(IUrlShortenerService urlShortenerService) : ControllerBase
{
    [HttpPost]
    [Route("short_url")]
    public async Task<IActionResult> ShortenUrl([FromBody] string url)
    {
        Console.WriteLine($"url {url}");
        try
        {
            var result = await urlShortenerService.ShortenUrl(url);

            return Ok(result);
        }
        catch (System.Exception e)
        {
            return StatusCode(500, $"Something went wrong {e}");
        }
    }

    [HttpGet]
    [Route("{short_code}")]
    public async Task<IActionResult> RedirectUrl(string short_code)
    {
        try
        {
            var result = await urlShortenerService.ResolveUrl(short_code);

            if (result is null)
            {
                return NotFound("Not found.");
            }

            return Redirect(result);
        }
        catch (System.Exception)
        {

            return StatusCode(500, "Something went wrong");
        }
    }
}