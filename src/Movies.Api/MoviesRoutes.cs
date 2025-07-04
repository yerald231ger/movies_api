namespace Movies.Api;

internal static class MoviesRoutes
{
    public const string Api = "api";
    public const string Base = $"{Api}/movies";
    public const string GetAll = Base;
    public const string Create = $"{Base}/{{id:guid}}";
    public const string GetById = $"{Base}/{{id:guid}}";
    public const string Update = $"{Base}/{{id:guid}}";
}