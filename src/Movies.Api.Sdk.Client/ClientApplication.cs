using Microsoft.Extensions.Hosting;
using Movies.Api.Sdk;
using Movies.Contracts.Requests;

namespace Movies.Api.Sdk.Client;

public class ClientApplication : BackgroundService
{
    private readonly IMoviesApi _moviesApi;
    private readonly IRatingsApi _ratingsApi;
    private readonly IHostApplicationLifetime _lifetime;

    public ClientApplication(
        IMoviesApi moviesApi,
        IRatingsApi ratingsApi,
        IHostApplicationLifetime lifetime)
    {
        _moviesApi = moviesApi;
        _ratingsApi = ratingsApi;
        _lifetime = lifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine("Starting Movies API SDK Client demonstration...");

            await DemonstrateMoviesApi();
            await DemonstrateRatingsApi();

            Console.WriteLine("Demonstration completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            _lifetime.StopApplication();
        }
    }

    private async Task DemonstrateMoviesApi()
    {
        Console.WriteLine("\n=== Movies API Demonstration ===");

        try
        {
            var getAllRequest = new GetAllMoviesRequest
            {
                Page = 1,
                PageSize = 10
            };

            var movies = await _moviesApi.GetAllMoviesAsync(getAllRequest);
            Console.WriteLine($"Retrieved {movies.Items.Count()} movies");

            if (movies.Items.Any())
            {
                var firstMovie = movies.Items.First();
                Console.WriteLine($"First movie: {firstMovie.Title} ({firstMovie.YearOfRelease})");

                var movieDetails = await _moviesApi.GetMovieAsync(firstMovie.Id.ToString());
                Console.WriteLine($"Movie details: {movieDetails.Title} - Rating: {movieDetails.Rating}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in movies API: {ex.Message}");
        }
    }

    private async Task DemonstrateRatingsApi()
    {
        Console.WriteLine("\n=== Ratings API Demonstration ===");

        try
        {
            var userRatings = await _ratingsApi.GetUserRatingsAsync();
            Console.WriteLine($"User has {userRatings.Count()} ratings");

            foreach (var rating in userRatings.Take(3))
            {
                Console.WriteLine($"Rated movie ID {rating.MovieId} with {rating.Rating} stars");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in ratings API: {ex.Message}");
        }
    }
}