namespace Movies.Api.Endpoints;

public static class DeleteMoviesEndpoint
{
    public static void MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(MoviesRoutes.GetById, async (Guid id, IMovieRepository repository) =>
            {
                var movie = await repository.GetByIdAsync(id);
                if (movie is null)
                    return Results.NotFound();

                await repository.DeleteAsync(movie.Id);
                return Results.NoContent();
            })
            .WithName("DeleteMovie")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }
}