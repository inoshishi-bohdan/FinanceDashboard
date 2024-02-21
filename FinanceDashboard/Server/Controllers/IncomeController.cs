using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared.Models;
using FinanceDashboard.Shared.DTO.Income;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(typeof(List<IncomeData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserIncomesAsync([FromBody] GetListRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var result = await _financeDashboardContext.Incomes.AsQueryable().Where(income => income.User.Login == request.UserLogin)
                .Include(income => income.Currency).Select(income => new IncomeData
                {
                    Id = income.Id,
                    Date = income.Date,
                    Description = income.Description,
                    Currency = income.Currency.Name,
                    CurrencyId = income.CurrencyId,
                    Amount = Math.Round(income.Amount, 2)
                }).ToListAsync();

            return Ok(result);
        }

        [HttpPost("update")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateIncomeAsync([FromBody] UpdateRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Id)
                .FieldIsRequired(x => x.Date)
                .FieldIsRequired(x => x.Amount)
                .FieldIsRequired(x => x.CurrencyId);

            if (validator.Any()) return validator.BadRequest();

            var currency = await _financeDashboardContext.Currencies.FindAsync(request.CurrencyId);
            if (currency == null) return BadRequest($"Currency with Id = {request.CurrencyId} not found");

            if (request.Amount <= 0) return BadRequest($"Amount can not be equal to 0 or less");

            var incomeRecord = await _financeDashboardContext.Incomes.AsQueryable().FirstAsync(income => income.Id == request.Id);
            if (incomeRecord == null) return BadRequest($"Income record not found (id = {request.Id})");

            incomeRecord.Date = request.Date!.Value;
            incomeRecord.Description = request.Description;
            incomeRecord.Amount = Math.Round(request.Amount!.Value, 2);
            incomeRecord.CurrencyId = request.CurrencyId!.Value;

            await _financeDashboardContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteIncomeAsync([FromBody] DeleteRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Id);

            if (validator.Any()) return validator.BadRequest();

            var incomeRecord = await _financeDashboardContext.Incomes.AsQueryable().FirstAsync(income => income.Id == request.Id);
            if (incomeRecord == null) return BadRequest($"Income record not found (id = {request.Id})");

            _financeDashboardContext.Incomes.Remove(incomeRecord);

            await _financeDashboardContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("create")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateIncomeAsync([FromBody] CreateRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Date)
                .FieldIsRequired(x => x.Amount)
                .FieldIsRequired(x => x.CurrencyId);

            if (validator.Any()) return validator.BadRequest();

            var user =  await _financeDashboardContext.Users.AsQueryable().FirstOrDefaultAsync(user => user.Login == request.UserLogin);
            if (user == null) return BadRequest($"User with login {request.UserLogin} not found");

            var currency = await _financeDashboardContext.Currencies.FindAsync(request.CurrencyId);
            if (currency == null) return BadRequest($"Currency with Id = {request.CurrencyId} not found");

            if (request.Amount <= 0) return BadRequest($"Amount can not be equal to 0 or less");

            var incomeRecord = new Income()
            {
                UserId = user.Id,
                Date = request.Date!.Value,
                Description = request.Description,
                Amount =  Math.Round(request.Amount!.Value, 2),
                CurrencyId = request.CurrencyId!.Value
            };

            _financeDashboardContext.Incomes.Add(incomeRecord);

            await _financeDashboardContext.SaveChangesAsync();

            return Ok(new CreateResponse { Id = incomeRecord.Id });
        }
    }
}
