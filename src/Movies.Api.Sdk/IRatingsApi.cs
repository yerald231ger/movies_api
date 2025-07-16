using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

[Headers("Authorization: Bearer")]
public interface IRatingsApi
{
    [Post("/api/movies/{movieId}/ratings")]
    Task<MovieRatingResponse> RateMovieAsync(Guid movieId, [Body] RateMovieRequest request, 
        CancellationToken cancellationToken = default);

    [Delete("/api/ratings/{movieId}")]
    Task DeleteRatingAsync(Guid movieId, CancellationToken cancellationToken = default);

    [Get("/api/ratings/me")]
    Task<IEnumerable<MovieRatingResponse>> GetUserRatingsAsync(
        CancellationToken cancellationToken = default);
}