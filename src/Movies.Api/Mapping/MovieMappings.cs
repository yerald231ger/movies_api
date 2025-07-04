using Movies.Application.Models;
using Movies.Contracts.Requests;

namespace Movies.Api.Mapping;

public static class MovieMappings
{
    public static Movie ToMovie(this CreateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genre = request.Genre.ToList()
        };
    }

    public static Movie ToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genre = request.Genre.ToList()
        };
    }

    public static MovieResponse ToResponse(this Movie movie)
    {
        return new MovieResponse(
            movie.Id,
            movie.Title,
            movie.YearOfRelease,
            movie.Genre
        );
    }

    public static MoviesResponse ToResponse(this IEnumerable<Movie> movies)
    {
        var e = new MoviesResponse(movies.Select(ToResponse));
        return e;
    }
}