namespace FinanceDashboard.Shared
{
    public class UserSession
    {
        public string? UserName { get; set; }
        public string Token { get; set; } = null!;
        public string UserLogin { get; set; } = null!;
        public string Role { get; set; } = null!;

        public int ExpiresIn { get; set; }
        public DateTime ExpiryTimeStamp { get; set; }

    }
}
