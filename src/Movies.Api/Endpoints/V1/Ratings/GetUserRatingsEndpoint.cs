using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.V1.Ratings;

public static class GetUserRatingsEndpoint
{
    public static void MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.V1.RatingsRoutes.GetUserRating, async (IRatingService ratingService, HttpContext context, CancellationToken cancellationToken) =>
            {
                var userId = context.User.GetUserId();
                if (userId == null)
                    return Results.Unauthorized();

                var ratings = await ratingService.GetRatingForUserAsync(userId.Value, cancellationToken);
                var ratingsResponse = ratings.ToResponse();
                return Results.Ok(ratingsResponse);
            })
            .WithName("GetUserRating")
            .Produces<MovieRatingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthConstants.UserPolicy);
    }
}