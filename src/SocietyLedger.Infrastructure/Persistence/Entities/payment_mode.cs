using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

public partial class payment_mode
{
    [Key]
    public short id { get; set; }

    public string code { get; set; } = null!;

    public string display_name { get; set; } = null!;

    [InverseProperty("payment_mode")]
    public virtual ICollection<maintenance_payment> maintenance_payments { get; set; } = new List<maintenance_payment>();
}
