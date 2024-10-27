using System.Text.Json.Serialization;
using HITS_API_1.Application.Interfaces;
using HITS_API_1.Application.Services;
using HITS_API_1.Domain;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Authentication;
using HITS_API_1.Infrastructure.Data;
using HITS_API_1.Infrastructure.Repositories;
using HITS_API_1.Middlewares;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Подключение БД
builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

// Подключение контроллеров
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IDoctorsRepository, DoctorsRepository>();
builder.Services.AddScoped<IDoctorsService, DoctorsService>();
builder.Services.AddScoped<ITokensRepository, TokensRepository>();
builder.Services.AddScoped<ITokensService, TokensService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication()
    .AddBearerToken();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseMiddleware<TokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Инициализация и проверка БД
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        DbInitializer.Initialize(dbContext);

        // Проверка, получилось ли подклюиться к БД
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("Подключение к БД установлено");
        }
        else
        {
            Console.WriteLine("Не удалось подключиться к БД");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка подключения к БД: {ex.Message}");
    }
}

app.Run();