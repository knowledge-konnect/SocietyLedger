/**
 * Standardized alert messages for the application
 * - Uses simple, user-friendly language
 * - No sensitive information (IDs, emails, etc.)
 * - Consistent tone and formatting
 */

export const AlertMessages = {
  // ===== SUCCESS MESSAGES =====
  success: {
    // Authentication
    loginSuccess: 'Welcome back! You\'ve been logged in successfully.',
    logoutSuccess: 'You\'ve been logged out. See you soon!',
    signupSuccess: 'Your account has been created. Redirecting...',
    passwordChangeSuccess: 'Your password has been changed. Please log in again with your new password.',

    // User Management
    userCreated: 'User has been created successfully.',
    userUpdated: 'User information has been updated.',
    userDeleted: 'User has been removed from the system.',
    passwordCopied: 'Password has been copied to clipboard.',

    // Financial Management
    paymentCreated: 'Payment has been recorded successfully.',
    paymentUpdated: 'Payment information has been updated.',
    paymentDeleted: 'Payment has been removed.',
    expenseCreated: 'Expense has been recorded successfully.',
    expenseUpdated: 'Expense information has been updated.',
    expenseDeleted: 'Expense has been removed.',

    // Flat Management
    flatCreated: 'Flat has been added to the system.',
    flatUpdated: 'Flat information has been updated.',
    flatDeleted: 'Flat has been removed.',

    // Announcements & Documents
    announcementCreated: 'Announcement has been posted.',
    announcementUpdated: 'Announcement has been updated.',
    announcementDeleted: 'Announcement has been removed.',
    documentUploaded: 'Document has been uploaded successfully.',
    documentDeleted: 'Document has been removed.',

    // Settings
    settingsSaved: 'Your settings have been saved.',
    profileUpdated: 'Your profile has been updated.',
  },

  // ===== ERROR MESSAGES =====
  error: {
    // General errors
    somethingWentWrong: 'Something went wrong. Please try again.',
    unexpectedError: 'An unexpected error occurred. Please try again.',
    tryAgain: 'Please try again.',

    // Authentication errors
    invalidCredentials: 'Your email or password is incorrect. Please try again.',
    accountNotFound: 'This account doesn\'t exist. Please check and try again.',
    accountLocked: 'Your account has been locked. Please contact support.',
    sessionExpired: 'Your session has expired. Please log in again.',
    loginFailed: 'Login failed. Please try again.',
    logoutFailed: 'Unable to log out. Please try again.',
    signupFailed: 'Unable to create account. Please try again.',
    passwordChangeFailed: 'Unable to change password. Please try again.',
    passwordsDoNotMatch: 'The passwords you entered don\'t match.',

    // Validation errors
    fillAllFields: 'Please fill in all required fields.',
    invalidEmail: 'Please enter a valid email address.',
    invalidPhoneNumber: 'Please enter a valid phone number.',
    passwordTooShort: 'Password must be at least 6 characters long.',
    passwordTooWeak: 'Please choose a stronger password.',

    // User management errors
    userCreationFailed: 'Unable to create user. Please try again.',
    userUpdateFailed: 'Unable to update user information. Please try again.',
    userDeleteFailed: 'Unable to delete user. Please try again.',
    userNotFound: 'User not found.',

    // Financial management errors
    paymentFailed: 'Unable to record payment. Please try again.',
    paymentUpdateFailed: 'Unable to update payment. Please try again.',
    paymentDeleteFailed: 'Unable to delete payment. Please try again.',
    expenseFailed: 'Unable to record expense. Please try again.',
    expenseUpdateFailed: 'Unable to update expense. Please try again.',
    expenseDeleteFailed: 'Unable to delete expense. Please try again.',
    invalidAmount: 'Please enter a valid amount.',

    // Flat management errors
    flatCreationFailed: 'Unable to add flat. Please try again.',
    flatUpdateFailed: 'Unable to update flat information. Please try again.',
    flatDeleteFailed: 'Unable to delete flat. Please try again.',
    flatAlreadyExists: 'This flat already exists in the system.',

    // Announcements & Documents errors
    announcementFailed: 'Unable to create announcement. Please try again.',
    announcementUpdateFailed: 'Unable to update announcement. Please try again.',
    announcementDeleteFailed: 'Unable to delete announcement. Please try again.',
    documentUploadFailed: 'Unable to upload document. Please try again.',
    documentDeleteFailed: 'Unable to delete document. Please try again.',
    fileTooLarge: 'File is too large. Please choose a smaller file.',
    unsupportedFileType: 'This file type is not supported.',

    // Settings errors
    settingsSaveFailed: 'Unable to save settings. Please try again.',
    profileUpdateFailed: 'Unable to update profile. Please try again.',
  },

  // ===== WARNING MESSAGES =====
  warning: {
    unsavedChanges: 'You have unsaved changes. They will be lost if you leave this page.',
    confirmDelete: 'Are you sure you want to delete this? This action cannot be undone.',
    confirmLogout: 'Are you sure you want to log out?',
    passwordWarning: 'Save this password securely. You won\'t be able to see it again.',
    sessionAboutToExpire: 'Your session will expire soon. Please save your work.',
    noDataAvailable: 'No data available. Please try again later.',
    lowStorage: 'You\'re running low on storage space.',
  },

  // ===== INFO MESSAGES =====
  info: {
    loading: 'Loading...',
    processingRequest: 'Processing your request...',
    redirecting: 'Redirecting...',
    pleaseWait: 'Please wait...',
    contactSupport: 'Please contact support for assistance.',
    needHelp: 'Need help? Contact our support team.',
    noResultsFound: 'No results found. Try adjusting your search.',
    emptyList: 'Nothing to show here yet.',
    passwordRequirements: 'Password must be at least 6 characters long.',
  },
};

/**
 * Get a user-friendly message from error response
 * Filters out sensitive information like IDs, tokens, etc.
 */
export const sanitizeErrorMessage = (
  error: any,
  fallback: string = AlertMessages.error.somethingWentWrong
): string => {
  // Handle string errors
  if (typeof error === 'string') {
    return filterSensitiveInfo(error) || fallback;
  }

  // Handle error objects with message property
  if (error?.message) {
    const message = filterSensitiveInfo(error.message);
    if (message) return message;
  }

  // Handle API response errors
  if (error?.response?.data?.message) {
    const message = filterSensitiveInfo(error.response.data.message);
    if (message) return message;
  }

  return fallback;
};

/**
 * Filter out sensitive information from error messages
 * Removes: IDs, emails, phone numbers, tokens, etc.
 */
const filterSensitiveInfo = (message: string): string => {
  if (!message) return '';

  let filtered = message;

  // Remove common sensitive patterns
  filtered = filtered
    .replace(/[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}/g, '[email]') // Email addresses
    .replace(/\b\d{10}\b/g, '[phone]') // Phone numbers
    .replace(/\b[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\b/gi, '[id]') // UUIDs
    .replace(/\bid[=:]\s*["']?\d+["']?/gi, '[id]') // ID numbers
    .replace(/token[=:]\s*["']?[^"'\s]+["']?/gi, '[token]') // Tokens
    .replace(/\$\d+/g, '[amount]') // Amounts
    .replace(/Bearer\s+[^\s]+/gi, '[token]'); // Bearer tokens

  // If after filtering, the message is too generic or empty, return empty string
  if (filtered.includes('[') && filtered.length < 20) {
    return '';
  }

  return filtered.trim();
};

/**
 * Map common API error codes to user-friendly messages
 */
export const getMessageByErrorCode = (code: string): string => {
  const errorCodeMap: Record<string, string> = {
    'UNAUTHORIZED': AlertMessages.error.sessionExpired,
    'FORBIDDEN': 'You don\'t have permission to perform this action.',
    'NOT_FOUND': 'The resource you\'re looking for doesn\'t exist.',
    'VALIDATION_ERROR': AlertMessages.error.fillAllFields,
    'DUPLICATE_ENTRY': 'This entry already exists.',
    'INVALID_CREDENTIALS': AlertMessages.error.invalidCredentials,
    'ACCOUNT_LOCKED': AlertMessages.error.accountLocked,
    'SERVER_ERROR': AlertMessages.error.somethingWentWrong,
    'NETWORK_ERROR': 'Network connection error. Please check your internet.',
    'TIMEOUT': 'Request took too long. Please try again.',
  };

  return errorCodeMap[code] || AlertMessages.error.somethingWentWrong;
};
