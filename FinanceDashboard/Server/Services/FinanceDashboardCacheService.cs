using FinanceDashboard.Server.Cache;
using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
using FinanceDashboard.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Services
{
    public class FinanceDashboardCacheService
    {
        private readonly CacheOfDbData<List<ExpenseCategory>> _expenseCategories;
        private readonly CacheOfDbData<List<Currency>> _currencies;

        private static readonly TimeSpan Timeout = new(0, 15, 0);

        public FinanceDashboardCacheService()
        {
            _expenseCategories = new CacheOfDbData<List<ExpenseCategory>>(Timeout, LoadExpenseCategoriesAsync);
            _currencies = new CacheOfDbData<List<Currency>>(Timeout, LoadCurrenciesAsync);
        }

        public List<ExpenseCategory> GetExpenseCategories(FinanceDashboardContext financeDashboardContext) => _expenseCategories.GetContent(financeDashboardContext);

        public List<Currency> GetCurrencies(FinanceDashboardContext financeDashboardContext) => _currencies.GetContent(financeDashboardContext);


        private async Task<List<ExpenseCategory>> LoadExpenseCategoriesAsync(FinanceDashboardContext financeDashboardContext)
        {
            return await financeDashboardContext.ExpenseCategories.ToListAsync();
        }

        private async Task<List<Currency>> LoadCurrenciesAsync(FinanceDashboardContext financeDashboardContext)
        {
            return await financeDashboardContext.Currencies.ToListAsync();
        }
    }
}
