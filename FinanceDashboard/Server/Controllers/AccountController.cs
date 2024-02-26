using FinanceDashboard.Server.Authentication;
using FinanceDashboard.Server.Data;
using FinanceDashboard.Shared.Models;
using FinanceDashboard.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinanceDashboard.Shared.DTO.User;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserAccountService _userAccountService;
        private readonly FinanceDashboardContext _financeDashboardContext;

        public AccountController(UserAccountService userAccountService, FinanceDashboardContext financeDashboardContext)
        {
            _userAccountService = userAccountService;
            _financeDashboardContext = financeDashboardContext;
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<UserSession> Login([FromBody] LoginRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.UserLogin)
                .FieldIsRequired(x => x.Password);

            if (validator.Any()) return validator.BadRequest();

            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = jwtAuthenticationManager.GenerateJwtToken(request.UserLogin!, request.Password!);

            if (userSession is null)
            {
                return Unauthorized();
            }
            else
            {
                return userSession;
            }
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult<UserSession> Register([FromBody] RegisterRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Login)
                .FieldIsRequired(x => x.Password);

            if (validator.Any()) return validator.BadRequest();

            var rand = new Random();
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var newUser = new User
            {
                Name = request.Name ?? $"User{ rand.Next()}",
                Login = request.Login!,
                Password = jwtAuthenticationManager.GetHashedPassword(request.Password!),
                RoleId = 1
            };

            _financeDashboardContext.Users.Add(newUser);
            _financeDashboardContext.SaveChanges();
            // add user to database and then authentication
            var userSession = jwtAuthenticationManager.GenerateJwtToken(request.Login!, request.Password!);

            if (userSession is null)
            {
                return Unauthorized();
            }
            else
            {
                return userSession;
            }
        }
        [HttpPost("userExist")]
        [AllowAnonymous]
        public ActionResult<bool> CheckExistingUsers([FromBody] UserExistenceRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Login);

            if (validator.Any()) return validator.BadRequest();

            if (_userAccountService.GetUserAccountByUserLogin(request.Login!) != null)
            {
                return true;
            }

            return false;
        }

        [HttpPost("edit")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUserAsync([FromBody] UpdateRequest request)
        {
            var validator = FieldValidator.Create(request);

            validator
                .FieldIsRequired(x => x.Login);

            var user = await _financeDashboardContext.Users.FirstOrDefaultAsync(user => user.Login == request.Login);
            if (user == null) return BadRequest($"User with login {request.Login} not found");

            if (request.NewName != null) user.Name = request.NewName;

            if (request.NewLogin != null)
            {
                var existingRecord = await _financeDashboardContext.Users.FirstOrDefaultAsync(user => user.Login == request.NewLogin && user.Login != request.Login); 
                if (existingRecord != null) return BadRequest($"User with login {request.NewLogin} already exists");

                user.Login = request.NewLogin;
            }

            if (request.NewPassword != null && request.RepeatedPassword != null)
            {
                if (request.NewPassword != request.RepeatedPassword) return BadRequest($"Passwords don't match");

                var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
                user.Password = jwtAuthenticationManager.GetHashedPassword(request.NewPassword);
            }

            if (request.NewUserImagePath != null)
            {
                var image = await _financeDashboardContext.Images.FirstOrDefaultAsync(image => image.Path == request.NewUserImagePath);
                if (image == null) return BadRequest($"Image wiht name {request.NewUserImagePath} not exists");

                user.ImageId = image.Id;
            }

            await _financeDashboardContext.SaveChangesAsync();

            return Ok();
        }
    }
}
