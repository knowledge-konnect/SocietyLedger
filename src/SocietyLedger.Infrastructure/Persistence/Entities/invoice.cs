using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("due_date", Name = "idx_invoices_due_date")]
[Index("invoice_number", Name = "idx_invoices_invoice_number")]
[Index("status", Name = "idx_invoices_status")]
[Index("user_id", Name = "idx_invoices_user_id")]
[Index("invoice_number", Name = "invoices_invoice_number_key", IsUnique = true)]
public partial class invoice
{
    [Key]
    public Guid id { get; set; }

    public long user_id { get; set; }

    public Guid? subscription_id { get; set; }

    [StringLength(50)]
    public string invoice_number { get; set; } = null!;

    [StringLength(30)]
    public string invoice_type { get; set; } = null!;

    [Precision(10, 2)]
    public decimal amount { get; set; }

    [Precision(10, 2)]
    public decimal? tax_amount { get; set; }

    [Precision(10, 2)]
    public decimal total_amount { get; set; }

    [StringLength(3)]
    public string? currency { get; set; }

    [StringLength(20)]
    public string status { get; set; } = null!;

    public DateOnly? period_start { get; set; }

    public DateOnly? period_end { get; set; }

    public DateOnly due_date { get; set; }

    public DateTime? paid_date { get; set; }

    [StringLength(50)]
    public string? payment_method { get; set; }

    [StringLength(255)]
    public string? payment_reference { get; set; }

    public string? description { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    [ForeignKey("subscription_id")]
    [InverseProperty("invoices")]
    public virtual subscription? subscription { get; set; }

    [ForeignKey("user_id")]
    [InverseProperty("invoices")]
    public virtual user user { get; set; } = null!;
}
