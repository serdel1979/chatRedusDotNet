using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


// Configuración de Redis
var configuration = builder.Configuration;
var redisConnectionString = configuration.GetSection("Redis")["ConnectionString"];
var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
var redis = ConnectionMultiplexer.Connect(redisOptions);

// Registra la instancia de ConnectionMultiplexer en el contenedor de dependencias
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
