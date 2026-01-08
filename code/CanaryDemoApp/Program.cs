using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Canary Demo API",
        Version = "v1"
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Canary Demo API v1");
    });
}

// Health endpoint (used in pipeline)
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow
}));

// Sample API
app.MapGet("/api/message", () =>
{
    return Results.Ok("Hello from Canary Deployment 🚀");
});

app.Run();
