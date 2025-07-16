using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Movies.Api.Sdk;

public class AuthenticationMessageHandler : DelegatingHandler
{
    private readonly Func<IServiceProvider, Task<string>> _tokenProvider;
    private readonly IServiceProvider _serviceProvider;

    public AuthenticationMessageHandler(
        Func<IServiceProvider, Task<string>> tokenProvider,
        IServiceProvider serviceProvider)
    {
        _tokenProvider = tokenProvider;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var token = await _tokenProvider(_serviceProvider);
        
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}