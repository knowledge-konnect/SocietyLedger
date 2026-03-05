namespace SocietyLedger.Shared
{
    /// <summary>
    /// Represents a single error detail with error code and message.
    /// </summary>
    public class ErrorDetail
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents field-level validation errors.
    /// </summary>
    public class FieldError
    {
        public string Field { get; set; } = string.Empty;
        public string[] Messages { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Standardized error response structure for API responses.
    /// </summary>
    public class ErrorResponse
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<FieldError> Errors { get; set; } = new();
        public string? TraceId { get; set; }

        public static ErrorResponse Create(string code, string message, string? traceId = null)
            => new() { Code = code, Message = message, TraceId = traceId };

        public static ErrorResponse CreateWithFields(string code, string message, List<FieldError> errors, string? traceId = null)
            => new() { Code = code, Message = message, Errors = errors, TraceId = traceId };
    }
}
