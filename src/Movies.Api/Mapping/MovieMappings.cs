using Movies.Application.Models;
using Movies.Contracts.Requests;

namespace Movies.Api.Mapping;

public static class MovieMappings
{
    public static Movie ToMovie(this CreateMovieRequest request, Guid id)
    {
        return Movie.Create(
            id,
            request.Title,
            request.YearOfRelease,
            request.Genres.ToList()
        );
    }

    public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
    {
        return Movie.Create(
            id,
            request.Title,
            request.YearOfRelease,
            request.Genre.ToList()
        );
    }

    public static MovieResponse ToResponse(this Movie movie)
    {
        return new MovieResponse(
            movie.Id,
            movie.Title,
            movie.YearOfRelease,
            movie.Genres,
            movie.Slug,
            movie.Rating,
            movie.UserRating
        );
    }

    public static MoviesResponse ToResponse(this IEnumerable<Movie> movies)
    {
        var e = new MoviesResponse(movies.Select(ToResponse));
        return e;
    }

    public static IEnumerable<MovieRatingResponse> ToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(rating => new MovieRatingResponse(rating.MovieId, rating.Slug, rating.Rating));
    }
    
    public static GetAllMoviesOptions ToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOptions
        { 
            Title = request.Title,
            Year = request.Year,
            SortBy = request.SortBy,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
    
    public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
    {
        options.UserId = userId;
        return options;
    }
}