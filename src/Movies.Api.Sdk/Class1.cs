using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Movies.Api.Sdk;

public static class SdkServiceCollectionExtensions
{
    public static IServiceCollection AddMoviesApiSdk(this IServiceCollection services, 
        string baseUrl, 
        Func<IServiceProvider, Task<string>> tokenProvider)
    {
        services.AddRefitClient<IMoviesApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler(provider => new AuthenticationMessageHandler(tokenProvider, provider));

        services.AddRefitClient<IRatingsApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddHttpMessageHandler(provider => new AuthenticationMessageHandler(tokenProvider, provider));

        return services;
    }
}