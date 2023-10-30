using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceDashboard.Shared.DTO
{
    public  class UpdateIncomeRequest
    {
        public int IncomeId { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}
