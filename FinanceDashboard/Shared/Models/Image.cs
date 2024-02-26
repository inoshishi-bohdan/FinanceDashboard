using System;
using System.Collections.Generic;

namespace FinanceDashboard.Shared.Models;

public partial class Image
{
    public int Id { get; set; }

    public string Path { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
