using SocietyLedger.Application.DTOs.Reports;
using SocietyLedger.Shared;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IReportRepository
    {
        Task<CollectionSummaryDto> GetCollectionSummaryAsync(long societyId, string? startPeriod, string? endPeriod, CancellationToken ct = default);
        Task<List<DefaulterDto>> GetDefaultersReportAsync(long societyId, decimal minOutstanding = 0, CancellationToken ct = default);
        Task<IncomeVsExpenseDto> GetIncomeVsExpenseAsync(long societyId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
        Task<FundLedgerReportDto> GetFundLedgerAsync(long societyId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
        Task<PagedResult<PaymentRegisterDto>> GetPaymentRegisterAsync(long societyId, DateOnly? startDate, DateOnly? endDate, int page, int pageSize, CancellationToken ct = default);
        Task<ExpenseByCategoryDto> GetExpenseByCategoryAsync(long societyId, DateOnly? startDate, DateOnly? endDate, CancellationToken ct = default);
    }
}
