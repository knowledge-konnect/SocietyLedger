using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("job", Schema = "hangfire")]
[Index("expireat", Name = "ix_hangfire_job_expireat")]
[Index("statename", Name = "ix_hangfire_job_statename")]
public partial class job
{
    [Key]
    public long id { get; set; }

    public long? stateid { get; set; }

    public string? statename { get; set; }

    [Column(TypeName = "jsonb")]
    public string invocationdata { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string arguments { get; set; } = null!;

    public DateTime createdat { get; set; }

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }

    [InverseProperty("job")]
    public virtual ICollection<jobparameter> jobparameters { get; set; } = new List<jobparameter>();

    [InverseProperty("job")]
    public virtual ICollection<state> states { get; set; } = new List<state>();
}
