using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

public partial class expense_category
{
    [Key]
    public short id { get; set; }

    public string code { get; set; } = null!;

    public string display_name { get; set; } = null!;
}
