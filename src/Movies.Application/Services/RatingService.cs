using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating,
        CancellationToken cancellationToken = default)
    {
        if (rating is < 1 or > 5)
        {
            throw new ValidationException([
                new ValidationFailure
                {
                    PropertyName = nameof(rating),
                    ErrorMessage = "Rating must be between 1 and 5."
                }
            ]);
        }

        var movieExist = await movieRepository.ExistsAsync(movieId, cancellationToken: cancellationToken);
        if (!movieExist)
        {
            throw new ValidationException([
                new ValidationFailure
                {
                    PropertyName = nameof(movieId),
                    ErrorMessage = "Movie does not exist."
                }
            ]);
        }

        return await ratingRepository.RateMovieAsync(movieId, userId, rating, cancellationToken);
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        return ratingRepository.DeleteRatingAsync(movieId, userId, cancellationToken);
    }
}