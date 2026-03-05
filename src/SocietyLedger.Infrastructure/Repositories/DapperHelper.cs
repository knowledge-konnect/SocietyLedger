using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace SocietyLedger.Infrastructure.Repositories
{
    /// <summary>
    /// Thin wrappers over Dapper that accept an <see cref="NpgsqlTransaction"/>.
    /// All Dapper usage must go through these helpers — never import Dapper directly
    /// in service or repository classes.
    /// </summary>
    public static class DapperHelper
    {
        public static async Task<IEnumerable<T>> QueryAsync<T>(
            this NpgsqlConnection conn, string sql, object param, NpgsqlTransaction tx)
        {
            return await conn.QueryAsync<T>(sql, param, tx);
        }

        public static async Task<T?> QueryFirstOrDefaultAsync<T>(
            this NpgsqlConnection conn, string sql, object param, NpgsqlTransaction tx)
        {
            return await conn.QueryFirstOrDefaultAsync<T>(sql, param, tx);
        }

        public static async Task<int> ExecuteAsync(
            this NpgsqlConnection conn, string sql, object param, NpgsqlTransaction tx)
        {
            return await conn.ExecuteAsync(sql, param, tx);
        }

        public static async Task<T?> ExecuteScalarAsync<T>(
            this NpgsqlConnection conn, string sql, object param, NpgsqlTransaction tx)
        {
            return await conn.ExecuteScalarAsync<T>(sql, param, tx);
        }
    }
}
