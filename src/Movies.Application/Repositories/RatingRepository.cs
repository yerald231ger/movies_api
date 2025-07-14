using Dapper;
using Movies.Application.Data;

namespace Movies.Application.Repositories;

public class RatingRepository(IDbConnectionFactory connectionFactory) : IRatingRepository
{
    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating,
        CancellationToken cancellationToken = default)
    {
        var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        const string query = """
                             INSERT INTO Ratings (MovieId, UserId, Rating) 
                             VALUES (@MovieId, @UserId, @Rating)
                             ON CONFLICT (MovieId, UserId) 
                             DO UPDATE SET Rating = @Rating
                             """;
        var affectedRows =
            await connection.ExecuteAsync(query, new { MovieId = movieId, UserId = userId, Rating = rating });
        return affectedRows > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        const string query = """
                             SELECT ROUND(AVG(Rating), 1) 
                             FROM Ratings  
                             WHERE MovieId = @MovieId
                             """;
        return await connection.QuerySingleOrDefaultAsync<float?>(query, new { MovieId = movieId });
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        const string query = """
                             SELECT ROUND(AVG(Rating), 1) AS AverageRating, 
                                    MAX(CASE WHEN UserId = @UserId THEN Rating END) AS UserRating
                             FROM Ratings  
                             WHERE MovieId = @MovieId
                             """;
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(query,
            new { MovieId = movieId, UserId = userId });
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnectionAsync(cancellationToken).Result;
        const string query = """
                             DELETE FROM Ratings 
                             WHERE MovieId = @MovieId 
                             AND UserId = @UserId
                             """;
        var affectedRows = await connection.ExecuteAsync(query, new { MovieId = movieId, UserId = userId });
        return affectedRows > 0;
    }
}