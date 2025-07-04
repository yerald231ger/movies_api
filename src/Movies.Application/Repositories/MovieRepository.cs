using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];
    
    public Task<bool> CreateAsync(Movie movie)
    {
        if (_movies.Any(m => m.Id == movie.Id))
            return Task.FromResult(false);
        
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Movie>>(_movies);
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var existingMovie = _movies.FindIndex(m => m.Id == movie.Id);
        if (existingMovie == -1)
        {
            return Task.FromResult(false);
        }
        
        _movies[existingMovie] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var removedCount = _movies.RemoveAll(m => m.Id == id);
        var movieRemoved = removedCount > 0;
        return Task.FromResult(movieRemoved);
    }
}