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
            movie.Slug ?? string.Empty
        );
    }

    public static MoviesResponse ToResponse(this IEnumerable<Movie> movies)
    {
        var e = new MoviesResponse(movies.Select(ToResponse));
        return e;
    }
}