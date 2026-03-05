using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using SocietyLedger.Application.DTOs.Reports;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Infrastructure.Data;
using SocietyLedger.Shared;
using System.Data;
using System.Text.Json;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ReportRepository> _logger;

        private static readonly JsonSerializerOptions JsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public ReportRepository(IDbConnectionFactory connectionFactory, ILogger<ReportRepository> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<CollectionSummaryDto> GetCollectionSummaryAsync(
            long societyId, string? startPeriod, string? endPeriod, CancellationToken ct = default)
        {
            const string sql = "SELECT public.get_collection_summary(@SocietyId, @StartPeriod, @EndPeriod)";
            var json = await QueryJsonAsync(sql, new { SocietyId = societyId, StartPeriod = startPeriod, EndPeriod = endPeriod }, ct);
            return Deserialize<CollectionSummaryDto>(json) ?? new CollectionSummaryDto();
        }

        public async Task<List<DefaulterDto>> GetDefaultersReportAsync(
            long societyId, decimal minOutstanding = 0, CancellationToken ct = default)
        {
            const string sql = "SELECT public.get_defaulters_report(@SocietyId, @MinOutstanding)";
            var json = await QueryJsonAsync(sql, new { SocietyId = societyId, MinOutstanding = minOutstanding }, ct);
            return Deserialize<List<DefaulterDto>>(json) ?? new List<DefaulterDto>();
        }

        public async Task<IncomeVsExpenseDto> GetIncomeVsExpenseAsync(
            long societyId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default)
        {
            const string sql = "SELECT public.get_income_vs_expense(@SocietyId, @StartDate::date, @EndDate::date)";
            var json = await QueryJsonAsync(sql, DateParams(societyId, startDate, endDate), ct);
            return Deserialize<IncomeVsExpenseDto>(json) ?? new IncomeVsExpenseDto();
        }

        public async Task<FundLedgerReportDto> GetFundLedgerAsync(
            long societyId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default)
        {
            const string sql = "SELECT public.get_fund_ledger_report(@SocietyId, @StartDate::date, @EndDate::date)";
            var json = await QueryJsonAsync(sql, DateParams(societyId, startDate, endDate), ct);
            return Deserialize<FundLedgerReportDto>(json) ?? new FundLedgerReportDto();
        }

        /// <summary>
        /// Fetches a paginated payment register by calling two PostgreSQL functions in parallel
        /// on separate connections: one for the data page, one for the total row count.
        /// No JSON parsing — Dapper maps directly to <see cref="PaymentRegisterRow"/> then
        /// projects to the public <see cref="PaymentRegisterDto"/>.
        /// </summary>
        public async Task<PagedResult<PaymentRegisterDto>> GetPaymentRegisterAsync(
            long societyId, DateOnly? startDate, DateOnly? endDate,
            int page, int pageSize, CancellationToken ct = default)
        {
            if (page     < 1) page     = 1;
            if (pageSize < 1) pageSize = 50;

            var offset = (page - 1) * pageSize;

            // Two independent read-only queries — run on separate connections so they
            // execute genuinely in parallel without blocking each other.
            await using var dataConn  = (NpgsqlConnection)_connectionFactory.CreateConnection();
            await using var countConn = (NpgsqlConnection)_connectionFactory.CreateConnection();

            // Open both connections concurrently to minimise connection-acquisition latency.
            await Task.WhenAll(dataConn.OpenAsync(ct), countConn.OpenAsync(ct));

            const string dataSql =
                "SELECT * FROM public.get_maintenance_payment_register(" +
                "@SocietyId, @StartDate::date, @EndDate::date, @p_limit, @p_offset)";

            // Build parameter bags — date params with explicit DbType.Date so Npgsql
            // sends PostgreSQL 'date' (not 'timestamp without time zone').
            var dataParams  = PagedDateParams(societyId, startDate, endDate, pageSize, offset);

            // Only one query needed; total_count comes from the result set.
            var rows = await dataConn.QueryAsync<PaymentRegisterRow>(
                dataSql, dataParams, commandTimeout: 30);

            // Get total_count from the first row (if any), else 0.
            var totalCount = rows.FirstOrDefault()?.total_count ?? 0L;

            var items = rows
                .Select(r => new PaymentRegisterDto
                {
                    DatePaid    = DateOnly.FromDateTime(r.date_paid),
                    FlatNo      = r.flat_no,
                    OwnerName   = r.owner_name,
                    Amount      = r.amount,
                    PaymentMode = r.payment_mode,
                    Reference   = r.reference,
                    Notes       = r.notes,
                    Period      = r.period,
                    PeriodLabel = r.period_label,
                    RecordedBy  = r.recorded_by
                })
                .ToList();   // materialise once; avoid double-enumeration

            return new PagedResult<PaymentRegisterDto>(items, totalCount, page, pageSize);
        }

        public async Task<ExpenseByCategoryDto> GetExpenseByCategoryAsync(
            long societyId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default)
        {
            const string sql = "SELECT public.get_expense_by_category(@SocietyId, @StartDate::date, @EndDate::date)";
            var json = await QueryJsonAsync(sql, DateParams(societyId, startDate, endDate), ct);
            return Deserialize<ExpenseByCategoryDto>(json) ?? new ExpenseByCategoryDto();
        }

        // ------------------------------------------------------------------ //
        //  Helpers                                                             //
        // ------------------------------------------------------------------ //

        private async Task<string> QueryJsonAsync(string sql, object parameters, CancellationToken ct)
        {
            using var connection = _connectionFactory.CreateConnection() as NpgsqlConnection;
            await connection!.OpenAsync(ct);
            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, parameters, commandTimeout: 30);
            return result ?? "{}";
        }

        private static T? Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json, JsonOptions);

        /// <summary>
        /// Date-only parameters (no pagination). Used by all non-paginated report queries.
        /// </summary>
        private static DynamicParameters DateParams(long societyId, DateOnly? start, DateOnly? end)
        {
            var p = new DynamicParameters();
            p.Add("SocietyId", societyId);
            // Npgsql 6+ maps DateOnly → PostgreSQL 'date', avoiding ambiguous timestamp inference.
            p.Add("StartDate", start.HasValue ? (object)start.Value : DBNull.Value, DbType.Date);
            p.Add("EndDate",   end.HasValue   ? (object)end.Value   : DBNull.Value, DbType.Date);
            return p;
        }

        /// <summary>
        /// Date + pagination parameters for <c>get_maintenance_payment_register</c>.
        /// </summary>
        private static DynamicParameters PagedDateParams(
            long societyId, DateOnly? start, DateOnly? end, int limit, int offset)
        {
            var p = DateParams(societyId, start, end);
            p.Add("p_limit",  limit);
            p.Add("p_offset", offset);
            return p;
        }

        // ------------------------------------------------------------------ //
        //  Private Dapper projection type                                     //
        //                                                                     //
        //  Property names intentionally use snake_case to match the column    //
        //  names returned by get_maintenance_payment_register exactly.        //
        //  Dapper matches constructor parameter names to column names in a    //
        //  case-insensitive comparison — no custom type mapper required.      //
        //  This record stays private; callers always receive PaymentRegisterDto.
        // ------------------------------------------------------------------ //
        private sealed record PaymentRegisterRow(
            DateTime  date_paid,
            string    flat_no,
            string    owner_name,
            decimal   amount,
            string?   payment_mode,
            string?   reference,
            string?   notes,
            string?   period,
            string?   period_label,
            string?   recorded_by,
            long      total_count);
    }
}
