namespace Movies.Contracts;

public abstract class JwtParameters
{
    public const string Key = "this-is-a-secret-key-that-should-be-at-least-32-characters-long";
    public const string Issuer = "MoviesIssuer";
    public const string Audience = "MoviesAudience";
}