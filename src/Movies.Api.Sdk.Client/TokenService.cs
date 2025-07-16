using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Movies.Api.Sdk.Client;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string? _currentToken;
    private DateTime _tokenExpiry;

    public TokenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetTokenAsync()
    {
        if (IsTokenValid())
        {
            return _currentToken!;
        }

        return await RefreshTokenAsync();
    }

    private bool IsTokenValid()
    {
        if (string.IsNullOrEmpty(_currentToken))
            return false;

        return DateTime.UtcNow < _tokenExpiry.AddMinutes(-5);
    }

    private async Task<string> RefreshTokenAsync()
    {
        var tokenEndpoint = _configuration["ApiSettings:TokenEndpoint"];
        var credentials = new
        {
            Username = _configuration["ApiSettings:Username"],
            Password = _configuration["ApiSettings:Password"]
        };

        var response = await _httpClient.PostAsJsonAsync(tokenEndpoint, credentials);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        _currentToken = tokenResponse!.Token;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(_currentToken);
        _tokenExpiry = jwt.ValidTo;

        return _currentToken;
    }

    private record TokenResponse(string Token);
}