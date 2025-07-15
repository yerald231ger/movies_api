namespace Movies.Application.Models;

public class GetAllMoviesOptions
{
    public string? Title { get; set; }
    public int? Year { get; set; } 
    public Guid? UserId { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}