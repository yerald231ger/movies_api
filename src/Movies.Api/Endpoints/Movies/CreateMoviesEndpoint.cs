using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Constants;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class CreateMoviesEndpoint
{
    public static void MapPostMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.MoviesRoutes.Create,
                async ([FromRoute] Guid id, [FromBody] CreateMovieRequest createMovie, 
                    IOutputCacheStore outputCacheStore,
                    IMovieService repository, HttpContext context, CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var movie = createMovie.ToMovie(id);
                    var result = await repository.CreateAsync(movie, userId, cancellationToken);
                    await outputCacheStore.EvictByTagAsync(Caching.GetAllMoviesTag, cancellationToken);
                    return result
                        ? Results.CreatedAtRoute("GetMovie", new { idOrSlug = movie.Id }, movie.ToResponse())
                        : Results.BadRequest();
                }).WithName("CreateMovie")
            .Produces<Movie>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new movie")
            .WithDescription("Creates a new movie with the provided details.")
            .WithTags("Movies")
            .RequireAuthorization(AuthConstants.AdminUserPolicyName)
            .HasApiVersion(1, 0);
    }
}