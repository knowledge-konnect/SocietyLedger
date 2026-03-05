using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using SocietyLedger.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class SocietyRepository : ISocietyRepository
    {
        private readonly AppDbContext _db;
        public SocietyRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Society society)
        {
            var entity = society.ToEntity();

            await _db.societies.AddAsync(entity);

            await _db.SaveChangesAsync();

            // Use reflection to set protected properties
            var setId = typeof(BaseEntity).GetMethod("SetId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setId?.Invoke(society, new object[] { entity.id });

            var setPublicId = typeof(BaseEntity).GetMethod("SetPublicId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            setPublicId?.Invoke(society, new object[] { entity.public_id });
        }

        public async Task<Society?> GetByIdAsync(long id)
        {
            var entity = await _db.societies
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.id == id);

            return entity?.ToDomain();
        }

        public async Task<Society?> GetByPublicIdAsync(Guid publicId)
        {
            var entity = await _db.societies
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.public_id == publicId);

            return entity?.ToDomain();
        }

        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
