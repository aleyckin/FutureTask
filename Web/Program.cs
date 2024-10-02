using Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Persistance;
using Persistence.Repositories;
using Presentation.Controllers;
using Services.Abstractions;
using Services.Profiles;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database");

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(AdministratorController).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Web", Version = "v1" }));
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContextPool<RepositoryDbContext>(options =>
{
    options.UseNpgsql(connectionString); // Используем заранее полученную строку подключения
});
builder.Services.AddAutoMapper(typeof(AdministratorProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();