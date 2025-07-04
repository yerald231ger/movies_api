namespace Movies.Contracts.Requests;

public record CreateMovieRequest(
    string Title,
    short YearOfRelease,
    IEnumerable<string> Genre
);