using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Table("maintenance_config")]
[Index("public_id", Name = "ux_maintenance_config_public_id", IsUnique = true)]
[Index("society_id", Name = "ux_maintenance_config_society_id", IsUnique = true)]
public partial class maintenance_config
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    [Precision(13, 2)]
    public decimal default_monthly_charge { get; set; }

    public int due_day_of_month { get; set; }

    [Precision(13, 2)]
    public decimal late_fee_per_month { get; set; }

    public int grace_period_days { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public long? created_by { get; set; }

    public long? updated_by { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("maintenance_configcreated_byNavigations")]
    public virtual user? created_byNavigation { get; set; }

    [ForeignKey("society_id")]
    [InverseProperty("maintenance_config")]
    public virtual society society { get; set; } = null!;

    [ForeignKey("updated_by")]
    [InverseProperty("maintenance_configupdated_byNavigations")]
    public virtual user? updated_byNavigation { get; set; }
}
