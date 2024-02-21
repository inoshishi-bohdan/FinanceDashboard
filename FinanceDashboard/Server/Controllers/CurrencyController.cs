using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared.Models;
using FinanceDashboard.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly FinanceDashboardCacheAccessor _cacheAccessor;

        public CurrencyController(FinanceDashboardCacheAccessor cacheAccessor)
        {
            _cacheAccessor = cacheAccessor;
        }
        [HttpGet("list")]
        [Authorize(Roles = "Customer")]
        public List<Currency> GetCurrencies()
        {
            return _cacheAccessor.Currencies;
        }
    }
}
