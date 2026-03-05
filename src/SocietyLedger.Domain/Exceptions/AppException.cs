namespace SocietyLedger.Domain.Exceptions
{
    /// <summary>
    /// Base exception class for application-specific exceptions.
    /// </summary>
    public class AppException : Exception
    {
        public string Code { get; set; }

        public AppException(string message, string code = "INTERNAL_SERVER_ERROR") 
            : base(message)
        {
            Code = code;
        }

        public AppException(string message, string code, Exception innerException) 
            : base(message, innerException)
        {
            Code = code;
        }
    }

    /// <summary>
    /// Exception for validation errors.
    /// </summary>
    public class ValidationException : AppException
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public ValidationException(string message, Dictionary<string, string[]>? errors = null)
            : base(message, "VALIDATION_FAILED")
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }

    /// <summary>
    /// Exception for authentication failures.
    /// </summary>
    public class AuthenticationException : AppException
    {
        public AuthenticationException(string message = "Invalid credentials")
            : base(message, "INVALID_CREDENTIALS")
        {
        }
    }

    /// <summary>
    /// Exception for authorization failures (insufficient permissions).
    /// </summary>
    public class AuthorizationException : AppException
    {
        public AuthorizationException(string message = "You do not have permission to perform this action")
            : base(message, "INSUFFICIENT_PERMISSIONS")
        {
        }
    }

    /// <summary>
    /// Exception for resource not found scenarios.
    /// </summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, string? identifier = null)
            : base(
                $"{resourceName} not found" + (identifier != null ? $" ({identifier})" : "."),
                "RESOURCE_NOT_FOUND"
            )
        {
        }
    }

    /// <summary>
    /// Exception for business logic conflicts.
    /// </summary>
    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, "RESOURCE_CONFLICT")
        {
        }
    }

    /// <summary>
    /// Exception for duplicate resources.
    /// </summary>
    public class DuplicateException : AppException
    {
        public DuplicateException(string resourceName, string fieldName)
            : base(
                $"A {resourceName} with this {fieldName} already exists.",
                "RESOURCE_ALREADY_EXISTS"
            )
        {
        }
    }
}
