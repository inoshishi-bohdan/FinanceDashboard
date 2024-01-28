using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly FinanceDashboardContext _financeDashboardContext;

        public CurrencyController(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
        }
        [HttpGet("list")]
        [Authorize(Roles = "Customer")]
        public async Task<List<Currency>> GetCurrenciesAsync()
        {
            return await _financeDashboardContext.Currencies.ToListAsync();
        }
    }
}
