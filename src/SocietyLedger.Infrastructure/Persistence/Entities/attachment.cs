using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SocietyLedger.Infrastructure.Persistence.Entities;

[Index("society_id", Name = "idx_attachments_society")]
[Index("public_id", Name = "ux_attachments_public_id", IsUnique = true)]
public partial class attachment
{
    [Key]
    public long id { get; set; }

    public long society_id { get; set; }

    public string object_key { get; set; } = null!;

    public string? file_name { get; set; }

    public string? mime_type { get; set; }

    public long? file_size { get; set; }

    public string? checksum { get; set; }

    public long? uploaded_by { get; set; }

    public DateTime created_at { get; set; }

    public Guid public_id { get; set; }

    public bool is_deleted { get; set; }

    public DateTime? deleted_at { get; set; }

    [ForeignKey("society_id")]
    [InverseProperty("attachments")]
    public virtual society society { get; set; } = null!;

    [ForeignKey("uploaded_by")]
    [InverseProperty("attachments")]
    public virtual user? uploaded_byNavigation { get; set; }
}
