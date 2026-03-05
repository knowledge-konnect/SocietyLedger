using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("aggregatedcounter", Schema = "hangfire")]
[Index("key", Name = "aggregatedcounter_key_key", IsUnique = true)]
public partial class aggregatedcounter
{
    [Key]
    public long id { get; set; }

    public string key { get; set; } = null!;

    public long value { get; set; }

    public DateTime? expireat { get; set; }
}
