using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    public static void MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiRoutes.RatingsRoutes.DeleteRating, async ([FromRoute] Guid movieId, IRatingService ratingService,
                HttpContext context, CancellationToken cancellationToken) =>
            {
                var userId = context.User.GetUserId();
                if (userId == null)
                    return Results.Unauthorized();

                var success = await ratingService.DeleteRatingAsync(movieId, userId.Value, cancellationToken);
                return !success ? Results.NotFound() : Results.Ok();
            })
            .WithName("DeleteRating")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.UserPolicy);
    }
}