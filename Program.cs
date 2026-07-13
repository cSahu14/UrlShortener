using UrlShortener.Interfaces;
using UrlShortener.Repositories;
using UrlShortener.Services;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIdGenerator, SnowFlakeIdGenerator>();
builder.Services.AddSingleton<IShortCodeGenerator, Base62CodeGenerator>();
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});

var app = builder.Build();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("fixed");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();