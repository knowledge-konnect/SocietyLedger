using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("status", Name = "idx_subscriptions_status")]
[Index("trial_end", Name = "idx_subscriptions_trial_end")]
[Index("user_id", Name = "idx_subscriptions_user_id")]
public partial class subscription
{
    [Key]
    public Guid id { get; set; }

    public long user_id { get; set; }

    public Guid plan_id { get; set; }

    [StringLength(20)]
    public string status { get; set; } = null!;

    [Precision(10, 2)]
    public decimal subscribed_amount { get; set; }

    [StringLength(3)]
    public string? currency { get; set; }

    public DateTime? current_period_start { get; set; }

    public DateTime? current_period_end { get; set; }

    public DateTime? trial_start { get; set; }

    public DateTime? trial_end { get; set; }

    public DateTime? cancelled_at { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    [InverseProperty("subscription")]
    public virtual ICollection<invoice> invoices { get; set; } = new List<invoice>();

    [ForeignKey("plan_id")]
    [InverseProperty("subscriptions")]
    public virtual plan plan { get; set; } = null!;

    [InverseProperty("subscription")]
    public virtual ICollection<subscription_event> subscription_events { get; set; } = new List<subscription_event>();

    [ForeignKey("user_id")]
    [InverseProperty("subscriptions")]
    public virtual user user { get; set; } = null!;
}
