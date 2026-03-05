using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("society_id", Name = "idx_users_society_id")]
[Index("username", Name = "users_username_key", IsUnique = true)]
[Index("public_id", Name = "ux_users_public_id", IsUnique = true)]
public partial class user
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public string name { get; set; } = null!;

    public string? email { get; set; }

    public string? mobile { get; set; }

    public short role_id { get; set; }

    public string? password_hash { get; set; }

    public bool is_active { get; set; }

    public DateTime? last_login { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public bool force_password_change { get; set; }

    public Guid? created_by { get; set; }

    public Guid? updated_by { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? trial_start_date { get; set; }

    [StringLength(20)]
    public string? subscription_status { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? subscription_start_date { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? next_billing_date { get; set; }

    public DateTime? trial_ends_date { get; set; }

    public DateTime? last_payment_date { get; set; }

    [StringLength(50)]
    public string? subscription_plan { get; set; }

    [Precision(10, 2)]
    public decimal? monthly_amount { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    [StringLength(100)]
    public string? username { get; set; }

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<adjustment> adjustments { get; set; } = new List<adjustment>();

    [InverseProperty("uploaded_byNavigation")]
    public virtual ICollection<attachment> attachments { get; set; } = new List<attachment>();

    [InverseProperty("generated_byNavigation")]
    public virtual ICollection<bill> bills { get; set; } = new List<bill>();

    [InverseProperty("approved_byNavigation")]
    public virtual ICollection<expense> expenseapproved_byNavigations { get; set; } = new List<expense>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<expense> expensecreated_byNavigations { get; set; } = new List<expense>();

    [InverseProperty("user")]
    public virtual ICollection<invoice> invoices { get; set; } = new List<invoice>();

    [InverseProperty("created_byNavigation")]
    public virtual ICollection<maintenance_config> maintenance_configcreated_byNavigations { get; set; } = new List<maintenance_config>();

    [InverseProperty("updated_byNavigation")]
    public virtual ICollection<maintenance_config> maintenance_configupdated_byNavigations { get; set; } = new List<maintenance_config>();

    [InverseProperty("recorded_byNavigation")]
    public virtual ICollection<maintenance_payment> maintenance_payments { get; set; } = new List<maintenance_payment>();

    [InverseProperty("user")]
    public virtual notification_preference? notification_preference { get; set; }

    [InverseProperty("recorded_byNavigation")]
    public virtual ICollection<payment> payments { get; set; } = new List<payment>();

    [InverseProperty("user")]
    public virtual ICollection<refresh_token> refresh_tokens { get; set; } = new List<refresh_token>();

    [ForeignKey("role_id")]
    [InverseProperty("users")]
    public virtual role role { get; set; } = null!;

    [ForeignKey("society_id")]
    [InverseProperty("users")]
    public virtual society society { get; set; } = null!;

    [InverseProperty("user")]
    public virtual ICollection<subscription_event> subscription_events { get; set; } = new List<subscription_event>();

    [InverseProperty("user")]
    public virtual ICollection<subscription> subscriptions { get; set; } = new List<subscription>();
}
