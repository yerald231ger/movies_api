namespace Movies.Contracts.Responses;

public record MovieRatingResponse(Guid MovieId, string Slug, int? Rating);