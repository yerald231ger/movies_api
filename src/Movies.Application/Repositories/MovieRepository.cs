using Dapper;
using Microsoft.Extensions.Logging;
using Movies.Application.Data;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory, ILogger<MovieRepository> logger)
    : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        try
        {
            const string query =
                "INSERT INTO Movies (Id, Title, Slug, YearOfRelease) VALUES (@Id, @Title, @Slug, @YearOfRelease)";
            var result = await connection.ExecuteAsync(new CommandDefinition(query, movie, transaction,
                cancellationToken: cancellationToken));

            foreach (var genre in movie.Genres)
            {
                const string genreQuery = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name)";
                await connection.ExecuteAsync(new CommandDefinition(genreQuery,
                    new { MovieId = movie.Id, Name = genre }, transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();
            return result > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating movie with ID {MovieId}", movie.Id);
            transaction.Rollback();
            return false;
        }
    }

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  SELECT m.*, round(avg(r.Rating), 1) as Rating, myr.Rating as UserRating
                                  FROM Movies m
                                  LEFT JOIN Ratings r ON m.Id = r.MovieId
                                  LEFT JOIN Ratings myr ON m.Id = myr.MovieId AND myr.UserId = @UserId
                                  WHERE m.Id = @Id
                                  GROUP BY m.Id, UserRating
                                  """,
                new { Id = id, UserId = userId },
                cancellationToken: cancellationToken));


        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("SELECT Name FROM Genres WHERE MovieId = @MovieId", new { MovieId = movie.Id },
                cancellationToken: cancellationToken));

        foreach (var genre in genres) movie.Genres.Add(genre);

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  SELECT m.*, round(avg(r.Rating), 1) as Rating, myr.Rating as UserRating
                                  FROM Movies m
                                  LEFT JOIN Ratings r ON m.Id = r.MovieId
                                  LEFT JOIN Ratings myr ON m.Id = myr.MovieId 
                                    AND myr.UserId = @UserId
                                  WHERE m.slug = @Slug
                                  GROUP BY m.Id, UserRating
                                  """,
                new { Slug = slug, UserId = userId },
                cancellationToken: cancellationToken));

        if (movie == null)
            return movie;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("SELECT Name FROM Genres WHERE MovieId = @MovieId", new { MovieId = movie.Id },
                cancellationToken: cancellationToken));

        foreach (var genre in genres)
            movie.AddGenre(genre);

        return movie;
    }

    public async Task<PagedResult<Movie>> GetAllAsync(GetAllMoviesOptions options,
        CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        
        var conditions = new List<string>();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", options.UserId);

        if (!string.IsNullOrWhiteSpace(options.Title))
        {
            conditions.Add("m.Title ILIKE @Title");
            parameters.Add("Title", $"%{options.Title}%");
        }

        if (options.Year.HasValue)
        {
            conditions.Add("m.YearOfRelease = @Year");
            parameters.Add("Year", options.Year.Value);
        }

        var whereClause = conditions.Any() ? $"WHERE {string.Join(" AND ", conditions)}" : "";

        // Get total count
        var countSql = $"""
            SELECT COUNT(DISTINCT m.Id)
            FROM Movies m 
            LEFT JOIN Genres g on m.Id = g.MovieId
            LEFT JOIN Ratings r ON m.Id = r.MovieId
            LEFT JOIN Ratings myr ON m.Id = myr.MovieId AND myr.UserId = @UserId
            {whereClause}
            """;

        var totalCount = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        // Get paginated data
        var orderByClause = GetOrderByClause(options.SortBy);
        var offset = (options.Page - 1) * options.PageSize;
        parameters.Add("Offset", offset);
        parameters.Add("PageSize", options.PageSize);

        var dataSql = $"""
            SELECT m.*, 
                string_agg(distinct g.Name, ', ') AS Genres,
                COALESCE(round(avg(r.Rating), 1), 0) as Rating,
                COALESCE(myr.Rating, 0) as UserRating
            FROM Movies m 
            LEFT JOIN Genres g on m.Id = g.MovieId
            LEFT JOIN Ratings r ON m.Id = r.MovieId
            LEFT JOIN Ratings myr ON m.Id = myr.MovieId AND myr.UserId = @UserId
            {whereClause}
            GROUP BY m.Id, myr.Rating
            {orderByClause}
            LIMIT @PageSize OFFSET @Offset
            """;

        var result = await connection.QueryAsync(
            new CommandDefinition(dataSql, parameters, cancellationToken: cancellationToken));

        var movies = result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = (short)m.yearofrelease,
            Genres = Enumerable.ToList(m.genres?.Split(',') ?? Array.Empty<string>()),
            Rating = (float)m.rating,
            UserRating = m.userrating
        }).ToList();

        return new PagedResult<Movie>
        {
            Items = movies,
            TotalCount = totalCount,
            Page = options.Page,
            PageSize = options.PageSize
        };
    }

    private static string GetOrderByClause(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return "ORDER BY m.Title";

        var direction = "ASC";
        var field = sortBy;

        if (sortBy.StartsWith('+'))
        {
            direction = "ASC";
            field = sortBy[1..];
        }
        else if (sortBy.StartsWith('-'))
        {
            direction = "DESC";
            field = sortBy[1..];
        }

        var column = field.ToLower() switch
        {
            "title" => "m.Title",
            "year" => "m.YearOfRelease",
            "rating" => "Rating",
            "userrating" => "UserRating",
            _ => "m.Title"
        };

        return $"ORDER BY {column} {direction}";
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        try
        {
            const string query =
                "UPDATE Movies SET Title = @Title, Slug = @Slug, YearOfRelease = @YearOfRelease WHERE Id = @Id";
            var result = await connection.ExecuteAsync(new CommandDefinition(query, movie, transaction,
                cancellationToken: cancellationToken));

            if (result == 0)
                return false;

            // Clear existing genres
            await connection.ExecuteAsync(new CommandDefinition("DELETE FROM Genres WHERE MovieId = @MovieId",
                new { MovieId = movie.Id }, transaction, cancellationToken: cancellationToken));

            // Insert new genres
            foreach (var genre in movie.Genres)
            {
                const string genreQuery = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name)";
                await connection.ExecuteAsync(new CommandDefinition(genreQuery,
                    new { MovieId = movie.Id, Name = genre }, transaction, cancellationToken: cancellationToken));
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while updating movie with ID {MovieId}", movie.Id);
            transaction.Rollback();
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        try
        {
            const string deleteMovieQuery = "DELETE FROM Movies WHERE Id = @Id";
            var result = await connection.ExecuteAsync(new CommandDefinition(deleteMovieQuery, new { Id = id },
                cancellationToken: cancellationToken));
            return result > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting movie with ID {MovieId}", id);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var exists = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("SELECT COUNT(1) FROM Movies WHERE Id = @Id", new { Id = id },
                cancellationToken: cancellationToken));
        return exists;
    }
}