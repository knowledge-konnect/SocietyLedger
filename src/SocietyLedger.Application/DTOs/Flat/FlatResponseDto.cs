using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Application.DTOs.Flat
{
    public record FlatResponseDto(
     Guid PublicId,
     Guid SocietyPublicId,
     string FlatNo,
     string? OwnerName,
     string? ContactMobile,
     string? ContactEmail,
     decimal MaintenanceAmount,
     short? StatusId,
     string StatusName,
     DateTime CreatedAt,
     DateTime UpdatedAt
 );
}
