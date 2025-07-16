# Movies API SDK

This SDK provides a simple way to consume the Movies API using Refit and HttpClientFactory.

## Features

- **Refit Integration**: Type-safe HTTP client with automatic JSON serialization
- **HttpClientFactory**: Proper HttpClient management with dependency injection
- **JWT Token Management**: Automatic token refresh and authentication
- **Movies API**: Full CRUD operations for movies
- **Ratings API**: Rate movies and retrieve user ratings

## Installation

Add the SDK to your project:

```bash
dotnet add package Movies.Api.Sdk
```

## Usage

### 1. Configure Services

```csharp
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;

services.AddMoviesApiSdk("https://localhost:7001", async provider =>
{
    var tokenService = provider.GetRequiredService<TokenService>();
    return await tokenService.GetTokenAsync();
});
```

### 2. Use the API Clients

#### Movies API

```csharp
public class MovieService
{
    private readonly IMoviesApi _moviesApi;

    public MovieService(IMoviesApi moviesApi)
    {
        _moviesApi = moviesApi;
    }

    public async Task<MoviesResponse> GetMoviesAsync()
    {
        var request = new GetAllMoviesRequest
        {
            Page = 1,
            PageSize = 10
        };
        
        return await _moviesApi.GetAllMoviesAsync(request);
    }
}
```

#### Ratings API

```csharp
public class RatingService
{
    private readonly IRatingsApi _ratingsApi;

    public RatingService(IRatingsApi ratingsApi)
    {
        _ratingsApi = ratingsApi;
    }

    public async Task RateMovieAsync(Guid movieId, int rating)
    {
        var request = new RateMovieRequest { Rating = rating };
        await _ratingsApi.RateMovieAsync(movieId, request);
    }
}
```

## Authentication

The SDK uses JWT tokens for authentication. You need to provide a token provider function that returns a valid JWT token. The SDK will automatically add the token to all API requests.

## Example Client

See the `Movies.Api.Sdk.Client` project for a complete example of how to use the SDK with dependency injection and token management.