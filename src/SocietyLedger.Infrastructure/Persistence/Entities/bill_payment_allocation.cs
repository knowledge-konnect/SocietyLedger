using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("bill_id", Name = "idx_allocations_bill")]
[Index("payment_id", Name = "idx_allocations_payment")]
public partial class bill_payment_allocation
{
    [Key]
    public long id { get; set; }

    public long payment_id { get; set; }

    public long bill_id { get; set; }

    [Precision(13, 2)]
    public decimal allocated_amount { get; set; }

    [ForeignKey("bill_id")]
    [InverseProperty("bill_payment_allocations")]
    public virtual bill bill { get; set; } = null!;

    [ForeignKey("payment_id")]
    [InverseProperty("bill_payment_allocations")]
    public virtual maintenance_payment payment { get; set; } = null!;
}
