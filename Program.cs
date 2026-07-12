using UrlShortener.Interfaces;
using UrlShortener.Repositories;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIdGenerator, SnowFlakeIdGenerator>();
builder.Services.AddSingleton<IShortCodeGenerator, Base62CodeGenerator>();
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();