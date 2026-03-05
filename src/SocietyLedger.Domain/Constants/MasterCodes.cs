namespace SocietyLedger.Domain.Constants
{
    /// <summary>
    /// Centralized master data codes for role-based access control.
    /// Maps to roles.code in database.
    /// </summary>
    public static class RoleCodes
    {
        public const string SuperAdmin = "super_admin";
        public const string Admin = "admin";
        public const string Treasurer = "treasurer";
        public const string Secretary = "secretary";
        public const string Manager = "manager";
        public const string Viewer = "viewer";
        public const string SocietyAdmin = "society_admin";
    }

    /// <summary>
    /// Centralized bill status codes.
    /// Maps to bill_statuses.code in database.
    /// </summary>
    public static class BillStatusCodes
    {
        public const string Unpaid = "unpaid";
        public const string Partial = "partial";
        public const string Paid = "paid";
        public const string Overdue = "overdue";
        public const string Cancelled = "cancelled";
    }

    /// <summary>
    /// Centralized expense category codes.
    /// Maps to expense_categories.code in database.
    /// </summary>
    public static class ExpenseCategoryCodes
    {
        public const string Electricity = "electricity";
        public const string Water = "water";
        public const string Security = "security";
        public const string Housekeeping = "housekeeping";
        public const string Maintenance = "maintenance";
        public const string Repair = "repair";
        public const string Lift = "lift";
        public const string Generator = "generator";
        public const string Cleaning = "cleaning";
        public const string Garden = "garden";
        public const string Salary = "salary";
        public const string Stationery = "stationery";
        public const string Insurance = "insurance";
        public const string Tax = "tax";
        public const string Others = "others";
    }

    /// <summary>
    /// Centralized payment mode codes.
    /// Maps to payment_modes.code in database.
    /// </summary>
    public static class PaymentModeCodes
    {
        public const string Cash = "cash";
        public const string Upi = "upi";
        public const string BankTransfer = "bank_transfer";
        public const string Cheque = "cheque";
        public const string Razorpay = "razorpay";
        public const string Card = "card";
        public const string Netbanking = "netbanking";
        public const string Other = "other";
    }

    /// <summary>
    /// Centralized flat status codes.
    /// Maps to flat_statuses.code in database.
    /// </summary>
    public static class FlatStatusCodes
    {
        public const string OwnerOccupied = "owner_occupied";
        public const string TenantOccupied = "tenant_occupied";
        public const string Vacant = "vacant";
        public const string UnderMaintenance = "under_maintenance";
    }

    /// <summary>
    /// Centralized maintenance cycle codes.
    /// Maps to maintenance_cycles.code in database.
    /// </summary>
    public static class MaintenanceCycleCodes
    {
        public const string Monthly = "monthly";
        public const string Quarterly = "quarterly";
        public const string HalfYearly = "half_yearly";
        public const string Yearly = "yearly";
    }

    /// <summary>
    /// Centralized payment type codes.
    /// Maps to payments.payment_type CHECK constraint in database.
    /// </summary>
    public static class PaymentTypeCodes
    {
        public const string Bill = "bill";
        public const string Subscription = "subscription";
    }

    /// <summary>
    /// Centralized subscription status codes.
    /// Maps to subscriptions.status CHECK constraint in database.
    /// </summary>
    public static class SubscriptionStatusCodes
    {
        public const string Trial = "trial";
        public const string Active = "active";
        public const string Expired = "expired";
        public const string PastDue = "past_due";
        public const string Cancelled = "cancelled";
    }

    /// <summary>
    /// Centralized invoice status codes.
    /// Maps to invoices.status CHECK constraint in database.
    /// </summary>
    public static class InvoiceStatusCodes
    {
        public const string Draft = "draft";
        public const string Pending = "pending";
        public const string Paid = "paid";
        public const string Failed = "failed";
        public const string Cancelled = "cancelled";
        public const string Refunded = "refunded";
    }

    /// <summary>
    /// Centralized entry type codes for adjustments.
    /// Used in adjustments.entry_type column.
    /// </summary>
    public static class EntryTypeCodes
    {
        public const string MonthlyMaintenance = "monthly_maintenance";
        public const string OpeningBalance = "opening_balance";
        public const string OpeningFund = "opening_fund";
    }
}
