using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Movies.Api.Sdk;
using Movies.Api.Sdk.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]!;

builder.Services.AddHttpClient<TokenService>();

builder.Services.AddMoviesApiSdk(baseUrl, async provider =>
{
    var tokenService = provider.GetRequiredService<TokenService>();
    return await tokenService.GetTokenAsync();
});

builder.Services.AddHostedService<ClientApplication>();

var app = builder.Build();

await app.RunAsync();
