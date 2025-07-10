using Microsoft.Extensions.Options;
using Movies.Auth.Models;
using Movies.Auth.Options;

namespace Movies.Auth.Repositories;

public class JsonUserRepository(IOptions<UserOptions> userOptions) : IUserRepository
{
    private readonly UserOptions _userOptions = userOptions.Value;

    public Task<User?> GetByUsernameAsync(string username)
    {
        var user = _userOptions.Users.FirstOrDefault(u => u.Username == username);
        return Task.FromResult(user);
    }

    public Task<bool> ValidateCredentialsAsync(string username, string password)
    {
        var user = _userOptions.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        return Task.FromResult(user != null);
    }
}