using System;
using System.Collections.Generic;

namespace FinanceDashboard.Server.Model;

public partial class Expense
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string? Description { get; set; }

    public int ExpenseCategoryId { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public int CurrencyId { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
