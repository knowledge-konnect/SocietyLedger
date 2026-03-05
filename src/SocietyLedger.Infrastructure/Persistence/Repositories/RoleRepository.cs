using Microsoft.EntityFrameworkCore;
using SocietyLedger.Application.Interfaces.Repositories;
using SocietyLedger.Domain.Entities;
using SocietyLedger.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _db;

        public RoleRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Role?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            var entity = await _db.roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.code.ToLower() == code.ToLower());

            return entity?.ToDomain();
        }
        public async Task<Role?> GetByIdAsync(short id)
        {
            var entity = await _db.roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.id == id);

            return entity?.ToDomain();
        }

        public async Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<short> ids)
        {
            if (ids == null || !ids.Any())
                return Enumerable.Empty<Role>();

            var entities = await _db.roles
                .AsNoTracking()
                .Where(r => ids.Contains(r.id))
                .ToListAsync();

            return entities.Select(e => e.ToDomain()).Where(r => r != null).Cast<Role>();
        }

        public async Task AddAsync(Role role)
        {
            var entity = role.ToEntity();
            await _db.roles.AddAsync(entity);
            await _db.SaveChangesAsync();
            role.Id = entity.id;
        }
    }
}
