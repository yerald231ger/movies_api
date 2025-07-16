using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.Api.Auth;

public class ApiKeyAuthFilter(IConfiguration configuration) : IEndpointFilter
{
    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName,
                out var extractedApiKey))
        {
            return ValueTask.FromResult<object?>(new UnauthorizedObjectResult("API Key missing"));
        }

        var apiKey = AuthConstants.ApiKeyHeaderValue;
        if (apiKey != extractedApiKey)
        {
            return ValueTask.FromResult<object?>(new UnauthorizedObjectResult("Invalid API Key"));
        }

        return next(context);
    }
}


