using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("set", Schema = "hangfire")]
[Index("expireat", Name = "ix_hangfire_set_expireat")]
[Index("key", "score", Name = "ix_hangfire_set_key_score")]
[Index("key", "value", Name = "set_key_value_key", IsUnique = true)]
public partial class set
{
    [Key]
    public long id { get; set; }

    public string key { get; set; } = null!;

    public double score { get; set; }

    public string value { get; set; } = null!;

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }
}
