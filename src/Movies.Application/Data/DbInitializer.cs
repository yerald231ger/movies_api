using Dapper;

namespace Movies.Application.Data;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    public async Task InitializeAsync(string connectionString)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Movies (
                Id UUID PRIMARY KEY,
                Title TEXT NOT NULL,
                Slug TEXT NOT NULL UNIQUE,
                YearOfRelease INT NOT NULL
            );
            """);

        await connection.ExecuteAsync(
            """
            CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS idx_movies_slug 
            ON Movies
            USING BTREE (Slug);
            """);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Genres ( 
                MovieId UUID NOT NULL REFERENCES Movies(Id) ON DELETE CASCADE,
                Name TEXT NOT NULL
            );
            """);

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Ratings (
                MovieId UUID NOT NULL REFERENCES Movies(Id) ON DELETE CASCADE,
                Rating INTEGER NOT NULL,
                UserId UUID,
                PRIMARY KEY (MovieId, UserId)
            )
            """);
    }
}