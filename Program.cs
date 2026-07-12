using UrlShortener.Interfaces;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IIdGenerator, SnowFlakeIdGenerator>();
builder.Services.AddSingleton<IShortCodeGenerator, Base62CodeGenerator>();

var app = builder.Build();

var generator = app.Services.GetRequiredService<IIdGenerator>();
var shortCodeGenerator = app.Services.GetRequiredService<IShortCodeGenerator>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();