using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("counter", Schema = "hangfire")]
[Index("expireat", Name = "ix_hangfire_counter_expireat")]
[Index("key", Name = "ix_hangfire_counter_key")]
public partial class counter
{
    [Key]
    public long id { get; set; }

    public string key { get; set; } = null!;

    public long value { get; set; }

    public DateTime? expireat { get; set; }
}
