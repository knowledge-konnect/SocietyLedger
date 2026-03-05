using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("public_id", Name = "maintenance_components_public_id_key", IsUnique = true)]
public partial class maintenance_component
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public string name { get; set; } = null!;

    public string component_type { get; set; } = null!;

    [Precision(13, 2)]
    public decimal? default_amount { get; set; }

    [Precision(13, 4)]
    public decimal? default_rate_per_sqft { get; set; }

    public bool? is_mandatory { get; set; }

    public bool? is_deleted { get; set; }

    public DateTime? created_at { get; set; }

    [InverseProperty("maintenance_component")]
    public virtual ICollection<plan_component> plan_components { get; set; } = new List<plan_component>();

    [ForeignKey("society_id")]
    [InverseProperty("maintenance_components")]
    public virtual society society { get; set; } = null!;
}
