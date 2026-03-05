using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("bill_id", Name = "idx_maintenance_bill")]
[Index("flat_id", Name = "idx_maintenance_flat")]
[Index("payment_mode_id", Name = "idx_maintenance_payment_mode")]
[Index("society_id", "payment_date", Name = "idx_maintenance_society_date")]
public partial class maintenance_payment
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public long flat_id { get; set; }

    public long? bill_id { get; set; }

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public DateTime payment_date { get; set; }

    public short payment_mode_id { get; set; }

    public string? reference_number { get; set; }

    public string? receipt_url { get; set; }

    public string? notes { get; set; }

    public long? recorded_by { get; set; }

    public DateTime created_at { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    public string? idempotency_key { get; set; }

    public long? adjustment_id { get; set; }

    [ForeignKey("adjustment_id")]
    [InverseProperty("maintenance_payments")]
    public virtual adjustment? adjustment { get; set; }

    [ForeignKey("bill_id")]
    [InverseProperty("maintenance_payments")]
    public virtual bill? bill { get; set; }

    [InverseProperty("payment")]
    public virtual ICollection<bill_payment_allocation> bill_payment_allocations { get; set; } = new List<bill_payment_allocation>();

    [ForeignKey("flat_id")]
    [InverseProperty("maintenance_payments")]
    public virtual flat flat { get; set; } = null!;

    [ForeignKey("payment_mode_id")]
    [InverseProperty("maintenance_payments")]
    public virtual payment_mode payment_mode { get; set; } = null!;

    [ForeignKey("recorded_by")]
    [InverseProperty("maintenance_payments")]
    public virtual user? recorded_byNavigation { get; set; }

    [ForeignKey("society_id")]
    [InverseProperty("maintenance_payments")]
    public virtual society society { get; set; } = null!;
}
