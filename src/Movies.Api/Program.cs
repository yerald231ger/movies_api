using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Application;
using Movies.Application.Data;
using Movies.Contracts;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MovieDb");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'MovieDb' is not configured.");

builder.Services.AddApplication();
builder.Services.AddData(connectionString);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
        System.Text.Encoding.UTF8.GetBytes(JwtKey.Value)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
    };
});

var app = builder.Build();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapPostMovie();
app.MapGetMovie();
app.MapPutMovie();
app.MapDeleteMovie();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync(connectionString);

app.Run();