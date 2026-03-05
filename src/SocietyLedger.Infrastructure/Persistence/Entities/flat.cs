using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("society_id", "flat_no", Name = "flats_society_id_flat_no_key", IsUnique = true)]
[Index("society_id", Name = "idx_flats_society_id")]
[Index("public_id", Name = "ux_flats_public_id", IsUnique = true)]
public partial class flat
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public string flat_no { get; set; } = null!;

    public string? owner_name { get; set; }

    public string? contact_mobile { get; set; }

    public string? contact_email { get; set; }

    [Precision(13, 2)]
    public decimal maintenance_amount { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public short? status_id { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    [Precision(10, 2)]
    public decimal? area_sqft { get; set; }

    [Precision(13, 2)]
    public decimal advance_balance { get; set; }

    [Precision(13, 2)]
    public decimal opening_balance { get; set; }

    [InverseProperty("flat")]
    public virtual ICollection<adjustment> adjustments { get; set; } = new List<adjustment>();

    [InverseProperty("flat")]
    public virtual ICollection<bill> bills { get; set; } = new List<bill>();

    [InverseProperty("flat")]
    public virtual ICollection<maintenance_payment> maintenance_payments { get; set; } = new List<maintenance_payment>();

    [InverseProperty("flat")]
    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    [ForeignKey("society_id")]
    [InverseProperty("flats")]
    public virtual society society { get; set; } = null!;

    [ForeignKey("status_id")]
    [InverseProperty("flats")]
    public virtual flat_status? status { get; set; }
}
