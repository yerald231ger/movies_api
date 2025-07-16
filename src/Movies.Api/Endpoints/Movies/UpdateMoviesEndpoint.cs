using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Constants;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class UpdateMoviesEndpoint
{
    public static void MapPutMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiRoutes.MoviesRoutes.Update,
                async ([FromRoute] Guid id, [FromBody] UpdateMovieRequest updateMovie, 
                    IOutputCacheStore outputCacheStore,
                    IMovieService repository, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var movie = updateMovie.ToMovie(id);
                    var result = await repository.UpdateAsync(movie, userId, cancellationToken);
                    await outputCacheStore.EvictByTagAsync(Caching.GetAllMoviesTag, cancellationToken);
                    return result != null
                        ? Results.Ok(movie.ToResponse())
                        : Results.NotFound();
                })
            .WithName("UpdateMovie")
            .Produces<Movie>()
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Updates an existing movie")
            .WithDescription("Updates an existing movie with the provided details.")
            .WithTags("Movies")
            .RequireAuthorization(AuthConstants.AdminPolicy)
            .HasApiVersion(1, 0);
    }
}