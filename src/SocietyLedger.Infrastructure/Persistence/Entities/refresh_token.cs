using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("user_id", "token_hash", Name = "ux_refresh_token_user_tokenhash", IsUnique = true)]
public partial class refresh_token
{
    [Key]
    public long id { get; set; }

    public long user_id { get; set; }

    public string token_hash { get; set; } = null!;

    public string? jwt_id { get; set; }

    public DateTime expires_at { get; set; }

    public DateTime created_at { get; set; }

    public string? created_by_ip { get; set; }

    public bool is_revoked { get; set; }

    public DateTime? revoked_at { get; set; }

    public string? replaced_by_token_hash { get; set; }

    [ForeignKey("user_id")]
    [InverseProperty("refresh_tokens")]
    public virtual user user { get; set; } = null!;
}
