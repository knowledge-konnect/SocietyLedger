using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

public partial class subscription_event
{
    [Key]
    public Guid id { get; set; }

    public long user_id { get; set; }

    public Guid? subscription_id { get; set; }

    [StringLength(50)]
    public string event_type { get; set; } = null!;

    [StringLength(20)]
    public string? old_status { get; set; }

    [StringLength(20)]
    public string? new_status { get; set; }

    [Precision(10, 2)]
    public decimal? amount { get; set; }

    [Column(TypeName = "jsonb")]
    public string? metadata { get; set; }

    public DateTime? created_at { get; set; }

    [ForeignKey("subscription_id")]
    [InverseProperty("subscription_events")]
    public virtual subscription? subscription { get; set; }

    [ForeignKey("user_id")]
    [InverseProperty("subscription_events")]
    public virtual user user { get; set; } = null!;
}
