using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared;
using FinanceDashboard.Shared.DTO;
using FinanceDashboard.Shared.DTO.Expense;
using FinanceDashboard.Shared.DTO.Income;
using FinanceDashboard.Shared.Enums;
using FinanceDashboard.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/statistic")]
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

        [HttpPost("incomeEUR")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIncomeStatisticEUR([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result = await _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.EUR ? income.Amount : income.Amount * USDToEURRate, Date = income.Date })
                .ToChartDataAsync();

            return Ok(result);
        }

        [HttpPost("incomeUSD")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIncomeStatisticUSD([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result = await _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.USD ? income.Amount : income.Amount * EURToUSDRate, Date = income.Date })
                .ToChartDataAsync();

            return Ok(result);
        }

        [HttpPost("expenseEUR")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetExpenseStatisticEUR([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result = await _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.EUR ? expense.Amount : expense.Amount * USDToEURRate, Date = expense.Date })
                .ToChartDataAsync();

            return Ok(result);
        }

        [HttpPost("expenseUSD")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetExpenseStatisticUSD([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result = await _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.USD ? expense.Amount : expense.Amount * EURToUSDRate, Date = expense.Date })
                .ToChartDataAsync();

            return Ok(result);
        }

        [HttpPost("netWorthEUR")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNetWorthStatisticEUR([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var incomes = await _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.EUR ? income.Amount : income.Amount * USDToEURRate, Date = income.Date })
                .ToChartDataAsync();

            var expenses = await _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.EUR ? expense.Amount : expense.Amount * USDToEURRate, Date = expense.Date })
                .ToChartDataAsync();

            foreach (var monthIncome in incomes)
            {
                var monthExpense = expenses.Find(item => item.Name == monthIncome.Name);
                monthIncome.Amount -= monthExpense!.Amount;
            }

            return Ok(incomes);
        }

        [HttpPost("netWorthUSD")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetNetWorthStatisticUSD([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var incomes = await _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.USD ? income.Amount : income.Amount * EURToUSDRate, Date = income.Date })
                .ToChartDataAsync();

            var expenses = await _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.USD ? expense.Amount : expense.Amount * EURToUSDRate, Date = expense.Date })
                .ToChartDataAsync();

            foreach (var monthIncome in incomes)
            {
                var monthExpense = expenses.Find(item => item.Name == monthIncome.Name);
                monthIncome.Amount -= monthExpense!.Amount;
            }

            return Ok(incomes);
        }

        [HttpPost("incomeYears")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetIncomeYearsAsync([FromBody] GetIncomesYearsRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result =  await _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin).Select(income => income.Date.Year).Distinct().ToListAsync();

            return Ok(result);
        }

        [HttpPost("expenseYears")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetExpenseYearsAsync([FromBody] GetExpensesYearsRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result =  await _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin).Select(expense => expense.Date.Year).Distinct().ToListAsync();

            return Ok(result);
        }

        [HttpPost("expenseCategories")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(Dictionary<string, List<ChartData>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetExpenseCategoriesDistributionAsync([FromBody] GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result = new Dictionary<string, List<ChartData>>
            {
                {"Jan", new List<ChartData> ()},
                {"Feb", new List<ChartData> ()},
                {"Mar", new List<ChartData> ()},
                {"Apr", new List<ChartData> ()},
                {"May", new List<ChartData> ()},
                {"Jun", new List<ChartData> ()},
                {"Jul", new List<ChartData> ()},
                {"Aug", new List<ChartData> ()},
                {"Sep", new List<ChartData> ()},
                {"Oct", new List<ChartData> ()},
                {"Nov", new List<ChartData> ()},
                {"Dec", new List<ChartData> ()}
            };
            var months = await _financeDashboardContext.Expenses.Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year).GroupBy(expense => expense.Date.Month, (key, g) => new
            {
                Name = System.Globalization.DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(key),
                Data = g.ToList()
            }).ToListAsync();

            var categories = new List<ChartData>();
            
            foreach (var month in months)
            {
                categories = month.Data.GroupBy(item => item.ExpenseCategoryId, (k, g) => new ChartData{ Name = ((ExpenseCategories)k).GetDisplayAsOrName(), Amount = g.Count()}).ToList();
                result[month.Name] = categories;
            }

            return Ok(result);
        }
    }
}
