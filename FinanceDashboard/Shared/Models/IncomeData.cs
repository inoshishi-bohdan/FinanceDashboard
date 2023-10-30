using FinanceDashboard.Server.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceDashboard.Shared.Models
{
    public record IncomeData
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? CurrencyName { get; set; }
        public int CurrencyId { get; set; }
    }
}
