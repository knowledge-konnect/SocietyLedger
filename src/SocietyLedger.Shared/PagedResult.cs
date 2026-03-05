namespace SocietyLedger.Shared
{
    /// <summary>
    /// Generic pagination envelope returned by all paginated repository and service methods.
    /// Immutable — constructed once after the DB round-trip and never mutated.
    /// </summary>
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items         { get; }
        public long             TotalCount    { get; }
        public int              Page          { get; }
        public int              PageSize      { get; }
        public int              TotalPages    { get; }
        public bool             HasNextPage   { get; }
        public bool             HasPreviousPage { get; }

        public PagedResult(IReadOnlyList<T> items, long totalCount, int page, int pageSize)
        {
            Items           = items;
            TotalCount      = totalCount;
            Page            = page;
            PageSize        = pageSize;
            TotalPages      = pageSize > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0;
            HasNextPage     = page < TotalPages;
            HasPreviousPage = page > 1;
        }

        /// <summary>
        /// Returns an empty page. Useful as a safe default to avoid null checks at call sites.
        /// </summary>
        public static PagedResult<T> Empty(int page, int pageSize) =>
            new(Array.Empty<T>(), 0L, page, pageSize);
    }
}
