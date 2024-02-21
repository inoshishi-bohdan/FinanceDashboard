using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared.Models;
using FinanceDashboard.Server.Services;
using FinanceDashboard.Shared.DTO.Expense;
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

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

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
                    Amount =  Math.Round(expense.Amount, 2) 
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

            var currency = await _financeDashboardContext.Currencies.FindAsync(request.CurrencyId);
            if (currency == null) return BadRequest($"Currency with Id = {request.CurrencyId} not found");

            var expenseCategory = await _financeDashboardContext.ExpenseCategories.FindAsync(request.ExpenseCategoryId);
            if (expenseCategory == null) return BadRequest($"Expense Category with Id = {request.ExpenseCategoryId} not found");

            if (request.Amount <= 0) return BadRequest($"Amount can not be equal to 0 or less");

            var expenseRecord = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == request.Id);     
            if (expenseRecord == null) return BadRequest($"Expense record not found (id = {request.Id})");

            expenseRecord.Date = request.Date!.Value;
            expenseRecord.Description = request.Description;
            expenseRecord.Amount = Math.Round(request.Amount!.Value, 2);
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
        [ProducesResponseType(typeof(CreateResponse),StatusCodes.Status200OK)]
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

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var currency = await _financeDashboardContext.Currencies.FindAsync(request.CurrencyId);
            if (currency == null) return BadRequest($"Currency with Id = {request.CurrencyId} not found");

            var expenseCategory = await _financeDashboardContext.ExpenseCategories.FindAsync(request.ExpenseCategoryId);
            if (expenseCategory == null) return BadRequest($"Expense Category with Id = {request.ExpenseCategoryId} not found");

            if (request.Amount <= 0) return BadRequest($"Amount can not be equal to 0 or less");

            var expenseRecord = new Expense()
            {
                UserId = user.Id,
                Date = request.Date!.Value,
                Description = request.Description,
                Amount =  Math.Round(request.Amount!.Value, 2),
                ExpenseCategoryId = request.ExpenseCategoryId!.Value,
                CurrencyId = request.CurrencyId!.Value
            };

            _financeDashboardContext.Expenses.Add(expenseRecord);

            await _financeDashboardContext.SaveChangesAsync();

            return Ok(new CreateResponse { Id = expenseRecord.Id });
        }
    }
}
