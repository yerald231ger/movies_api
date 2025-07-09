using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required short YearOfRelease { get; set; }
    public string Slug => GenerateSlug();
    public List<string> Genres { get; set; } = [];

    public static Movie Create(Guid id, string title, short yearOfRelease, List<string> genres)
    {
        var movie = new Movie
        {
            Id = id,
            Title = title,
            YearOfRelease = yearOfRelease,
            Genres = genres.ToList()
        };
        return movie;
    }

    public void AddGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
        {
            throw new ArgumentException("Genre cannot be null or empty.", nameof(genre));
        }
        
        if (Genres.Contains(genre))
        {
            throw new InvalidOperationException($"Genre '{genre}' already exists in the movie.");
        }

        Genres.Add(genre);
    }

    public void ChangeTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(newTitle));
        }

        GenerateSlug();
        Title = newTitle;
    }

    private string GenerateSlug()
    {
        var sluggedTitle = MyRegex().Replace(Title, string.Empty)
            .ToLower()
            .Replace(" ", "-");
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]")]
    private static partial Regex MyRegex();
}