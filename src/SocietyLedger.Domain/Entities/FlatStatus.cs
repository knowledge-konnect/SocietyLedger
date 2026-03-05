using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Domain.Entities
{
    public class FlatStatus
    {
        public short Id { get; set; }
        public string Code { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
