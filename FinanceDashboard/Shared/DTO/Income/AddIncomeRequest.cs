using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceDashboard.Shared.DTO.Income
{
    public class AddIncomeRequest
    {
        public DateTime Date { get; set; }

        public string? Description { get; set; }

        public string? UserLogin { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
    }
}
