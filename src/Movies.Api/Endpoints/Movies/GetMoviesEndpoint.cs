using Movies.Api.Auth;
using Movies.Api.Constants;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Movies;

public static class GetMoviesEndpoint
{
    public static void MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.MoviesRoutes.GetById,
                async ([FromRoute] string idOrSlug, IMovieService repository, HttpContext context,
                    LinkGenerator linkGenerator,
                    CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var movie = Guid.TryParse(idOrSlug, out var id)
                        ? await repository.GetByIdAsync(id, userId, cancellationToken)
                        : await repository.GetBySlugAsync(idOrSlug, userId, cancellationToken);

                    if (movie is null)
                    {
                        return Results.NotFound();
                    }

                    var movieResponse = movie.ToResponse();

                    var getMovieUrl = linkGenerator.GetPathByName(
                        "GetMovie",
                        new { idOrSlug = movieResponse.Id });

                    var putMovieUrl = linkGenerator.GetPathByName(
                        "UpdateMovie",
                        new { id = movieResponse.Id });

                    var deleteMovieUrl = linkGenerator.GetPathByName(
                        "DeleteMovie",
                        new { id = movieResponse.Id });

                    movieResponse.Links.Add(new Link("self", getMovieUrl!, "GET"));
                    movieResponse.Links.Add(new Link("update", putMovieUrl!, "PUT"));
                    movieResponse.Links.Add(new Link("delete", deleteMovieUrl!, "DELETE"));

                    return Results.Ok(movieResponse);
                })
            .WithName("GetMovie")
            .Produces<MovieResponse>()
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet(ApiRoutes.MoviesRoutes.GetAll,
                async (GetAllMoviesRequest request, IMovieService repository, HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var options = request.ToOptions()
                        .WithUser(userId);
                    var pagedResult = await repository.GetAllAsync(options, cancellationToken);
                    return Results.Ok(pagedResult.ToPagedResponse());
                })
            .WithName("GetMovies")
            .Produces<PagedResponse<MovieResponse>>()
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(1, 0)
            .CacheOutput(Caching.MovieCachePolicy);
        

        app.MapGet(ApiRoutes.MoviesRoutes.GetAll,
                async (GetAllMoviesRequest request, IMovieService repository, HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    var userId = context.User.GetUserId();
                    var options = request.ToOptions()
                        .WithUser(userId);
                    var pagedResult = await repository.GetAllAsync(options, cancellationToken);
                    return Results.Ok(pagedResult.ToPagedResponse());
                })
            .WithName("GetMovies.v2")
            .Produces<PagedResponse<MovieResponse>>()
            .Produces(StatusCodes.Status404NotFound)
            .HasApiVersion(2, 0)
            .CacheOutput(Caching.MovieCachePolicy);
    }
}