namespace SocietyLedger.Application.DTOs.MaintenancePayment
{
    // =========================================================================
    //  REQUESTS
    // =========================================================================

    /// <summary>
    /// Request for processing a maintenance payment with idempotency.
    /// IdempotencyKey is optional here; the endpoint resolves it from the
    /// <c>Idempotency-Key</c> header, falling back to a generated UUID.
    /// </summary>
    public record MaintenancePaymentRequest(
        Guid     FlatPublicId,
        decimal  Amount,
        DateTime PaymentDate,
        string   PaymentModeCode,
        string?  ReferenceNumber,
        string?  ReceiptUrl,
        string?  Notes,
        string?  IdempotencyKey = null
    );

    /// <summary>
    /// Records a new maintenance payment for a flat.
    /// The amount is allocated FIFO to the oldest unpaid bills.
    /// Caller must supply an <c>Idempotency-Key</c> header.
    /// </summary>
    public record CreateMaintenancePaymentRequest(
        Guid     FlatPublicId,
        decimal  Amount,
        DateTime PaymentDate,
        string   PaymentModeCode,
        string?  ReferenceNumber,
        string?  ReceiptUrl,
        string?  Notes
    );

    /// <summary>
    /// All fields are optional — only supplied fields are updated.
    /// </summary>
    public record UpdateMaintenancePaymentRequest(
        decimal?  Amount,
        DateTime? PaymentDate,
        string?   PaymentModeCode,
        string?   ReferenceNumber,
        string?   ReceiptUrl,
        string?   Notes
    );

    // =========================================================================
    //  RESPONSES
    // =========================================================================

    /// <summary>
    /// Returned after a maintenance payment is processed.
    /// <list type="bullet">
    ///   <item><c>TotalPaid</c> — sum of all FIFO allocations made.</item>
    ///   <item><c>Allocations</c> — per-bill breakdown.</item>
    ///   <item><c>RemainingAdvance</c> — unallocated portion when payment exceeds dues.</item>
    /// </list>
    /// </summary>
    public record MaintenancePaymentResponse
    {
        // Identity — populated on reads; null on the create/allocate response
        public Guid?     PublicId        { get; init; }
        public Guid?     SocietyPublicId { get; init; }
        public Guid?     FlatPublicId    { get; init; }
        public string?   FlatNumber      { get; init; }

        // Payment details
        public decimal?  Amount          { get; init; }
        public DateTime? PaymentDate     { get; init; }
        public string?   PaymentModeName { get; init; }
        public string?   ReferenceNumber { get; init; }
        public string?   ReceiptUrl      { get; init; }
        public string?   Notes           { get; init; }
        public string?   RecordedByName  { get; init; }
        public DateTime? CreatedAt       { get; init; }

        // FIFO allocation summary
        public decimal                           TotalPaid        { get; init; }
        public List<MaintenancePaymentAllocation> Allocations     { get; init; } = [];
        public decimal                           RemainingAdvance { get; init; }
    }

    /// <summary>
    /// Single FIFO allocation line — mirrors one row in <c>bill_payment_allocations</c>.
    /// <c>Period</c> is the billing month (YYYY-MM) the allocation clears, used by the
    /// frontend to badge arrear payments vs current-month payments.
    /// </summary>
    public record MaintenancePaymentAllocation(
        Guid    BillPublicId,
        decimal AllocatedAmount,
        string? Period
    );

    /// <summary>Paginated / list wrapper for maintenance payment responses.</summary>
    public record ListMaintenancePaymentsResponse(
        List<MaintenancePaymentResponse> Payments
    )
    {
        public ListMaintenancePaymentsResponse() : this([]) { }
    }

    /// <summary>
    /// Collection-level summary for a billing period.
    /// <list type="bullet">
    ///   <item><c>TotalCharges</c>            — sum of all bills generated for the period.</item>
    ///   <item><c>TotalCollected</c>          — sum of payments allocated to bills for the period (excludes OB and advance).</item>
    ///   <item><c>BillOutstanding</c>         — remaining unpaid balance on bills for the period.</item>
    ///   <item><c>OpeningBalanceRemaining</c> — pre-system dues still owed across all flats.</item>
    ///   <item><c>TotalOutstanding</c>        — BillOutstanding + OpeningBalanceRemaining.</item>
    ///   <item><c>CollectionPercentage</c>    — TotalCollected / TotalCharges × 100.</item>
    /// </list>
    /// </summary>
    public record MaintenanceSummaryResponse(
        decimal TotalCharges,
        decimal TotalCollected,
        decimal BillOutstanding,
        decimal OpeningBalanceRemaining,
        decimal TotalOutstanding,
        decimal CollectionPercentage
    );

    /// <summary>Public-facing payment mode option.</summary>
    public record PaymentModeResponse(
        string Code,
        string DisplayName
    );

    /// <summary>List wrapper for payment mode options.</summary>
    public record ListPaymentModesResponse(
        List<PaymentModeResponse> Modes
    )
    {
        public ListPaymentModesResponse() : this([]) { }
    }

    // =========================================================================
    //  INTERNAL ENTITIES  (Dapper / repository projections — not EF tracked)
    // =========================================================================

    /// <summary>
    /// Flat maintenance payment row as returned by the repository layer.
    /// Used by <c>MapToResponse</c> inside <c>MaintenancePaymentService</c>.
    /// </summary>
    public record MaintenancePaymentEntity
    {
        public Guid     PublicId        { get; init; }
        public long     SocietyId       { get; init; }
        public Guid     SocietyPublicId { get; init; }
        public Guid     FlatPublicId    { get; init; }
        public string?  FlatNumber      { get; init; }
        public decimal  Amount          { get; init; }
        public DateTime PaymentDate     { get; init; }
        public short    PaymentModeId   { get; init; }
        public string?  PaymentModeName { get; init; }
        public string?  ReferenceNumber { get; init; }
        public string?  ReceiptUrl      { get; init; }
        public string?  Notes           { get; init; }
        public long?    RecordedBy      { get; init; }
        public string?  RecordedByName  { get; init; }
        public DateTime CreatedAt       { get; init; }
        // Bill info — null for advance / opening-balance rows
        public Guid?    BillPublicId    { get; init; }
        public string?  Period          { get; init; }
    }

    /// <summary>
    /// Payment mode row as returned by the repository layer.
    /// </summary>
    public record PaymentModeEntity(
        short  Id,
        string Code,
        string DisplayName
    );
}

