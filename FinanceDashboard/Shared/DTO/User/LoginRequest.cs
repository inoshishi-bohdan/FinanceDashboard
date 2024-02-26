namespace FinanceDashboard.Shared.DTO.User
{
    public class LoginRequest
    {
        //[Required]
        //[EmailAddress]
        public string? UserLogin { get; set; }
        //[Required]
        public string? Password { get; set; }
    }
}
