using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SocietyLedger.Domain.Constants;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("razorpay_payments")]
public partial class razorpay_payment
{
    [Key]
    public Guid id { get; set; }

    public long user_id { get; set; }

    [StringLength(255)]
    public string razorpay_order_id { get; set; } = null!;

    [StringLength(255)]
    public string? razorpay_payment_id { get; set; }

    [StringLength(500)]
    public string? razorpay_signature { get; set; }

    [Precision(10, 2)]
    public decimal amount { get; set; }

    [StringLength(3)]
    public string currency { get; set; } = "INR";

    [StringLength(20)]
    public string status { get; set; } = InvoiceStatusCodes.Pending;

    public string? failure_reason { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? verified_at { get; set; }

    [ForeignKey("user_id")]
    [InverseProperty("razorpay_payments")]
    public virtual user? user { get; set; }
}