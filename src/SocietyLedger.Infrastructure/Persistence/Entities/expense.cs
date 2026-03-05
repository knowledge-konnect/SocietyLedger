using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("society_id", "created_at", Name = "idx_expenses_society_date")]
[Index("society_id", "date_incurred", Name = "idx_expenses_society_month")]
[Index("public_id", Name = "ux_expenses_public_id", IsUnique = true)]
public partial class expense
{
    [Key]
    public long id { get; set; }

    public Guid public_id { get; set; }

    public long society_id { get; set; }

    public DateOnly date_incurred { get; set; }

    public string category_code { get; set; } = null!;

    public string? vendor { get; set; }

    public string? description { get; set; }

    [Precision(13, 2)]
    public decimal amount { get; set; }

    public long? attachment_id { get; set; }

    public long? approved_by { get; set; }

    public string status { get; set; } = null!;

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    [ForeignKey("approved_by")]
    [InverseProperty("expenseapproved_byNavigations")]
    public virtual user? approved_byNavigation { get; set; }

    [ForeignKey("created_by")]
    [InverseProperty("expensecreated_byNavigations")]
    public virtual user? created_byNavigation { get; set; }

    [ForeignKey("society_id")]
    [InverseProperty("expenses")]
    public virtual society society { get; set; } = null!;
}
