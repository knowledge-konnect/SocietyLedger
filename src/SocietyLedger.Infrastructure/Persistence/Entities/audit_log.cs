using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("society_id", Name = "idx_audit_logs_society")]
[Index("table_name", Name = "idx_audit_logs_table")]
public partial class audit_log
{
    [Key]
    public long id { get; set; }

    public long? society_id { get; set; }

    public string table_name { get; set; } = null!;

    public long? record_id { get; set; }

    public Guid? record_public_id { get; set; }

    public string action { get; set; } = null!;

    public long? changed_by { get; set; }

    public DateTime changed_at { get; set; }

    [Column(TypeName = "jsonb")]
    public string? diff { get; set; }

    [Column(TypeName = "jsonb")]
    public string? metadata { get; set; }
}
