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
    public class ExpenseController : ControllerBase
    {
        private readonly FinanceDashboardContext _financeDashboardContext;

        public ExpenseController(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
        }

        [HttpGet]
        [Route("Categories")]
        [Authorize(Roles = "Customer")]
        public async Task<List<ExpenseCategory>> GetExpenseCategories()
        {
            return await _financeDashboardContext.ExpenseCategories.ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<List<ExpenseData>> GetUserExpensesAsync([FromBody] GetExpensesRequest expensesRequest)
        {
            return await _financeDashboardContext.Expenses.AsQueryable().Where(expense => expense.User.Login.Equals(expensesRequest.UserLogin))
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

        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task UpdateExpenseAsync([FromBody] ExpenseData expenseData)
        {
            var expenseToUpdate = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == expenseData.Id);
            expenseToUpdate.Date = expenseData.Date;
            expenseToUpdate.Description = expenseData.Description;
            expenseToUpdate.Amount = expenseData.Amount;
            expenseToUpdate.CurrencyId = expenseData.CurrencyId;
            expenseToUpdate.ExpenseCategoryId = expenseData.ExpenseCategoryId;
            await _financeDashboardContext.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Remove")]
        [Authorize(Roles = "Customer")]
        public async Task DeleteExpenseAsync([FromBody] DeleteExpenseRequest deleteExpenseRequest)
        {
            var expenseToDelete = await _financeDashboardContext.Expenses.AsQueryable().FirstAsync(expense => expense.Id == deleteExpenseRequest.ExpenseId);
            _financeDashboardContext.Expenses.Remove(expenseToDelete);
            await _financeDashboardContext.SaveChangesAsync();
        }
        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Customer")]
        public async Task AddExpenseAsync([FromBody] AddExpenseRequest addExpenseRequest)
        {
            var userId = _financeDashboardContext.Users.First(user => user.Login.Equals(addExpenseRequest.UserLogin)).Id;
            var newExpense = new Expense()
            {
                UserId = userId,
                Date = addExpenseRequest.Date,
                Description = addExpenseRequest.Description,
                Amount = addExpenseRequest.Amount,
                ExpenseCategoryId = addExpenseRequest.ExpenseCategoryId,
                CurrencyId = addExpenseRequest.CurrencyId
            };
            _financeDashboardContext.Expenses.Add(newExpense);
            await _financeDashboardContext.SaveChangesAsync();
        }
    }
}
