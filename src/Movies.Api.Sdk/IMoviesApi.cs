using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

[Headers("Authorization: Bearer")]
public interface IMoviesApi
{
    [Get("/api/movies")]
    Task<MoviesResponse> GetAllMoviesAsync([Query] GetAllMoviesRequest request, 
        CancellationToken cancellationToken = default);

    [Get("/api/movies/{idOrSlug}")]
    Task<MovieResponse> GetMovieAsync(string idOrSlug, 
        CancellationToken cancellationToken = default);

    [Post("/api/movies/{id}")]
    Task<MovieResponse> CreateMovieAsync(Guid id, [Body] CreateMovieRequest request, 
        CancellationToken cancellationToken = default);

    [Put("/api/movies/{id}")]
    Task<MovieResponse> UpdateMovieAsync(Guid id, [Body] UpdateMovieRequest request, 
        CancellationToken cancellationToken = default);

    [Delete("/api/movies/{id}")]
    Task DeleteMovieAsync(Guid id, CancellationToken cancellationToken = default);
}