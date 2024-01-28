namespace FinanceDashboard.Shared.DTO.Expense
{
    public class StatGetExpensesRequest
    {
        public string? UserLogin { get; set; }
        public int CurrencyId { get; set; }
    }
}
