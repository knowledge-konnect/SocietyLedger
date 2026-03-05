using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("maintenance_plan_id", Name = "idx_plan_components_plan")]
[Index("public_id", Name = "plan_components_public_id_key", IsUnique = true)]
public partial class plan_component
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long maintenance_plan_id { get; set; }

    public long maintenance_component_id { get; set; }

    [Precision(13, 2)]
    public decimal? amount { get; set; }

    [Precision(13, 4)]
    public decimal? rate_per_sqft { get; set; }

    public DateTime? created_at { get; set; }

    [ForeignKey("maintenance_component_id")]
    [InverseProperty("plan_components")]
    public virtual maintenance_component maintenance_component { get; set; } = null!;

    [ForeignKey("maintenance_plan_id")]
    [InverseProperty("plan_components")]
    public virtual maintenance_plan maintenance_plan { get; set; } = null!;
}
