namespace FinanceDashboard.Server.Authentication
{
    public class UserAccount
    {
        public string UserName { get; set; } = null!;
        public string UserLogin { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
    }
}
