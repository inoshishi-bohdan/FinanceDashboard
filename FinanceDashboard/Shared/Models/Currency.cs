using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinanceDashboard.Server.Model;

public partial class Currency
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    [JsonIgnore]
    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();
}
