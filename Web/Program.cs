using Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Presentation.Controllers;
using Services.Abstractions;
using Services.Profiles;
using Services.Services;
using System.Text;
using Microsoft.OpenApi.Models;
using Domain.Entities.Enums;
using Web.Middlewares;
using Persistence.Repositories;
using Persistance;
using Web;
using Services.Validators;
using FluentValidation.AspNetCore;
using Services.Validators.TaskValidators;
using LikhodedDynamics.Sber.GigaChatSDK;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database");
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<TaskValidatorForCreate>();
    }); ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Web", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Введите токен с префиксом 'Bearer' в формате 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"} }, new string[] { } }
    });
});
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IValidatorManager, ValidatorManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddDbContextPool<RepositoryDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
builder.Services.AddAutoMapper(typeof(Services.AssemblyReference));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddSingleton<GigaChat>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new GigaChat(
        configuration["ChatBotSettings:ApiKey"],
        isCommercial: false,
        ignoreTLS: true,
        saveImage: false
    );
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await SeedDataAsync(app.Services);

app.Run();


static async Task SeedDataAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
{
    using var scope = serviceProvider.CreateScope();
    var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

    // Инициализация специализации и администратора
    await serviceManager.SpecializationService.SeedSpecializationUserAsync(cancellationToken);
    await serviceManager.UserService.SeedAdminUserAsync(cancellationToken);
}