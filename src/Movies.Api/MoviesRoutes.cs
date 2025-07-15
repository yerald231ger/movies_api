namespace Movies.Api;

public static class ApiRoutes
{
    public const string ApiBase = "api";

    public static class V1
    {
        public const string VersionBase = $"{ApiBase}/V1";

        internal static class MoviesRoutes
        {
            public const string Base = $"{VersionBase}/movies";
            public const string GetAll = Base;
            public const string Create = $"{Base}/{{id:guid}}";
            public const string GetById = $"{Base}/{{idOrSlug}}";
            public const string Update = $"{Base}/{{id:guid}}";
            public const string Delete = $"{Base}/{{id:guid}}";

        }

        internal static class RatingsRoutes
        {
            public const string Base = $"{VersionBase}/ratings";
            public const string Rate = $"{MoviesRoutes.Base}/{{movieId:guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{movieId:guid}}";
            public const string GetUserRating = $"{Base}/me";
        }
    }

    public static class V2
    {
        public const string VersionBase = $"{ApiBase}/v2";

        internal static class MoviesRoutes
        {
            public const string Base = $"{VersionBase}/movies";
            public const string GetAll = Base;
            public const string GetById = $"{Base}/{{idOrSlug}}";
        }
    }
}

