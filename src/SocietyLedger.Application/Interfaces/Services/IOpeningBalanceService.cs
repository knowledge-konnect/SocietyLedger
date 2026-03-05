using SocietyLedger.Application.DTOs.OpeningBalance;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IOpeningBalanceService
    {
        /// <summary>
        /// Apply opening balance for a society. Can only be executed once per society.
        /// </summary>
        Task ApplyOpeningBalanceAsync(OpeningBalanceRequest request, long societyId, long userId);

        /// <summary>
        /// Get opening balance status for a society.
        /// </summary>
        Task<OpeningBalanceStatusResponse> GetStatusAsync(long societyId);
    }
}
