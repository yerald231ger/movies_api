namespace Movies.Api.Auth;

public abstract class AuthConstants
{
    public const string AdminPolicy = "AdminOnly";
    public const string UserPolicy = "UserOnly";
    public const string ApiKeyHeaderName = "x-api-key";
    public const string ApiKeyHeaderValue = "05b269aaed614075a50e5e4a7a20645f"; // Example value, should be stored securely
    public const string AdminUserPolicyName = "AdminUserPolicyName";
}