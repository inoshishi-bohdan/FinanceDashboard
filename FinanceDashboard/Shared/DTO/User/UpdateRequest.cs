namespace FinanceDashboard.Shared.DTO.User
{
    public class UpdateRequest
    {
        public string? Login { get; set; }
        public string? NewName { get; set; }
        public string? NewLogin { get; set; }
        public string? NewUserImagePath { get; set; }
        public string? NewPassword { get; set; }
        public string? RepeatedPassword { get; set; }
    }
}
