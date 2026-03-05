using SocietyLedger.Domain.Entities;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IFlatRepository
    {
        Task<IEnumerable<Flat>> GetBySocietyIdAsync(long societyId);
        Task<Flat?> GetByPublicIdAsync(Guid publicId, long societyId);
        Task<Flat?> GetByFlatNoAndSocietyAsync(string flatNo, long societyId);
        Task AddAsync(Flat entity);
        Task UpdateAsync(Flat entity, long societyId);
        Task DeleteByPublicIdAsync(Guid publicId, long societyId);
        Task SaveChangesAsync();
        Task<IEnumerable<FlatStatus>> GetAllAsync();
        Task<FlatStatus?> GetByCodeAsync(string code);
    }
}
