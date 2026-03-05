using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Keyless]
[Table("lock", Schema = "hangfire")]
[Index("resource", Name = "lock_resource_key", IsUnique = true)]
public partial class _lock
{
    public string resource { get; set; } = null!;

    public int updatecount { get; set; }

    public DateTime? acquired { get; set; }
}
