using Movies.Application.Services;

namespace Movies.Api.Endpoints;

public static class GetMoviesEndpoint
{
    public static void MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(MoviesRoutes.GetById, async ([FromRoute] string idOrSlug, IMovieService repository, CancellationToken cancellationToken) =>
            {
                var movie = Guid.TryParse(idOrSlug, out var id)
                    ? await repository.GetByIdAsync(id, cancellationToken)
                    : await repository.GetBySlugAsync(idOrSlug, cancellationToken);
                return movie is null ? Results.NotFound() : Results.Ok(movie.ToResponse());
            })
            .WithName("GetMovie")
            .Produces<Movie>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(MoviesRoutes.GetAll, async (IMovieService repository, CancellationToken cancellationToken) =>
            {
                var movies = await repository.GetAllAsync(cancellationToken);
                return Results.Ok(movies.ToResponse());
            })
            .WithName("GetMovies")
            .Produces<IEnumerable<Movie>>()
            .Produces(StatusCodes.Status404NotFound);
    }
}