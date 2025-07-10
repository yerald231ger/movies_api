using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(
    IMovieRepository movieRepository,
    IValidator<Movie> movieValidator,
    IRatingRepository ratingRepository
    ) : IMovieService
{
    public async Task<bool> CreateAsync(Movie movie, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await movieRepository.CreateAsync(movie, userId, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return movieRepository.GetByIdAsync(id, userId, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return movieRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return movieRepository.GetAllAsync(userId, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        var existingMovie = await movieRepository.ExistsAsync(movie.Id, cancellationToken);

        if (!existingMovie)
            return null;

        await movieRepository.UpdateAsync(movie, cancellationToken);
        var updatedMovie = await movieRepository.GetByIdAsync(movie.Id, userId, cancellationToken);
        
        if (userId.HasValue)
        {
            var rating = await ratingRepository.GetRatingAsync(movie.Id, cancellationToken);
            movie.Rating = rating ?? 0;
            return updatedMovie;
        }
        
        var ratings = await ratingRepository.GetRatingAsync(movie.Id, userId!.Value, cancellationToken);
        updatedMovie!.Rating = ratings.Rating;
        updatedMovie.UserRating = ratings.UserRating;
        
        return updatedMovie;
    }

    public Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        return movieRepository.DeleteAsync(id, userId, cancellationToken);
    }
}