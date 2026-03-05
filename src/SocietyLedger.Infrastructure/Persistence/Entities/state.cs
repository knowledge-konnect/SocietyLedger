using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("state", Schema = "hangfire")]
[Index("jobid", Name = "ix_hangfire_state_jobid")]
public partial class state
{
    [Key]
    public long id { get; set; }

    public long jobid { get; set; }

    public string name { get; set; } = null!;

    public string? reason { get; set; }

    public DateTime createdat { get; set; }

    [Column(TypeName = "jsonb")]
    public string? data { get; set; }

    public int updatecount { get; set; }

    [ForeignKey("jobid")]
    [InverseProperty("states")]
    public virtual job job { get; set; } = null!;
}
