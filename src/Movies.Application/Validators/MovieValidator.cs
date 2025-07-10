using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;

    public MovieValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;

        RuleFor(movie => movie.Id)
            .NotEmpty()
            .WithMessage("Movie ID is required.");

        RuleFor(movie => movie.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title must not exceed 100 characters.");

        RuleFor(movie => movie.Genres)
            .NotEmpty()
            .WithMessage("At least one genre is required.")
            .Must(genres => genres.Count <= 5)
            .WithMessage("A maximum of 5 genres is allowed.");

        RuleFor(movie => movie.YearOfRelease)
            .LessThanOrEqualTo((short)DateTime.Now.Year);

        RuleFor(movie => movie.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("Slug must be unique and not empty.");
    }

    private async Task<bool> ValidateSlug(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        var movie = await _movieRepository.GetBySlugAsync(slug, cancellationToken: cancellationToken);

        if (movie is null)
            return true;

        return movie.Slug != slug;
    }
}