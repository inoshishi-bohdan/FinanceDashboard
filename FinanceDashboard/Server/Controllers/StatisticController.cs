using FinanceDashboard.Client.Pages;
using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared.DTO;
using FinanceDashboard.Shared.DTO.Expense;
using FinanceDashboard.Shared.DTO.Income;
using FinanceDashboard.Shared.Enums;
using FinanceDashboard.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        public IActionResult GetIncomeStatisticEUR([FromBody] Shared.DTO.Income.GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var result = _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.EUR ? income.Amount : income.Amount * USDToEURRate, Date = income.Date })
                .ToChartData();

            return Ok(result);
        }

        [HttpPost("incomeUSD")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetIncomeStatisticUSD([FromBody] Shared.DTO.Income.GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var result = _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.USD ? income.Amount : income.Amount * EURToUSDRate, Date = income.Date })
                .ToChartData();

            return Ok(result);
        }

        [HttpPost("expenseEUR")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetExpenseStatisticEUR([FromBody] Shared.DTO.Expense.GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var result = _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.EUR ? expense.Amount : expense.Amount * USDToEURRate, Date = expense.Date })
                .ToChartData();

            return Ok(result);
        }

        [HttpPost("expenseUSD")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetExpenseStatisticUSD([FromBody] Shared.DTO.Expense.GetStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var result = _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.USD ? expense.Amount : expense.Amount * EURToUSDRate, Date = expense.Date })
                .ToChartData();

            return Ok(result);
        }

        [HttpPost("netWorthEUR")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetNetWorthStatisticEUR([FromBody] GetNetWorthStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var incomes = _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.EUR ? income.Amount : income.Amount * USDToEURRate, Date = income.Date })
                .ToChartData();

            var expenses = _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.EUR ? expense.Amount : expense.Amount * USDToEURRate, Date = expense.Date })
                .ToChartData();

            foreach (var monthIncome in incomes)
            {
                var monthExpense = expenses.Find(item => item.Month == monthIncome.Month);
                monthIncome.Amount -= monthIncome.Amount;
            }

            return Ok(incomes);
        }

        [HttpPost("netWorthUSD")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ChartData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetNetWorthStatisticUSD([FromBody] GetNetWorthStatisticRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Year);

            if (validator.Any()) return validator.BadRequest();

            var incomes =  _financeDashboardContext.Incomes
                .AsQueryable()
                .Where(income => income.User.Login == request.UserLogin && income.Date.Year == request.Year)
                .Select(income => new StatisticData { Amount = income.CurrencyId == (int)Currencies.USD ? income.Amount : income.Amount * EURToUSDRate, Date = income.Date })
                .ToChartData();

            var expenses = _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin && expense.Date.Year == request.Year)
                .Select(expense => new StatisticData { Amount = expense.CurrencyId == (int)Currencies.USD ? expense.Amount : expense.Amount * EURToUSDRate, Date = expense.Date })
                .ToChartData();

            foreach (var monthIncome in incomes)
            {
                var monthExpense = expenses.Find(item => item.Month == monthIncome.Month);
                monthIncome.Amount -= monthIncome.Amount;
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

            var result =  await _financeDashboardContext.Expenses
                .AsQueryable()
                .Where(expense => expense.User.Login == request.UserLogin).Select(expense => expense.Date.Year).Distinct().ToListAsync();

            return Ok(result);
        }
    }
}
