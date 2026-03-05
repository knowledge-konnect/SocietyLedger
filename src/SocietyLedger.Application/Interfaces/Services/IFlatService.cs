using SocietyLedger.Application.DTOs.Flat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Interfaces.Services
{
    public interface IFlatService
    {
        Task<IEnumerable<FlatResponseDto>> GetBySocietyIdAsync(long societyId);
        /// <summary>
        /// Get a flat by its public UUID with tenant isolation.
        /// </summary>
        Task<FlatResponseDto?> GetByPublicIdAsync(Guid publicId, long userId);

        /// <summary>
        /// Create a new flat and return the created flat DTO.
        /// </summary>
        Task<FlatResponseDto> CreateAsync(CreateFlatDto dto, long userId);

        /// <summary>
        /// Update an existing flat with tenant isolation.
        /// Returns the updated DTO if found, or null if not.
        /// </summary>
        Task<FlatResponseDto?> UpdateAsync(UpdateFlatDto dto, long userId);

        /// <summary>
        /// Delete a flat by its public UUID with tenant isolation.
        /// Returns true if the record was deleted, false if not found.
        /// </summary>
        Task<bool> DeleteByPublicIdAsync(Guid publicId, long userId);

        /// <summary>
        /// Returns all flat statuses suitable for populating a dropdown.
        /// </summary>
        Task<IEnumerable<FlatStatusDto>> GetAllAsync();

        /// <summary>
        /// Get flat ledger with all transactions and running balance.
        /// </summary>
        Task<FlatLedgerResponse> GetFlatLedgerAsync(Guid publicId, long userId, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get flat financial summary with balances and charges.
        /// </summary>
        Task<FlatFinancialSummaryResponse> GetFlatFinancialSummaryAsync(Guid publicId, long userId);
    }
}
