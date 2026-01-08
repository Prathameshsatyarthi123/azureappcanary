var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Read version & slot info from environment variables
// (These can be set in App Service slot configuration)
var appVersion = Environment.GetEnvironmentVariable("APP_VERSION") ?? "v1";
var slotName = Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME") ?? "production";

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// Health endpoint for Canary validation
app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        version = appVersion,
        slot = slotName,
        timestamp = DateTime.UtcNow
    });
});

// Root endpoint – helps visually confirm Canary traffic
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        message = "Hello from Canary Deployment Demo",
        version = appVersion,
        slot = slotName
    });
});

// Existing weather forecast endpoint
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return Results.Ok(new
    {
        version = appVersion,
        slot = slotName,
        data = forecast
    });
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
