using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Movies.Api.Auth;
using Movies.Api.Endpoints.Movies;
using Movies.Api.Endpoints.Ratings;
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
            System.Text.Encoding.UTF8.GetBytes(JwtParameters.Key)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = JwtParameters.Issuer,
        ValidAudience = JwtParameters.Audience,
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy(AuthConstants.AdminPolicy, p => p.RequireClaim(ClaimTypes.Role, "Admin"));
    x.AddPolicy(AuthConstants.UserPolicy, p => p.RequireClaim(ClaimTypes.Role, "User"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();

app.MapPostMovie();
app.MapGetMovie();
app.MapPutMovie();
app.MapDeleteMovie();
app.MapRateMovie();
app.MapDeleteRating();
app.MapGetUserRatings();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync(connectionString);

app.Run();