﻿using Blazored.SessionStorage;
using FinanceDashboard.Client.Extensions;
using FinanceDashboard.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FinanceDashboard.Client.Authentication
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSession = await _sessionStorage.ReadEncryptedItemAsync<UserSession>("UserSession");
                if (userSession == null)
                    return await Task.FromResult(new AuthenticationState(_anonymous));
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName!),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim(ClaimTypes.Email, userSession.UserLogin)
                }, "JwtAuth"));

                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_anonymous));
            }
        }

        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if (userSession != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName!),
                    new Claim(ClaimTypes.Role, userSession.Role),
                    new Claim(ClaimTypes.Email, userSession.UserLogin)
                }));
                userSession.ExpiryTimeStamp = DateTime.UtcNow.AddSeconds(userSession.ExpiresIn);
                await _sessionStorage.SaveItemEncryptedAsync("UserSession", userSession);
            }
            else
            {
                claimsPrincipal = _anonymous;
                await _sessionStorage.RemoveItemAsync("UserSession");
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task<string> GetToken()
        {
            var result = string.Empty;

            try
            {
                var userSession = await _sessionStorage.ReadEncryptedItemAsync<UserSession>("UserSession");
                if (userSession != null && DateTime.UtcNow < userSession.ExpiryTimeStamp)
                    result = userSession.Token;
            }
            catch { }

            return result;
        }
    }
}
