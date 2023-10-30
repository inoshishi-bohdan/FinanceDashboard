using FinanceDashboard.Client.Pages;
using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
using FinanceDashboard.Shared.DTO;
using FinanceDashboard.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly FinanceDashboardContext _financeDashboardContext;
        private decimal EURToUSDRate = 1.06m;
        private decimal USDToEURRate = 0.94m;

        public StatisticController(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
        }
        [HttpPost]
        [Route("IncomesEUR")]
        [Authorize(Roles = "Customer")]
        public async Task<List<StatisticData>> GetUserIncomesEURAsync([FromBody] StatGetIncomesRequest getIncomesRequest)
        {
            return await _financeDashboardContext.Incomes.AsQueryable().Where(income => income.User.Login.Equals(getIncomesRequest.UserLogin))
                .Select(income => new StatisticData { Amount = income.CurrencyId == 1 ? income.Amount : income.Amount * USDToEURRate, Date = income.Date }).ToListAsync();
        }
        [HttpPost]
        [Route("IncomesUSD")]
        [Authorize(Roles = "Customer")]
        public async Task<List<StatisticData>> GetUserIncomesUSDAsync([FromBody] StatGetIncomesRequest getIncomesRequest)
        {
            return await _financeDashboardContext.Incomes.AsQueryable().Where(income => income.User.Login.Equals(getIncomesRequest.UserLogin))
                .Select(income => new StatisticData { Amount = income.CurrencyId == 2 ? income.Amount : income.Amount * EURToUSDRate, Date = income.Date }).ToListAsync();
        }
        [HttpPost]
        [Route("ExpensesEUR")]
        [Authorize(Roles = "Customer")]
        public async Task<List<StatisticData>> GetUserExpensesEURAsync([FromBody] StatGetExpensesRequest getExpensesRequest)
        {
            return await _financeDashboardContext.Expenses.AsQueryable().Where(expense => expense.User.Login.Equals(getExpensesRequest.UserLogin))
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == 1 ? expense.Amount : expense.Amount * USDToEURRate, Date = expense.Date }).ToListAsync();
        }
        [HttpPost]
        [Route("ExpensesUSD")]
        [Authorize(Roles = "Customer")]
        public async Task<List<StatisticData>> GetUserExpensesUSDAsync([FromBody] StatGetExpensesRequest getExpensesRequest)
        {
            return await _financeDashboardContext.Expenses.AsQueryable().Where(expense => expense.User.Login.Equals(getExpensesRequest.UserLogin))
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == 2 ? expense.Amount : expense.Amount * EURToUSDRate, Date = expense.Date }).ToListAsync();
        }
    }
}
