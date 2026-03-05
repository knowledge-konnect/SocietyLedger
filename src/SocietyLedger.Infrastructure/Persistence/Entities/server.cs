using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("server", Schema = "hangfire")]
public partial class server
{
    [Key]
    public string id { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string? data { get; set; }

    public DateTime lastheartbeat { get; set; }

    public int updatecount { get; set; }
}
