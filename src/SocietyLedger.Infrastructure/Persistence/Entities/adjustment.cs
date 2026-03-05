using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("society_id", Name = "idx_adjustments_society")]
[Index("society_id", "entry_type", "period", Name = "idx_adjustments_society_type_period")]
[Index("public_id", Name = "ux_adjustments_public_id", IsUnique = true)]
public partial class adjustment
{
    [Key]
    public long id { get; set; }

    public long society_id { get; set; }

    public long? flat_id { get; set; }

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public string? reason { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public string entry_type { get; set; } = null!;

    public string? period { get; set; }

    public Guid public_id { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    [Precision(13, 2)]
    public decimal remaining_amount { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("adjustments")]
    public virtual user? created_byNavigation { get; set; }

    [ForeignKey("flat_id")]
    [InverseProperty("adjustments")]
    public virtual flat? flat { get; set; }

    [InverseProperty("adjustment")]
    public virtual ICollection<maintenance_payment> maintenance_payments { get; set; } = new List<maintenance_payment>();

    [ForeignKey("society_id")]
    [InverseProperty("adjustments")]
    public virtual society society { get; set; } = null!;
}
