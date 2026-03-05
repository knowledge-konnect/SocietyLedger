using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data;

namespace SocietyLedger.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
        Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly ILogger<DbConnectionFactory> _logger;

        public DbConnectionFactory(IConfiguration configuration, ILogger<DbConnectionFactory> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            _logger = logger;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using var connection = CreateConnection() as NpgsqlConnection;
                await connection!.OpenAsync();
                var result = await connection.ExecuteScalarAsync<T>(sql, param, commandType: commandType);
                return result!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scalar query: {Sql}", sql);
                throw;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using var connection = CreateConnection() as NpgsqlConnection;
                await connection!.OpenAsync();
                var result = await connection.QueryAsync<T>(sql, param, commandType: commandType);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Sql}", sql);
                throw;
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using var connection = CreateConnection() as NpgsqlConnection;
                await connection!.OpenAsync();
                var result = await connection.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Sql}", sql);
                throw;
            }
        }
    }
}
