namespace SocietyLedger.Application.DTOs.Auth
{
    public class ChangePasswordResponse
    {
        public string Message { get; set; } = null!;
        public bool ForcePasswordChange { get; set; }
    }
}
