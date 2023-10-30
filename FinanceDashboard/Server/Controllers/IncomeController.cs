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
    public class IncomeController : ControllerBase
    {
        private readonly FinanceDashboardContext _financeDashboardContext;

        public IncomeController(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
        }


        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<List<IncomeData>> GetUserIncomesAsync([FromBody] GetIncomesRequest getIncomesRequest)
        {
            return await _financeDashboardContext.Incomes.AsQueryable().Where(income => income.User.Login.Equals(getIncomesRequest.UserLogin))
                .Include(income => income.Currency).Select(income => new IncomeData 
                { 
                    Id = income.Id,
                    Date = income.Date,
                    Description = income.Description,
                    CurrencyName = income.Currency.Name,
                    CurrencyId = income.CurrencyId,
                    Amount = income.Amount
                }).ToListAsync();
        }

        [HttpPut]
        [Authorize(Roles = "Customer")]
        public async Task UpdateIncomeAsync([FromBody] IncomeData incomeData)
        {
            var incomeToUpdate = await _financeDashboardContext.Incomes.AsQueryable().FirstAsync(income => income.Id == incomeData.Id);
            incomeToUpdate.Date = incomeData.Date;
            incomeToUpdate.Description = incomeData.Description;
            incomeToUpdate.Amount = incomeData.Amount;
            incomeToUpdate.CurrencyId = incomeData.CurrencyId;
            await _financeDashboardContext.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Remove")]
        [Authorize(Roles = "Customer")]
        public async Task DeleteIncomeAsync([FromBody] DeleteIncomeRequest deleteIncomeRequest)
        {
            var incomeToDelete = await _financeDashboardContext.Incomes.AsQueryable().FirstAsync(income => income.Id == deleteIncomeRequest.IncomeId);
            _financeDashboardContext.Incomes.Remove(incomeToDelete);
            await _financeDashboardContext.SaveChangesAsync();
        }
        [HttpPost]
        [Route("Add")]
        [Authorize(Roles = "Customer")]
        public async Task AddIncomeAsync([FromBody] AddIncomeRequest addIncomeRequest)
        {
            var userId = _financeDashboardContext.Users.First(user => user.Login.Equals(addIncomeRequest.UserLogin)).Id;
            var newIncome = new Income()
            {
                UserId = userId,
                Date = addIncomeRequest.Date,
                Description = addIncomeRequest.Description,
                Amount = addIncomeRequest.Amount,
                CurrencyId = addIncomeRequest.CurrencyId
            };
            _financeDashboardContext.Incomes.Add(newIncome);
            await _financeDashboardContext.SaveChangesAsync();
        }
    }
}
