using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Movies.Contracts;
using Movies.Auth.Options;
using Movies.Auth.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddOpenApi();

builder.Services.Configure<UserOptions>(builder.Configuration);
builder.Services.AddScoped<IUserRepository, JsonUserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/token", async (TokenRequest request, IUserRepository userRepository) =>
    {
        if (await userRepository.ValidateCredentialsAsync(request.Username, request.Password))
        {
            var user = await userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JwtParameters.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                ]),
                Issuer = JwtParameters.Issuer,
                Audience = JwtParameters.Audience,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
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