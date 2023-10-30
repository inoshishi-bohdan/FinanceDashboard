using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceDashboard.Shared.DTO
{
    public class StatGetIncomesRequest
    {
        public string? UserLogin { get; set; }
        public int CurrencyId { get; set; }
    }
}
