using System;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.dbConnetcion.APIQueries;
using Newtonsoft.Json;

namespace FiszkiApp.Services
{
    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private const string UserNameKey = "UserName";
        private const string IsadminKey = "Is_admin";
        private const string UserIdKey = "UserId";
        private const string RememberMe = "RememberMe";
        public async Task<(bool AuthStateKey, string? UserId)> IsAuthenticatedAsync()
        {
            var authState = Preferences.Default.Get(AuthStateKey, false);
            var userName = Preferences.Default.Get<string>(UserNameKey, null);
            var is_admin = Preferences.Default.Get<bool>(IsadminKey, false);
            var userId = Preferences.Default.Get<string>(UserIdKey, null);
            var rememberMe = Preferences.Default.Get<bool>(RememberMe, false);

            if (authState && !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userName))
            {
                try
                {
                    var logInQuery = new LogInQuery();
                    bool? isActive = await logInQuery.IsUserActiveAsync(userName);

                    if (isActive != true)
                    {
                        Logout();
                        return (false, null);
                    }

                    return (true, userId);
                }
                catch
                {
                    Logout();
                    return (false, null);
                }
            }

            return (false, null);
        }

        public async Task<UserLoginResult> Login(string userName, string userPassword, bool rememberMe)
        {
            var loginInQuery = new LogInQuery();
            var result = await loginInQuery.UserLogIn(userName, userPassword);

            if (result.Message != "Hasło lub login jest niepoprawne" && result.Message != "Wystąpił błąd podczas logowania" && result.Message != "Konto jest nieaktywne")
            {
                Preferences.Default.Set(AuthStateKey, true);
                Preferences.Default.Set(UserNameKey, userName);
                Preferences.Default.Set(UserIdKey, result.UserId.ToString());
                Preferences.Default.Set(RememberMe, rememberMe);
                Preferences.Default.Set(IsadminKey, result.IsAdmin);
            }
            return result;
        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}