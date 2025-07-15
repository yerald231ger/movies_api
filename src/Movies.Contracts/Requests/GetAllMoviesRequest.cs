using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Movies.Contracts.Requests;

public class GetAllMoviesRequest
{
    public string? Title { get; set; }
    public int? Year { get; set; }
    
    public static ValueTask<GetAllMoviesRequest> BindAsync(HttpContext httpContext, 
        ParameterInfo parameter)
    {
        var request = httpContext.Request;
        var options = new GetAllMoviesRequest
        {
            Title = request.Query["title"].ToString(),
            Year = int.TryParse(request.Query["year"], out var year) ? year : null
        };
        return ValueTask.FromResult(options);
    }
}