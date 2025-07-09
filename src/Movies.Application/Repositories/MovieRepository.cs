using Dapper;
using Microsoft.Extensions.Logging;
using Movies.Application.Data;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory, ILogger<MovieRepository> logger)
    : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string query =
                "INSERT INTO Movies (Id, Title, Slug, YearOfRelease) VALUES (@Id, @Title, @Slug, @YearOfRelease)";
            var result = await connection.ExecuteAsync(query, movie, transaction);

            foreach (var genre in movie.Genres)
            {
                const string genreQuery = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name)";
                await connection.ExecuteAsync(genreQuery, new { MovieId = movie.Id, Name = genre }, transaction);
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

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = dbConnectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            "SELECT * FROM Movies WHERE Id = @Id",
            new { Id = id });

        if (movie is null)
            return null;

        var genres = await connection.QueryAsync<string>(
            "SELECT Name FROM Genres WHERE MovieId = @MovieId",
            new { MovieId = movie.Id });

        foreach (var genre in genres) movie.Genres.Add(genre);

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            "SELECT * FROM Movies WHERE Slug = @Slug"
            , new { Slug = slug });

        if (movie == null)
            return movie;

        var genres = await connection.QueryAsync<string>(
            "SELECT Name FROM Genres WHERE MovieId = @MovieId"
            , new { MovieId = movie.Id });

        foreach (var genre in genres)
            movie.AddGenre(genre);

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var commandDefinition = new CommandDefinition(
            """
            SELECT m.*, string_agg(g.Name, ', ') AS Genres
            FROM Movies m left join Genres g on m.Id = g.MovieId
            GROUP BY m.Id
            """);

        var result = await connection.QueryAsync(commandDefinition);

        var movies = result.Select(m => new Movie
        {
            Id = m.id,
            Title = m.title,
            YearOfRelease = (short)m.yearofrelease,
            Genres = Enumerable.ToList(m.genres.Split(','))
        }).ToList();

        return movies;
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();
        try
        {
            const string query =
                "UPDATE Movies SET Title = @Title, Slug = @Slug, YearOfRelease = @YearOfRelease WHERE Id = @Id";
            var result = await connection.ExecuteAsync(query, movie, transaction);

            if (result == 0)
                return false;

            // Clear existing genres
            await connection.ExecuteAsync("DELETE FROM Genres WHERE MovieId = @MovieId",
                new { MovieId = movie.Id }, transaction);

            // Insert new genres
            foreach (var genre in movie.Genres)
            {
                const string genreQuery = "INSERT INTO Genres (MovieId, Name) VALUES (@MovieId, @Name)";
                await connection.ExecuteAsync(genreQuery, new { MovieId = movie.Id, Name = genre }, transaction);
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

    public async Task<bool> DeleteAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        try
        {
            const string deleteMovieQuery = "DELETE FROM Movies WHERE Id = @Id";
            var result = await connection.ExecuteAsync(deleteMovieQuery, new { Id = id });
            return result > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting movie with ID {MovieId}", id);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var exists = await connection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(1) FROM Movies WHERE Id = @Id",
            new { Id = id });
        return exists;
    }
}