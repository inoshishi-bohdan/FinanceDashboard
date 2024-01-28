using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
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

        public ExpenseController(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
        }

        [HttpGet("categories")]
        [Authorize(Roles = "Customer")]
        public async Task<List<ExpenseCategory>> GetExpenseCategories()
        {
            return await _financeDashboardContext.ExpenseCategories.ToListAsync();
        }

        [HttpPost("list")]
        [Authorize(Roles = "Customer")]
        public async Task<List<ExpenseData>> GetUserExpensesAsync([FromBody] GetListRequest request)
        {
            return await _financeDashboardContext.Expenses.AsQueryable().Where(expense => expense.User.Login == request.UserLogin)
                .Include(expense => expense.ExpenseCategory)
                .Include(expense => expense.Currency).Select(expense => new ExpenseData
                {
                    Id = expense.Id,
                    Date = expense.Date,
                    Description = expense.Description,
                    ExpenseCategory = expense.ExpenseCategory.Name,
                    ExpenseCategoryId = expense.ExpenseCategory.Id,
                    CurrencyName = expense.Currency.Name,
                    CurrencyId = expense.Currency.Id,
                    Amount = expense.Amount
                }).ToListAsync();
        }

        [HttpPut("change")]
        [Authorize(Roles = "Customer")]
        public async Task UpdateExpenseAsync([FromBody] ExpenseData expenseData)
        {
            var expenseRecord = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == expenseData.Id);
            expenseRecord.Date = expenseData.Date;
            expenseRecord.Description = expenseData.Description;
            expenseRecord.Amount = expenseData.Amount;
            expenseRecord.CurrencyId = expenseData.CurrencyId;
            expenseRecord.ExpenseCategoryId = expenseData.ExpenseCategoryId;
            
            await _financeDashboardContext.SaveChangesAsync();
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Customer")]
        public async Task DeleteExpenseAsync([FromBody] DeleteRequest request)
        {
            var expenseRecord = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == request.Id);
            
            _financeDashboardContext.Expenses.Remove(expenseRecord);
            
            await _financeDashboardContext.SaveChangesAsync();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task AddExpenseAsync([FromBody] CreateRequest request)
        {
            var userId = (await _financeDashboardContext.Users.AsQueryable().FirstAsync(user => user.Login.Equals(request.UserLogin))).Id;
            
            var expenseRecord = new Expense()
            {
                UserId = userId,
                Date = request.Date,
                Description = request.Description,
                Amount = request.Amount,
                ExpenseCategoryId = request.ExpenseCategoryId,
                CurrencyId = request.CurrencyId
            };

            _financeDashboardContext.Expenses.Add(expenseRecord);
            
            await _financeDashboardContext.SaveChangesAsync();
        }
    }
}
