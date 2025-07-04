namespace Movies.Api.Endpoints;

public static class GetMoviesEndpoint
{
    public static void MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(MoviesRoutes.GetById, async (Guid id, IMovieRepository repository) =>
            {
                var movie = await repository.GetByIdAsync(id);
                return movie is null ? Results.NotFound() : Results.Ok(movie.ToResponse());
            })
            .WithName("GetMovie")
            .Produces<Movie>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(MoviesRoutes.GetAll, async (IMovieRepository repository) =>
            {
                var movies = await repository.GetAllAsync();
                return Results.Ok(movies.ToResponse());
            })
            .WithName("GetMovies")
            .Produces<IEnumerable<Movie>>()
            .Produces(StatusCodes.Status404NotFound);
    }
}