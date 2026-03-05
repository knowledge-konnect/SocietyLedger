using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("hash", Schema = "hangfire")]
[Index("key", "field", Name = "hash_key_field_key", IsUnique = true)]
[Index("expireat", Name = "ix_hangfire_hash_expireat")]
public partial class hash
{
    [Key]
    public long id { get; set; }

    public string key { get; set; } = null!;

    public string field { get; set; } = null!;

    public string? value { get; set; }

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }
}
