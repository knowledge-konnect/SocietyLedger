using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("code", Name = "flat_statuses_code_key", IsUnique = true)]
public partial class flat_status
{
    [Key]
    public short id { get; set; }

    public string code { get; set; } = null!;

    public string display_name { get; set; } = null!;

    [InverseProperty("status")]
    public virtual ICollection<flat> flats { get; set; } = new List<flat>();
}
