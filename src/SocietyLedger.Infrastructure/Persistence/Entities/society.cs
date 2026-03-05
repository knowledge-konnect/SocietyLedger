using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("public_id", Name = "idx_societies_public_id")]
[Index("public_id", Name = "ux_societies_public_id", IsUnique = true)]
public partial class society
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public string name { get; set; } = null!;

    public string? address { get; set; }

    public string? city { get; set; }

    public string? state { get; set; }

    public string? pincode { get; set; }

    public string currency { get; set; } = null!;

    public string default_maintenance_cycle { get; set; } = null!;

    public int? billing_plan_id { get; set; }

    [Column(TypeName = "jsonb")]
    public string? settings { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public short? maintenance_cycle_id { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    public DateOnly onboarding_date { get; set; }

    [InverseProperty("society")]
    public virtual ICollection<adjustment> adjustments { get; set; } = new List<adjustment>();

    [InverseProperty("society")]
    public virtual ICollection<attachment> attachments { get; set; } = new List<attachment>();

    [InverseProperty("society")]
    public virtual ICollection<bill> bills { get; set; } = new List<bill>();

    [InverseProperty("society")]
    public virtual ICollection<expense> expenses { get; set; } = new List<expense>();

    [InverseProperty("society")]
    public virtual ICollection<flat> flats { get; set; } = new List<flat>();

    [InverseProperty("society")]
    public virtual ICollection<maintenance_component> maintenance_components { get; set; } = new List<maintenance_component>();

    [InverseProperty("society")]
    public virtual maintenance_config? maintenance_config { get; set; }

    [ForeignKey("maintenance_cycle_id")]
    [InverseProperty("societies")]
    public virtual maintenance_cycle? maintenance_cycle { get; set; }

    [InverseProperty("society")]
    public virtual ICollection<maintenance_payment> maintenance_payments { get; set; } = new List<maintenance_payment>();

    [InverseProperty("society")]
    public virtual ICollection<maintenance_plan> maintenance_plans { get; set; } = new List<maintenance_plan>();

    [InverseProperty("society")]
    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    [InverseProperty("society")]
    public virtual ICollection<user> users { get; set; } = new List<user>();
}
