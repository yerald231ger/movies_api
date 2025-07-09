using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/token", (TokenRequest request) =>
{
    if (request.Username == "admin" && request.Password == "password")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = "this-is-a-secret-key-that-should-be-at-least-32-characters-long"u8.ToArray();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Results.Ok(new TokenResponse(tokenString));
    }

    return Results.Unauthorized();
})
.WithName("GenerateToken");

app.Run();

internal record TokenRequest(string Username, string Password);

internal record TokenResponse(string Token);
