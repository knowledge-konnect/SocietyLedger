namespace SocietyLedger.Domain.Entities
{
    /// <summary>
    /// Society aggregate root following DDD best practices.
    /// Inherits common properties (Id, PublicId, audit fields) from BaseEntity.
    /// </summary>
    public class Society : BaseEntity
    {
        // Domain properties with protected setters for encapsulation
        public string Name { get; private set; } = null!;   
        public string? Address { get; private set; }        
        public string? City { get; private set; }           
        public string? State { get; private set; }         
        public string? Country { get; private set; }      
        public string? Pincode { get; private set; }

        /// <summary>
        /// The date the society was onboarded. No ledger transaction_date may be
        /// earlier than this value — represents the financial epoch for this society.
        /// </summary>
        public DateOnly OnboardingDate { get; private set; }

        // Navigation properties
        private readonly List<User> _users = new();
        public IReadOnlyCollection<User> Users => _users.AsReadOnly();

        /// <summary>
        /// Private constructor for EF Core.
        /// </summary>
        private Society() : base()
        {
        }

        /// <summary>
        /// Factory method to create a new Society (DDD pattern).
        /// </summary>
        public static Society Create(
            string name,
            string? address = null,
            string? city = null,
            string? state = null,
            string? country = null,
            string? pincode = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Society name cannot be empty.", nameof(name));

            var society = new Society
            {
                Name = name,
                Address = address,
                City = city,
                State = state,
                Country = country,
                Pincode = pincode
            };

            return society;
        }

        /// <summary>
        /// Updates society information.
        /// </summary>
        public void Update(
            string name,
            string? address = null,
            string? city = null,
            string? state = null,
            string? country = null,
            string? pincode = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Society name cannot be empty.", nameof(name));

            Name = name;
            Address = address;
            City = city;
            State = state;
            Country = country;
            Pincode = pincode;

            MarkAsUpdated();
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> when <paramref name="transactionDate"/>
        /// is earlier than <see cref="OnboardingDate"/>.  Call this from any service layer
        /// method that creates a <c>society_fund_ledger</c> row.
        /// </summary>
        public void EnsureLedgerDateAllowed(DateOnly transactionDate, string fieldName = "transaction_date")
        {
            if (transactionDate < OnboardingDate)
                throw new ArgumentException(
                    $"'{fieldName}' ({transactionDate:yyyy-MM-dd}) cannot be earlier than the society " +
                    $"onboarding date ({OnboardingDate:yyyy-MM-dd}).",
                    fieldName);
        }

        /// <summary>
        /// Adds a user to the society. Domain method to enforce business rules.
        /// </summary>
        public void AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (_users.Any(u => u.Id == user.Id))
                throw new InvalidOperationException("User already exists in this society.");

            _users.Add(user);
            MarkAsUpdated();
        }

        /// <summary>
        /// Removes a user from the society.
        /// </summary>
        public void RemoveUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _users.Remove(user);
            MarkAsUpdated();
        }
    }
}
