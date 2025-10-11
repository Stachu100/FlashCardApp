using System;
using System.Threading.Tasks;
using FiszkiApp.dbConnetcion.APIQueries;

namespace FiszkiApp.Services
{
    public class AuthService
    {
        private const string AuthStateKey = "AuthState";
        private const string UserNameKey = "UserName";
        private const string UserPasswordKey = "UserPassword";
        private const string UserIdKey = "UserId";
        private const string RememberMe = "RememberMe";
        public async Task<(bool AuthStateKey, string? UserId)> IsAuthenticatedAsync()
        {
            var authState = Preferences.Default.Get(AuthStateKey, false);
            var userName = Preferences.Default.Get<string>(UserNameKey, null);
            var userPassword = Preferences.Default.Get<string>(UserPasswordKey, null);
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

        public async Task<string> Login(string userName, string userPassword, bool rememberMe)
        {
            var loginInQuery = new dbConnetcion.APIQueries.LogInQuery();
            string result = await loginInQuery.UserLogIn(userName, userPassword);

            if (result != "Hasło lub login jest niepoprawne" && result != "Wystąpił błąd podczas logowania" && result != "Konto jest nieaktywne")
            {

                Preferences.Default.Set(AuthStateKey, true);
                Preferences.Default.Set(UserNameKey, userName);
                Preferences.Default.Set(UserIdKey, result);
                Preferences.Default.Set(RememberMe, rememberMe);
                return result;
            }

            return result;
        }
        public void Logout()
        {
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}