namespace FinanceDashboard.Shared.DTO.Expense
{
    public class UpdateRequest
    {
        public int? Id { get; set; }
        public DateTime? Date { get; set; }
        public string? Description { get; set; }
        public int? ExpenseCategoryId { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyId { get; set; }

    }
}
