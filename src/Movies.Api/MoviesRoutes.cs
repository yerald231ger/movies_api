namespace Movies.Api;

internal static class MoviesRoutes
{
    public const string Api = "api";
    public const string Base = $"{Api}/movies";
    public const string GetAll = Base;
    public const string Create = $"{Base}/{{id:guid}}";
    public const string GetById = $"{Base}/{{idOrSlug}}";
    public const string Update = $"{Base}/{{id:guid}}";
    public const string Delete = $"{Base}/{{id:guid}}";
    
    public const string Rate = $"{Base}/{{id:guid}}/ratings";
}

internal static class RatingsRoutes
{
    public const string Base = $"{MoviesRoutes.Api}/ratings";
    public const string GetUserRating = $"{Base}/me";
}