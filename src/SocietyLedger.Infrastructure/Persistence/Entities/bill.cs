using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("flat_id", Name = "idx_bills_flat")]
[Index("society_id", "created_at", Name = "idx_bills_society_date")]
[Index("society_id", "period", Name = "idx_bills_society_period")]
[Index("society_id", "status_code", Name = "idx_bills_society_status")]
[Index("public_id", Name = "ux_bills_public_id", IsUnique = true)]
public partial class bill
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public long flat_id { get; set; }

    public string period { get; set; } = null!;

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public DateOnly? due_date { get; set; }

    public string status_code { get; set; } = null!;

    public long? generated_by { get; set; }

    public DateTime generated_at { get; set; }

    public string? note { get; set; }

    public string? source { get; set; }

    public DateTime created_at { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    public long? maintenance_plan_id { get; set; }

    [Precision(13, 2)]
    public decimal? paid_amount { get; set; }

    [Precision(13, 2)]
    public decimal? balance_amount { get; set; }

    public DateTime? updated_at { get; set; }

    [InverseProperty("bill")]
    public virtual ICollection<bill_item> bill_items { get; set; } = new List<bill_item>();

    [InverseProperty("bill")]
    public virtual ICollection<bill_payment_allocation> bill_payment_allocations { get; set; } = new List<bill_payment_allocation>();

    [ForeignKey("flat_id")]
    [InverseProperty("bills")]
    public virtual flat flat { get; set; } = null!;

    [ForeignKey("generated_by")]
    [InverseProperty("bills")]
    public virtual user? generated_byNavigation { get; set; }

    [InverseProperty("bill")]
    public virtual ICollection<maintenance_payment> maintenance_payments { get; set; } = new List<maintenance_payment>();

    [ForeignKey("maintenance_plan_id")]
    [InverseProperty("bills")]
    public virtual maintenance_plan? maintenance_plan { get; set; }

    [InverseProperty("bill")]
    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    [ForeignKey("society_id")]
    [InverseProperty("bills")]
    public virtual society society { get; set; } = null!;
}
