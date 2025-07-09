using Movies.Application;
using Movies.Application.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MovieDb");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'MovieDb' is not configured.");

builder.Services.AddApplication();
builder.Services.AddData(connectionString);

var app = builder.Build();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapPostMovie();
app.MapGetMovie();
app.MapPutMovie();
app.MapDeleteMovie();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync(connectionString);

app.Run();


