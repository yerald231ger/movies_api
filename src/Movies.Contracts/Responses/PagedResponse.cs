namespace Movies.Contracts.Responses;

public record PagedResponse<TResponse>
{
    public IEnumerable<TResponse> Items { get; set; } = [];
    public int PageSize { get; set; }
    public int Page { get; set; }
    public int Total { get; set; }
    public bool HasNextPage { get; set; }
}