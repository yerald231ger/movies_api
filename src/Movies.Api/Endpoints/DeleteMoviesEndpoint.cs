using Movies.Application.Services;

namespace Movies.Api.Endpoints;

public static class DeleteMoviesEndpoint
{
    public static void MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(MoviesRoutes.Delete, async (Guid id, IMovieService repository, CancellationToken cancellationToken) =>
            {
                var movie = await repository.GetByIdAsync(id, cancellationToken);
                if (movie is null)
                    return Results.NotFound();

                await repository.DeleteAsync(movie.Id, cancellationToken);
                return Results.NoContent();
            })
            .WithName("DeleteMovie")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}