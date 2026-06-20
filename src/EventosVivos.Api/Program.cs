using EventosVivos.Api.Infrastructure;
using EventosVivos.Application;
using EventosVivos.Infrastructure;
using EventosVivos.Infrastructure.Persistence.Seed;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsSetting = builder.Configuration["Cors:AllowedOrigins"];
string[] corsOrigins = string.IsNullOrWhiteSpace(corsSetting)
    ? ["http://localhost:4200", "https://localhost:4200"]
    : corsSetting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapControllers();

if (!app.Environment.IsEnvironment("IntegrationTesting"))
{
    await DatabaseSeeder.SeedAsync(app.Services);
}

app.Run();

public partial class Program;
