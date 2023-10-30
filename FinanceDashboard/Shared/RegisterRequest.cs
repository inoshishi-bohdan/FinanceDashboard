using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceDashboard.Shared
{
    public class RegisterRequest
    {
        public string? Name { get; set; }

        public string? Surname { get; set; }

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

    }
}
