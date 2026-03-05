using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("public_id", Name = "bill_items_public_id_key", IsUnique = true)]
[Index("bill_id", Name = "idx_bill_items_bill")]
public partial class bill_item
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long bill_id { get; set; }

    public string component_name { get; set; } = null!;

    public string calculation_type { get; set; } = null!;

    [Precision(13, 4)]
    public decimal? rate { get; set; }

    [Precision(13, 2)]
    public decimal? quantity { get; set; }

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public DateTime? created_at { get; set; }

    [ForeignKey("bill_id")]
    [InverseProperty("bill_items")]
    public virtual bill bill { get; set; } = null!;
}
