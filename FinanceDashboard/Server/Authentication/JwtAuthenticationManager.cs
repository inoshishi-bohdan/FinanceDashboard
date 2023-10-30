using FinanceDashboard.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinanceDashboard.Server.Authentication
{
    public class JwtAuthenticationManager
    {
        public const string JWT_SECURITY_KEY = "yPasdfasui37OljKh2sul3sdSAsd4313gdXkt7F23HkdlP";
        private const int   JWT_TOKEN_VALIDITY_MINS = 20;
        private readonly UserAccountService _userAccountService;

        public JwtAuthenticationManager(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }
        public UserSession? GenerateJwtToken(string userLogin, string password)
        {
            var hashePassword = GetHashedPassword(password);
            if (string.IsNullOrWhiteSpace(userLogin) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }
            //Validate the User Credentials
            var userAccount = _userAccountService.GetUserAccountByUserLogin(userLogin);
            if (userAccount == null || !userAccount.Password.Equals(hashePassword)) 
            {
                return null;
            }
            //Generating JWT token
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim> {
                new Claim(ClaimTypes.Name, userAccount.UserName),
                new Claim(ClaimTypes.Role, userAccount.Role),
            });
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);
            var securityTokenDescriptor = new SecurityTokenDescriptor(){
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);
            var userSession = new UserSession()
            {
                UserName = userAccount.UserName,
                UserLogin = userAccount.UserLogin,
                Role = userAccount.Role,
                Token = token,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
            };
            return userSession;
        }
        public string GetHashedPassword(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Compute the hash value from the input bytes
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the hash bytes to a hexadecimal string
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2")); // "x2" formats each byte as a two-digit hexadecimal number
                }

                return stringBuilder.ToString();
            }
        }
    }
}
