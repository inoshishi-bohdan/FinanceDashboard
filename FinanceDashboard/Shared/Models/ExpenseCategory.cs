using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanceDashboard.Server.Model;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
