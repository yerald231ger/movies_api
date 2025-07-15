using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.V1.Ratings;

public static class RateMovieEndpoint
{
    public static void MapRateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.V1.RatingsRoutes.Rate, async ([FromRoute] Guid movieId, [FromBody] RateMovieRequest request, IRatingService ratingService,
                HttpContext context, CancellationToken cancellationToken) =>
            {
                var userId = context.User.GetUserId();
                if (userId == null)
                    return Results.Unauthorized();

                var success = await ratingService.RateMovieAsync(movieId, userId.Value, request.Rating, cancellationToken);
                return !success ? Results.NotFound() : Results.Ok();
            })
            .WithName("RateMovie")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.UserPolicy);
    }
}