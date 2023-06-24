using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var corsPolicy = "AllowAll"; // Nombre de la pol�tica CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Reemplaza con el origen de tu aplicaci�n Angular
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
    });
});

builder.Services.AddSignalR();

// Configuraci�n de Redis
var configuration = builder.Configuration;
var redisConnectionString = configuration.GetSection("Redis")["ConnectionString"];
var redisOptions = ConfigurationOptions.Parse(redisConnectionString);
var redis = ConnectionMultiplexer.Connect(redisOptions);

// Registra la instancia de ConnectionMultiplexer en el contenedor de dependencias
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);


var app = builder.Build();

app.UseRouting();

app.UseCors(corsPolicy);

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("Hub/ChatHub");
});

app.Run();
