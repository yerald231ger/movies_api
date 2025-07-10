namespace Movies.Contracts.Responses;

public record MovieResponse(
    Guid Id,
    string Title,
    short YearOfRelease,
    IEnumerable<string> Genre,
    string Slug,
    float? Rating,
    int? UserRating);