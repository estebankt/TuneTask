using Microsoft.OpenApi.Models;
using TuneTask.Core.Entities;
using TuneTask.Core.Interfaces;
using TuneTask.Core.Services;
using TuneTask.Infrastructure.Database;
using TuneTask.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is missing.");
}

// Register DatabaseContext
builder.Services.AddSingleton(new DatabaseContext(connectionString));

// Register Task Repository & Task Service
builder.Services.AddScoped<IRepository<TaskItem>, TaskRepository>();
builder.Services.AddScoped<TaskService>();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TuneTask API",
        Version = "v1",
        Description = "An AI-powered Task Manager with Spotify"
    });
});

var app = builder.Build();

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
