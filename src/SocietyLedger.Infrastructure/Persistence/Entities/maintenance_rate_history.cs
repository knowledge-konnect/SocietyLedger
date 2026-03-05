using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("maintenance_rate_history")]
[Index("public_id", Name = "maintenance_rate_history_public_id_key", IsUnique = true)]
public partial class maintenance_rate_history
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long maintenance_plan_id { get; set; }

    [Precision(13, 2)]
    public decimal? old_fixed_amount { get; set; }

    [Precision(13, 4)]
    public decimal? old_rate_per_sqft { get; set; }

    public long? changed_by { get; set; }

    public DateTime? changed_at { get; set; }

    [ForeignKey("maintenance_plan_id")]
    [InverseProperty("maintenance_rate_histories")]
    public virtual maintenance_plan maintenance_plan { get; set; } = null!;
}
