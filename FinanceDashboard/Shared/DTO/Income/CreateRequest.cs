namespace FinanceDashboard.Shared.DTO.Income
{
    public class CreateRequest
    {
        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public string? UserLogin { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
    }
}
