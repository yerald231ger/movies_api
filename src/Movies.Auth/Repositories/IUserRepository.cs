using Movies.Auth.Models;

namespace Movies.Auth.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> ValidateCredentialsAsync(string username, string password);
}