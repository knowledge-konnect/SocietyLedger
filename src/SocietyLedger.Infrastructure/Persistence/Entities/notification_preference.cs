using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("public_id", Name = "ux_notification_preferences_public_id", IsUnique = true)]
[Index("user_id", Name = "ux_notification_preferences_user_id", IsUnique = true)]
public partial class notification_preference
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long user_id { get; set; }

    public bool payment_reminders { get; set; }

    public bool bill_generated { get; set; }

    public bool expense_updates { get; set; }

    public bool monthly_reports { get; set; }

    public DateTime updated_at { get; set; }

    [ForeignKey("user_id")]
    [InverseProperty("notification_preference")]
    public virtual user user { get; set; } = null!;
}
