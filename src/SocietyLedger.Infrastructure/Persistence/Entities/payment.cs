using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("bill_id", Name = "idx_payments_bill_id")]
[Index("razorpay_order_id", Name = "idx_payments_razorpay_order")]
[Index("society_id", "date_paid", Name = "idx_payments_society_date")]
[Index("public_id", Name = "ux_payments_public_id", IsUnique = true)]
public partial class payment
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public long? bill_id { get; set; }

    public long? flat_id { get; set; }

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public DateTime? date_paid { get; set; }

    public string? mode_code { get; set; }

    public string? reference { get; set; }

    public string? receipt_url { get; set; }

    public long? recorded_by { get; set; }

    public string? idempotency_key { get; set; }

    public long? reversed_by_payment_id { get; set; }

    public DateTime created_at { get; set; }

    public string? razorpay_order_id { get; set; }

    public string? razorpay_payment_id { get; set; }

    public string? razorpay_signature { get; set; }

    public string? payment_type { get; set; }

    public DateTime? verified_at { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    [ForeignKey("bill_id")]
    [InverseProperty("payments")]
    public virtual bill? bill { get; set; }

    [ForeignKey("flat_id")]
    [InverseProperty("payments")]
    public virtual flat? flat { get; set; }

    [ForeignKey("recorded_by")]
    [InverseProperty("payments")]
    public virtual user? recorded_byNavigation { get; set; }

    [ForeignKey("society_id")]
    [InverseProperty("payments")]
    public virtual society society { get; set; } = null!;
}
