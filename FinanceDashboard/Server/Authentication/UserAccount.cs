namespace FinanceDashboard.Server.Authentication
{
    public class UserAccount
    {
        public string? UserName { get; set; }
        public string UserLogin { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
