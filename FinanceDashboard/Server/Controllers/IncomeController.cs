using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
using FinanceDashboard.Shared.DTO.Income;
using FinanceDashboard.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/income")]
    [ApiController]
    public class IncomeController : ControllerBase
    {
        private readonly FinanceDashboardContext _financeDashboardContext;

        public IncomeController(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
        }


        [HttpPost("list")]
        [Authorize(Roles = "Customer")]
        public async Task<List<IncomeData>> GetUserIncomesAsync([FromBody] GetListRequest request)
        {
            return await _financeDashboardContext.Incomes.AsQueryable().Where(income => income.User.Login.Equals(request.UserLogin))
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

        [HttpPut("change")]
        [Authorize(Roles = "Customer")]
        public async Task UpdateIncomeAsync([FromBody] IncomeData incomeData)
        {
            var incomeRecord = await _financeDashboardContext.Incomes.AsQueryable().FirstAsync(income => income.Id == incomeData.Id);           
            incomeRecord.Date = incomeData.Date;
            incomeRecord.Description = incomeData.Description;
            incomeRecord.Amount = incomeData.Amount;
            incomeRecord.CurrencyId = incomeData.CurrencyId;
            
            await _financeDashboardContext.SaveChangesAsync();
        }

        [HttpPost("remove")]
        [Authorize(Roles = "Customer")]
        public async Task DeleteIncomeAsync([FromBody] DeleteRequest request)
        {
            var incomeRecord = await _financeDashboardContext.Incomes.AsQueryable().FirstAsync(income => income.Id == request.Id);
            
            _financeDashboardContext.Incomes.Remove(incomeRecord);
            
            await _financeDashboardContext.SaveChangesAsync();
        }
        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        public async Task AddIncomeAsync([FromBody] CreateRequest request)
        {
            var userId = (await _financeDashboardContext.Users.AsQueryable().FirstAsync(user => user.Login.Equals(request.UserLogin))).Id;
            
            var incomeRecord = new Income()
            {
                UserId = userId,
                Date = request.Date,
                Description = request.Description,
                Amount = request.Amount,
                CurrencyId = request.CurrencyId
            };

            _financeDashboardContext.Incomes.Add(incomeRecord);
            
            await _financeDashboardContext.SaveChangesAsync();
        }
    }
}
