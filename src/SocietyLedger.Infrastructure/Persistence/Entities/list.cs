using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("list", Schema = "hangfire")]
[Index("expireat", Name = "ix_hangfire_list_expireat")]
public partial class list
{
    [Key]
    public long id { get; set; }

    public string key { get; set; } = null!;

    public string? value { get; set; }

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }
}
