using System.Text.Json.Serialization;

namespace Movies.Contracts.Responses;

public record HalResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Link> Links { get; } = [];
}

public record Link(string Rel, string Href, string? Type = null);