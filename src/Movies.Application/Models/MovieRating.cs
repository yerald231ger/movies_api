namespace Movies.Application.Models;

public class MovieRating
{
    public Guid MovieId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public int? Rating { get; set; }
}