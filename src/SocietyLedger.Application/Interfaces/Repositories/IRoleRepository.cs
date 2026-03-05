using SocietyLedger.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetByCodeAsync(string code);

        Task<Role?> GetByIdAsync(short id);

        Task<IEnumerable<Role>> GetByIdsAsync(IEnumerable<short> ids);
    }
}
