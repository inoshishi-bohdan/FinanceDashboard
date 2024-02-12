﻿namespace FinanceDashboard.Shared.Models
{
    public record IncomeData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public int CurrencyId { get; set; }
    }
}
