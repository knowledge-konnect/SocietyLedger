using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("jobparameter", Schema = "hangfire")]
[Index("jobid", "name", Name = "ix_hangfire_jobparameter_jobidandname")]
public partial class jobparameter
{
    [Key]
    public long id { get; set; }

    public long jobid { get; set; }

    public string name { get; set; } = null!;

    public string? value { get; set; }

    public int updatecount { get; set; }

    [ForeignKey("jobid")]
    [InverseProperty("jobparameters")]
    public virtual job job { get; set; } = null!;
}
