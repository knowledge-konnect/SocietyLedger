using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocietyLedger.Shared
{
    public static class ApiConstants
    {
        // Versioning
        public const double API_VERSION_1_0 = 1.0;
        public const string VERSION_HEADER_NAME = "x-api-version";

        // Swagger
        public const bool SWAGGER_ENABLED = true;
        public const string SWAGGER_VERSION = "v1";
        public const string SWAGGER_TITLE = "Society Ledger API";
        public const string SWAGGER_DESCRIPTION = "API documentation for the Society Ledger platform.";
    }
}
