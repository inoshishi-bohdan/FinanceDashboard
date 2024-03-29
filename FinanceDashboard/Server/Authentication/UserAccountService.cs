﻿using FinanceDashboard.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceDashboard.Server.Authentication
{
    public class UserAccountService
    {
        private readonly FinanceDashboardContext _financeDashboardContext;
        private List<UserAccount> _users;
        public UserAccountService(FinanceDashboardContext financeDashboardContext)
        {
            _financeDashboardContext = financeDashboardContext;
            _users = financeDashboardContext.Users.AsQueryable().Include(user => user.Role).Select(user => new UserAccount {
            UserName = user.Name != null ? user.Name : string.Empty,
            UserLogin = user.Login,
            Password = user.Password,
            Role  = user.Role.Name,
            ImagePath = user.Image != null ? user.Image.Path : string.Empty
            }).ToList();
        }
        public UserAccount? GetUserAccountByUserLogin(string userLogin)
        {
            if (_financeDashboardContext.Users.Count() != _users.Count())
            {
                _users = _financeDashboardContext.Users.AsQueryable().Include(user => user.Role).Select(user => new UserAccount
                {
                    UserName = user.Name != null ? user.Name : string.Empty,
                    UserLogin = user.Login,
                    Password = user.Password,
                    Role = user.Role.Name,
                    ImagePath = user.Image != null ? user.Image.Path : string.Empty
                }).ToList();
            }
            return _users.FirstOrDefault(x => x.UserLogin == userLogin);
        }
    }
}
