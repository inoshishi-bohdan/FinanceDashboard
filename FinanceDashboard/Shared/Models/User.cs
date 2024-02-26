namespace FinanceDashboard.Shared.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public int? ImageId { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual Image? Image { get; set; }

    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    public virtual Role Role { get; set; } = null!;
}
