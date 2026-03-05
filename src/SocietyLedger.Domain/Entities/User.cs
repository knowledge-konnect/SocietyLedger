namespace SocietyLedger.Domain.Entities
{
    public class User
    {
        public long Id { get; set; }
        public Guid PublicId { get; set; }
        public long SocietyId { get; set; }
        public Guid SocietyPublicId { get; set; }
        public string SocietyName { get; set; }
        public string Name { get; set; } = null!;

        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Mobile { get; set; }
        public short RoleId { get; set; }    
        public Role Role { get; set; }       
        public string? PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;
        public bool ForcePasswordChange { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


    public class Role
    {
        public short Id { get; set; }         
        public string Code { get; set; } = null!;         
        public string DisplayName { get; set; } = null!;  
    }


    public class UserRole
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }


    public class RefreshToken
    {
        public long Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public string? ReplacedByToken { get; set; }
    }
}
