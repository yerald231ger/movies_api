using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Endpoints;

public static class UpdateMoviesEndpoint
{
    public static void MapPutMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(MoviesRoutes.Update,
                async ([FromRoute] Guid id, [FromBody] UpdateMovieRequest updateMovie, IMovieRepository repository) =>
                {
                    var movie = updateMovie.ToMovie(id);
                    var result = await repository.UpdateAsync(movie);
                    return result
                        ? Results.Ok(movie.ToResponse())
                        : Results.NotFound();
                })
            .WithName("UpdateMovie")
            .Produces<Movie>()
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Updates an existing movie")
            .WithDescription("Updates an existing movie with the provided details.")
            .WithTags("Movies");
    }
}