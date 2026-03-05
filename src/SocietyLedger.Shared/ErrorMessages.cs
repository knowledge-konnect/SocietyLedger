namespace SocietyLedger.Shared
{
    /// <summary>
    /// Standardized error messages for the API.
    /// All messages are user-friendly with no sensitive information.
    /// </summary>
    public static class ErrorMessages
    {
        // Validation Errors
        public const string VALIDATION_FAILED = "One or more validation errors occurred.";
        public const string INVALID_REQUEST = "The request contains invalid data.";
        public const string MISSING_REQUIRED_FIELD = "A required field is missing.";

        // Authentication & Authorization
        public const string UNAUTHORIZED = "You are not authorized to access this resource.";
        public const string INVALID_CREDENTIALS = "Invalid username, email, or password.";
        public const string TOKEN_EXPIRED = "Your session has expired. Please log in again.";
        public const string INVALID_TOKEN = "The token is invalid or has expired.";
        public const string INSUFFICIENT_PERMISSIONS = "You do not have permission to perform this action.";

        // User & Account
        public const string USER_NOT_FOUND = "User not found.";
        public const string USER_INACTIVE = "This user account is inactive.";
        public const string EMAIL_ALREADY_EXISTS = "This email address is already registered.";
        public const string USERNAME_ALREADY_EXISTS = "This username is already taken.";
        public const string INCORRECT_PASSWORD = "The current password is incorrect.";
        public const string PASSWORD_CHANGE_FAILED = "Failed to change password. Please try again.";

        // Resource Management
        public const string RESOURCE_NOT_FOUND = "The requested resource was not found.";
        public const string RESOURCE_ALREADY_EXISTS = "This resource already exists.";
        public const string RESOURCE_CONFLICT = "The operation cannot be completed due to a conflict.";

        // Business Logic
        public const string INVALID_OPERATION = "This operation cannot be completed.";
        public const string INSUFFICIENT_DATA = "Required data is missing or incomplete.";

        // Server Errors
        public const string INTERNAL_SERVER_ERROR = "An unexpected error occurred. Please try again later.";
        public const string SERVICE_UNAVAILABLE = "The service is temporarily unavailable. Please try again later.";
    }

    /// <summary>
    /// Error codes for standardized error response identification.
    /// </summary>
    public static class ErrorCodes
    {
        // Validation
        public const string VALIDATION_FAILED = "VALIDATION_FAILED";
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        
        public const string INVALID_REQUEST = "INVALID_REQUEST";
        public const string MISSING_REQUIRED_FIELD = "MISSING_REQUIRED_FIELD";

        // Authentication
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string TOKEN_EXPIRED = "TOKEN_EXPIRED";
        public const string INVALID_TOKEN = "INVALID_TOKEN";

        // Authorization
        public const string FORBIDDEN = "FORBIDDEN";
        public const string INSUFFICIENT_PERMISSIONS = "INSUFFICIENT_PERMISSIONS";

        // User & Account
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string USER_INACTIVE = "USER_INACTIVE";
        public const string EMAIL_ALREADY_EXISTS = "EMAIL_ALREADY_EXISTS";
        public const string USERNAME_ALREADY_EXISTS = "USERNAME_ALREADY_EXISTS";
        public const string INCORRECT_PASSWORD = "INCORRECT_PASSWORD";
        public const string PASSWORD_CHANGE_FAILED = "PASSWORD_CHANGE_FAILED";

        // Resource
        public const string RESOURCE_NOT_FOUND = "RESOURCE_NOT_FOUND";
        public const string RESOURCE_ALREADY_EXISTS = "RESOURCE_ALREADY_EXISTS";
        public const string RESOURCE_CONFLICT = "RESOURCE_CONFLICT";

        // Business Logic
        public const string INVALID_OPERATION = "INVALID_OPERATION";
        public const string INSUFFICIENT_DATA = "INSUFFICIENT_DATA";

        // Server
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string SERVICE_UNAVAILABLE = "SERVICE_UNAVAILABLE";
    }
}
