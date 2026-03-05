using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("jobqueue", Schema = "hangfire")]
[Index("jobid", "queue", Name = "ix_hangfire_jobqueue_jobidandqueue")]
[Index("queue", "fetchedat", Name = "ix_hangfire_jobqueue_queueandfetchedat")]
public partial class jobqueue
{
    [Key]
    public long id { get; set; }

    public long jobid { get; set; }

    public string queue { get; set; } = null!;

    public DateTime? fetchedat { get; set; }

    public int updatecount { get; set; }
}
