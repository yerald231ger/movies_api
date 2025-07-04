namespace Movies.Contracts.Requests;

public record UpdateMovieRequest(
    string Title,
    short YearOfRelease,
    IEnumerable<string> Genre
);