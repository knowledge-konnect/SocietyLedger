using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("society_fund_ledger")]
public partial class society_fund_ledger
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public string entry_type { get; set; } = null!;

    public string? reference { get; set; }

    public string? notes { get; set; }

    public long created_by { get; set; }

    public DateTime created_at { get; set; }

    public bool? is_deleted { get; set; }

    public DateOnly? transaction_date { get; set; }
}
