using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Constants;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class DeleteMoviesEndpoint
{
    public static void MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.MoviesRoutes.Delete, async (Guid id, 
                IOutputCacheStore outputCacheStore,
                IMovieService repository, HttpContext context, CancellationToken cancellationToken) =>
            {
                var userId = context.User.GetUserId();
                var movie = await repository.GetByIdAsync(id, userId, cancellationToken);
                if (movie is null)
                    return Results.NotFound();

                await outputCacheStore.EvictByTagAsync(Caching.GetAllMoviesTag, cancellationToken);
                await repository.DeleteAsync(movie.Id, userId, cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteMovie")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.AdminPolicy)
            .HasApiVersion(1, 0);
    }
}