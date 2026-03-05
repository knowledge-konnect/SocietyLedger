using SocietyLedger.Application.DTOs.Reports;
using SocietyLedger.Shared;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<CollectionSummaryDto> GetCollectionSummaryAsync(long userId, string? startPeriod, string? endPeriod, CancellationToken ct = default);
        Task<List<DefaulterDto>> GetDefaultersReportAsync(long userId, decimal minOutstanding = 0, CancellationToken ct = default);
        Task<IncomeVsExpenseDto> GetIncomeVsExpenseAsync(long userId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
        Task<FundLedgerReportDto> GetFundLedgerAsync(long userId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
        Task<PagedResult<PaymentRegisterDto>> GetPaymentRegisterAsync(long userId, DateOnly? startDate, DateOnly? endDate, int page, int pageSize, CancellationToken ct = default);
        Task<ExpenseByCategoryDto> GetExpenseByCategoryAsync(long userId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
    }
}
