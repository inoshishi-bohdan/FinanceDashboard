using FinanceDashboard.Server.Authentication;
using FinanceDashboard.Server.Data;
using FinanceDashboard.Server.Model;
using FinanceDashboard.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDashboard.Server.Controllers
{
    [Route("api/[controller]")]
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
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public ActionResult<UserSession> Login([FromBody] LoginRequest loginRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = jwtAuthenticationManager.GenerateJwtToken(loginRequest.UserLogin, loginRequest.Password);
            if (userSession is null)
            {
                return Unauthorized();
            }
            else
            {
                return userSession;
            }
        }
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public ActionResult<UserSession> Register([FromBody] RegisterRequest registerRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var newUser = new User
            {
                Name = registerRequest.Name,
                Surname = registerRequest.Surname,
                Login = registerRequest.Login,
                Password = jwtAuthenticationManager.GetHashedPassword(registerRequest.Password),
                RoleId = 1
            };
            _financeDashboardContext.Users.Add(newUser);
            _financeDashboardContext.SaveChanges();
            // add user to database and then authentication
            var userSession = jwtAuthenticationManager.GenerateJwtToken(registerRequest.Login, registerRequest.Password);
            if (userSession is null)
            {
                return Unauthorized();
            }
            else
            {
                return userSession;
            }
        }
        [HttpPost]
        [Route("UserExist")]
        [AllowAnonymous]
        public ActionResult<bool> CheckExistingUsers([FromBody] UserExistenceRequest userExistenceRequest)
        {
            if (_userAccountService.GetUserAccountByUserLogin(userExistenceRequest.Login) != null)
            {
                return true;
            }
            return false;
        }
    }
}
