using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("code", Name = "maintenance_cycles_code_key", IsUnique = true)]
public partial class maintenance_cycle
{
    [Key]
    public short id { get; set; }

    public string code { get; set; } = null!;

    public string display_name { get; set; } = null!;

    public string? description { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    [InverseProperty("maintenance_cycle")]
    public virtual ICollection<society> societies { get; set; } = new List<society>();
}
