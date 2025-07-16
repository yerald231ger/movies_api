using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Movies.Api.Auth;
using Movies.Api.Constants;
using Movies.Api.Endpoints.Movies;
using Movies.Api.Endpoints.Ratings;
using Movies.Api.Health;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Data;
using Movies.Contracts;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(JwtParameters.Key)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = JwtParameters.Issuer,
        ValidAudience = JwtParameters.Audience,
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy(AuthConstants.AdminUserPolicyName,
        p => p.AddRequirements(new AdminAuthRequirement()));
    x.AddPolicy(AuthConstants.AdminPolicy, p => p.RequireClaim(ClaimTypes.Role, "Admin"));
    x.AddPolicy(AuthConstants.UserPolicy, p => p.RequireClaim(ClaimTypes.Role, "User"));
});

builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1, 0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new HeaderApiVersionReader("x-ms-version");
}).AddMvc().AddApiExplorer();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

var connectionString = config.GetConnectionString("MovieDb");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Connection string 'MovieDb' is not configured.");

builder.Services.AddApplication();
builder.Services.AddData(connectionString);

builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(cachePolicyBuilder => cachePolicyBuilder.Cache());
    options.AddPolicy(Caching.MovieCachePolicy, cachePolicyBuilder =>
        cachePolicyBuilder
            .Cache()
            .Expire(TimeSpan.FromMinutes(1))
            .SetVaryByQuery(["title", "year", "sortBy", "page", "pageSize"])
            .Tag(Caching.GetAllMoviesTag)
    );
});

// APP Configuration
var app = builder.Build();

app.MapHealthChecks("_health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.UseOutputCache();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

app.UseMiddleware<ValidationMappingMiddleware>();

var apiVersionSet = app.NewVersionedApi();

apiVersionSet.MapPostMovie();
apiVersionSet.MapGetMovie();
apiVersionSet.MapPutMovie();
apiVersionSet.MapDeleteMovie();
apiVersionSet.MapRateMovie();
apiVersionSet.MapDeleteRating();
apiVersionSet.MapGetUserRatings();


var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync(connectionString);

app.Run();