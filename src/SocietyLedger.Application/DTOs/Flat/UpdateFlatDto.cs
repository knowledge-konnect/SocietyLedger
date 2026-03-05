using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.DTOs.Flat
{
    public record UpdateFlatDto(
        Guid PublicId,
        string? FlatNo,
        string? OwnerName,
        string? ContactMobile,
        string? ContactEmail,
        decimal? MaintenanceAmount,
        string? StatusCode
    );
}
