using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("jobs")]
[Index("society_id", Name = "idx_jobs_society")]
[Index("status", Name = "idx_jobs_status")]
[Index("public_id", Name = "ux_jobs_public_id", IsUnique = true)]
public partial class job1
{
    [Key]
    public long id { get; set; }

    public long? society_id { get; set; }

    public string job_type { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string? payload { get; set; }

    public string status { get; set; } = null!;

    [Column(TypeName = "jsonb")]
    public string? result { get; set; }

    public int attempts { get; set; }

    public string? last_error { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public Guid public_id { get; set; }
}
