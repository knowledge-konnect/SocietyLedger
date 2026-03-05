namespace SocietyLedger.Application.DTOs.Auth
{
    public class RegisterResponse : LoginResponse
    {
        public long? SocietyId { get; set; }
        public long? UserId { get; set; }
    }
}
