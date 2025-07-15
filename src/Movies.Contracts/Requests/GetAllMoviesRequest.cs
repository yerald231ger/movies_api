using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Movies.Contracts.Requests;

public class GetAllMoviesRequest
{
    public string? Title { get; set; }
    public int? Year { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    
    public static ValueTask<GetAllMoviesRequest> BindAsync(HttpContext httpContext, 
        ParameterInfo parameter)
    {
        var request = httpContext.Request;
        var options = new GetAllMoviesRequest
        {
            Title = request.Query["title"].ToString(),
            Year = int.TryParse(request.Query["year"], out var year) ? year : null,
            SortBy = request.Query["sortBy"].ToString(),
            Page = int.TryParse(request.Query["page"], out var page) ? page : 1,
            PageSize = int.TryParse(request.Query["pageSize"], out var pageSize) ? pageSize : 25
        };
        return ValueTask.FromResult(options);
    }
}