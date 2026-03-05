using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("code", Name = "roles_code_key", IsUnique = true)]
public partial class role
{
    [Key]
    public short id { get; set; }

    public string code { get; set; } = null!;

    public string display_name { get; set; } = null!;

    [InverseProperty("role")]
    public virtual ICollection<user> users { get; set; } = new List<user>();
}
