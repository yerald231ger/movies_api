namespace Movies.Contracts.Responses;

public record PagedResponse<TResponse>
{
    public IEnumerable<TResponse> Items { get; set; } = [];
}