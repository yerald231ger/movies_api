namespace Movies.Api;

public static class ApiRoutes
{
    public const string ApiBase = "api";    
   
    internal static class MoviesRoutes
    {
        public const string Base = $"{ApiBase}/movies";
        public const string GetAll = Base;
        public const string Create = $"{Base}/{{id:guid}}";
        public const string GetById = $"{Base}/{{idOrSlug}}";
        public const string Update = $"{Base}/{{id:guid}}";
        public const string Delete = $"{Base}/{{id:guid}}";
    }

    internal static class RatingsRoutes
    {
        public const string Base = $"{ApiBase}/ratings";
        public const string Rate = $"{MoviesRoutes.Base}/{{movieId:guid}}/ratings";
        public const string DeleteRating = $"{Base}/{{movieId:guid}}";
        public const string GetUserRating = $"{Base}/me";
    }
}