using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class GetMoviesEndpoint
{
    public static void MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(MoviesRoutes.GetById,
                async ([FromRoute] string idOrSlug, IMovieService repository, HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var movie = Guid.TryParse(idOrSlug, out var id)
                        ? await repository.GetByIdAsync(id, userId, cancellationToken)
                        : await repository.GetBySlugAsync(idOrSlug, userId, cancellationToken);
                    return movie is null ? Results.NotFound() : Results.Ok(movie.ToResponse());
                })
            .WithName("GetMovie")
            .Produces<Movie>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(MoviesRoutes.GetAll,
                async ([FromQuery] GetAllMoviesRequest request, IMovieService repository, HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var options = request.ToOptions()
                        .WithUser(userId);
                    var movies = await repository.GetAllAsync(userId, cancellationToken);
                    return Results.Ok(movies.ToResponse());
                })
            .WithName("GetMovies")
            .Produces<IEnumerable<Movie>>()
            .Produces(StatusCodes.Status404NotFound);
    }
}