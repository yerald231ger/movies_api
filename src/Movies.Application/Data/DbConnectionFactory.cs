using System.Data;

namespace Movies.Application.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync();
}

public class NpgsqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new Npgsql.NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}