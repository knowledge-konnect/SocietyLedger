// This file previously contained a misplaced BillRepository stub in the wrong namespace.
// The FIFO allocation logic is now fully implemented in
// SocietyLedger.Infrastructure.Services.MaintenancePaymentService using inline Dapper queries
// inside a RepeatableRead transaction with FOR UPDATE row-level locks.
// No additional repository class is needed for this feature.