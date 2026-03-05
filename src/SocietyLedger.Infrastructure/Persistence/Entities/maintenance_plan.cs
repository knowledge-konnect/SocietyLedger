using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("public_id", Name = "maintenance_plans_public_id_key", IsUnique = true)]
public partial class maintenance_plan
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public string name { get; set; } = null!;

    public string calculation_type { get; set; } = null!;

    [Precision(13, 2)]
    public decimal? fixed_amount { get; set; }

    [Precision(13, 4)]
    public decimal? rate_per_sqft { get; set; }

    public DateOnly effective_from { get; set; }

    public DateOnly? effective_to { get; set; }

    public bool? is_active { get; set; }

    public bool? is_deleted { get; set; }

    public DateTime? created_at { get; set; }

    [InverseProperty("maintenance_plan")]
    public virtual ICollection<bill> bills { get; set; } = new List<bill>();

    [InverseProperty("maintenance_plan")]
    public virtual ICollection<maintenance_rate_history> maintenance_rate_histories { get; set; } = new List<maintenance_rate_history>();

    [InverseProperty("maintenance_plan")]
    public virtual ICollection<plan_component> plan_components { get; set; } = new List<plan_component>();

    [ForeignKey("society_id")]
    [InverseProperty("maintenance_plans")]
    public virtual society society { get; set; } = null!;
}
