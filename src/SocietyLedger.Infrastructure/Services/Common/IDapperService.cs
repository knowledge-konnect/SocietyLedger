using Npgsql;
using System.Data;

namespace SocietyLedger.Infrastructure.Services.Common
{
    /// <summary>
    /// Reusable abstraction for all raw Dapper database operations.
    /// Inject this wherever SQL queries or transactional Dapper work is needed.
    /// Never import Dapper or NpgsqlConnection directly in service classes.
    /// </summary>
    public interface IDapperService
    {
        // ── Connection-less helpers (open + close internally) ─────────────

        /// <summary>Executes a query and returns a typed list.</summary>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null);

        /// <summary>Executes a query and returns the first row or null.</summary>
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null);

        /// <summary>Executes a non-query SQL (INSERT / UPDATE / DELETE) and returns rows affected.</summary>
        Task<int> ExecuteAsync(string sql, object? param = null);

        /// <summary>Executes a scalar SQL and returns a single typed value.</summary>
        Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null);

        // ── Transactional helpers (caller controls the connection scope) ───

        /// <summary>
        /// Opens a new connection, begins a transaction at the requested isolation level,
        /// and returns both so the caller can run multiple statements atomically.
        /// The caller is responsible for CommitAsync / RollbackAsync and for disposing both.
        /// </summary>
        Task<(NpgsqlConnection Connection, NpgsqlTransaction Transaction)> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.RepeatableRead);

        /// <summary>Runs a typed query on an existing connection inside a transaction.</summary>
        Task<IEnumerable<T>> QueryAsync<T>(NpgsqlConnection conn, NpgsqlTransaction tx, string sql, object? param = null);

        /// <summary>Returns the first row or null on an existing connection inside a transaction.</summary>
        Task<T?> QueryFirstOrDefaultAsync<T>(NpgsqlConnection conn, NpgsqlTransaction tx, string sql, object? param = null);

        /// <summary>Executes a non-query on an existing connection inside a transaction.</summary>
        Task<int> ExecuteAsync(NpgsqlConnection conn, NpgsqlTransaction tx, string sql, object? param = null);

        /// <summary>Executes a scalar on an existing connection inside a transaction.</summary>
        Task<T?> ExecuteScalarAsync<T>(NpgsqlConnection conn, NpgsqlTransaction tx, string sql, object? param = null);
    }
}
