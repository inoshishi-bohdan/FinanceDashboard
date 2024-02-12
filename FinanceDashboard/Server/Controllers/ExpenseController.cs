using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
using FinanceDashboard.Server.Services;
using FinanceDashboard.Shared.DTO.Expense;
using FinanceDashboard.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/expense")]
    [ApiController]
    public class ExpenseController : ControllerBase
    {
        private readonly FinanceDashboardContext _financeDashboardContext;
        private readonly FinanceDashboardCacheAccessor _cacheAccessor;

        public ExpenseController(FinanceDashboardContext financeDashboardContext, FinanceDashboardCacheAccessor cacheAccessor)
        {
            _financeDashboardContext = financeDashboardContext;
            _cacheAccessor = cacheAccessor;
        }

        [HttpGet("categories")]
        [Authorize(Roles = "Customer")]
        public List<ExpenseCategory> GetExpenseCategories()
        {
            return _cacheAccessor.ExpenseCategories;
        }

        [HttpPost("list")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<ExpenseData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserExpensesAsync([FromBody] GetListRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin);

            if (validator.Any()) return validator.BadRequest();

            var result = await _financeDashboardContext.Expenses.AsQueryable().Where(expense => expense.User.Login == request.UserLogin)
                .Include(expense => expense.ExpenseCategory)
                .Include(expense => expense.Currency).Select(expense => new ExpenseData
                {
                    Id = expense.Id,
                    Date = expense.Date,
                    Description = expense.Description,
                    ExpenseCategory = expense.ExpenseCategory.Name,
                    ExpenseCategoryId = expense.ExpenseCategory.Id,
                    Currency = expense.Currency.Name,
                    CurrencyId = expense.Currency.Id,
                    Amount = expense.Amount
                }).ToListAsync();

            return Ok(result);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateExpenseAsync([FromBody] UpdateRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Id)
                .FieldIsRequired(x => x.Date)
                .FieldIsRequired(x => x.ExpenseCategoryId)
                .FieldIsRequired(x => x.Amount)
                .FieldIsRequired(x => x.CurrencyId);

            if (validator.Any()) return validator.BadRequest();

            if (request.Amount <= 0) return BadRequest($"Amount can not be equal to 0 or less");

            var expenseRecord = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == request.Id);
            
            if (expenseRecord == null) return BadRequest($"Expense record not found (id = {request.Id})");

            expenseRecord.Date = request.Date!.Value;
            expenseRecord.Description = request.Description;
            expenseRecord.Amount = request.Amount!.Value;
            expenseRecord.CurrencyId = request.CurrencyId!.Value;
            expenseRecord.ExpenseCategoryId = request.ExpenseCategoryId!.Value;

            await _financeDashboardContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteExpenseAsync([FromBody] DeleteRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Id);

            if (validator.Any()) return validator.BadRequest();

            var expenseRecord = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == request.Id);

            if (expenseRecord == null) return BadRequest($"Expense record not found (id = {request.Id})");

            _financeDashboardContext.Expenses.Remove(expenseRecord);

            await _financeDashboardContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExpenseAsync([FromBody] CreateRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Date)
                .FieldIsRequired(x => x.ExpenseCategoryId)
                .FieldIsRequired(x => x.Amount)
                .FieldIsRequired(x => x.CurrencyId);

            if (validator.Any()) return validator.BadRequest();

            if (request.Amount <= 0) return BadRequest($"Amount can not be equal to 0 or less");

            var userId = (await _financeDashboardContext.Users.AsQueryable().FirstAsync(user => user.Login.Equals(request.UserLogin))).Id;

            var expenseRecord = new Expense()
            {
                UserId = userId,
                Date = request.Date!.Value,
                Description = request.Description,
                Amount = request.Amount!.Value,
                ExpenseCategoryId = request.ExpenseCategoryId!.Value,
                CurrencyId = request.CurrencyId!.Value
            };

            _financeDashboardContext.Expenses.Add(expenseRecord);

            await _financeDashboardContext.SaveChangesAsync();

            return Ok();
        }
    }
}
