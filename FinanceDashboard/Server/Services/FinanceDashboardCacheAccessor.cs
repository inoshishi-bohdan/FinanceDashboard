using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared.Models;

namespace FinanceDashboard.Server.Services
{
    public class FinanceDashboardCacheAccessor
    {
        private readonly FinanceDashboardCacheService _cacheService;
        private readonly FinanceDashboardContext _context;

        public FinanceDashboardCacheAccessor(FinanceDashboardCacheService cacheService, FinanceDashboardContext context)
        {
            _cacheService = cacheService;
            _context = context;
        }
        public List<ExpenseCategory> ExpenseCategories => _cacheService.GetExpenseCategories(_context);

        public List<Currency> Currencies => _cacheService.GetCurrencies(_context);

    }
}
