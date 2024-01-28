namespace FinanceDashboard.Shared.Models
{
    public record ExpenseData
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyName { get; set; }
        public int CurrencyId { get; set; }
        public string? ExpenseCategory { get; set; }
        public int ExpenseCategoryId { get; set; }
    };
}
