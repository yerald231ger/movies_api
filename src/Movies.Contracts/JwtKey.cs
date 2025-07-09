namespace Movies.Contracts;

public class JwtKey
{
    public const string Value = "this-is-a-secret-key-that-should-be-at-least-32-characters-long";
    public const string Issuer = "MoviesIssuer";
    public const string Audience = "MoviesAudience";
}