using System.Text.Json.Serialization;
using FluentValidation;
using HITS_API_1.Application.Interfaces;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Jobs;
using HITS_API_1.Application.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Authentication;
using HITS_API_1.Infrastructure.Data;
using HITS_API_1.Infrastructure.Repositories;
using HITS_API_1.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Quartz;

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

// Репозитории
builder.Services.AddScoped<IDoctorsRepository, DoctorsRepository>();
builder.Services.AddScoped<ITokensRepository, TokensRepository>();
builder.Services.AddScoped<ISpecialitiesRepository, SpecialitiesRepository>();
builder.Services.AddScoped<IHasher, Hasher>();
builder.Services.AddScoped<IIcd10Repository, Icd10Repository>();
builder.Services.AddScoped<IPatientsRepository, PatientsRepository>();
builder.Services.AddScoped<IInspectionsRepository, InspectionsRepository>();
builder.Services.AddScoped<IDiagnosesRepository, DiagnosesRepository>();
builder.Services.AddScoped<IConsultationsRepository, ConsultationsRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IEmailMessagesRepository, EmailMessagesRepository>();

// Сервисы
builder.Services.AddScoped<IDoctorsService, DoctorsService>();
builder.Services.AddScoped<ITokensService, TokensService>();
builder.Services.AddScoped<ISpecialitiesService, SpecialitiesService>();
builder.Services.AddScoped<IIcd10Service, Icd10Service>();
builder.Services.AddScoped<IPatientsService, PatientsService>();
builder.Services.AddScoped<IInspectionsService, InspectionsService>();
builder.Services.AddScoped<IConsultationsService, ConsultationsService>();
builder.Services.AddScoped<ICommentsService, CommentsService>();
builder.Services.AddScoped<IDiagnosesService, DiagnosesService>();
builder.Services.AddScoped<IEmailsService, EmailsService>();
builder.Services.AddScoped<IPaginationService, PaginationService>();

// Валидаторы
builder.Services.AddValidatorsFromAssemblyContaining<RegistrationRequestValidator>();

// Quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    
    var jobKey = new JobKey("SendEmailJob");
    
    q.AddJob<SendEmailJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SendEmailJob-trigger")
        .WithCronSchedule("0 * * ? * *")
    );
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Добавляем поддержку авторизации Bearer Token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Baerer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { 
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string> {} 
        }
    });
});

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
    var icd10Repository = scope.ServiceProvider.GetRequiredService<IIcd10Repository>();

    try
    {
        await DbInitializer.Initialize(dbContext, icd10Repository);

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