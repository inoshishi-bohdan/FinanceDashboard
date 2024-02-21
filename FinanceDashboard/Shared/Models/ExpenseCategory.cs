using System.Text.Json.Serialization;

namespace FinanceDashboard.Shared.Models;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
