using Microsoft.AspNetCore.Mvc;

namespace Movies.Api.Endpoints;

public static class CreateMoviesEndpoint
{
    public static void MapPostMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(MoviesRoutes.Create,
                async ([FromRoute] Guid id, [FromBody] CreateMovieRequest createMovie, IMovieRepository repository) =>
                {
                    var movie = createMovie.ToMovie(id);
                    
                    var result = await repository.CreateAsync(movie);
                    return result
                        ? Results.CreatedAtRoute("GetMovie", new { id = movie.Id }, movie.ToResponse())
                        : Results.BadRequest();
                }).WithName("CreateMovie")
            .Produces<Movie>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new movie")
            .WithDescription("Creates a new movie with the provided details.")
            .WithTags("Movies");
    }
}