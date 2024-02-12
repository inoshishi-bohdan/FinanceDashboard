namespace FinanceDashboard.Shared.DTO.Expense
{
    public class CreateRequest
    {
        public DateTime? Date { get; set; }

        public string? Description { get; set; }

        public int? ExpenseCategoryId { get; set; }

        public string? UserLogin { get; set; }

        public decimal? Amount { get; set; }

        public int? CurrencyId { get; set; }
    }
}
