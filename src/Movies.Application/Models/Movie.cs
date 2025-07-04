namespace Movies.Application.Models;

public class Movie
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required short YearOfRelease { get; set; }
    public required List<string> Genre { get; set; } = [];
    
    public void ChangeTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(newTitle));
        }
        Title = newTitle;
    }
}