namespace SocietyLedger.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? SocietyName { get; set; }
        public string? SocietyAddress { get; set; }
    }
}
