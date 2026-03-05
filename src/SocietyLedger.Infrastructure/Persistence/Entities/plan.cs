using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("name", Name = "ux_plans_name", IsUnique = true)]
public partial class plan
{
    [Key]
    public Guid id { get; set; }

    [StringLength(100)]
    public string name { get; set; } = null!;

    [Precision(10, 2)]
    public decimal monthly_amount { get; set; }

    [StringLength(3)]
    public string currency { get; set; } = null!;

    public bool? is_active { get; set; }

    public DateTime? created_at { get; set; }

    public int duration_months { get; set; }

    [InverseProperty("plan")]
    public virtual ICollection<subscription> subscriptions { get; set; } = new List<subscription>();
}
