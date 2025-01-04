using System;
using System.Threading.Tasks;

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

            if (authState && !string.IsNullOrEmpty(userId))
            {
                return (true, userId);
            }

            return (false, null);
        }
        public async Task<string> Login(string userName, string userPassword, bool rememberMe)
        {
            var loginInQuery = new dbConnetcion.APIQueries.LogInQuery();
            string result = await loginInQuery.UserLogIn(userName, userPassword);

            if (result != "Hasło lub login jest niepoprawne" && result != "Wystąpił błąd podczas logowania")
            {

                Preferences.Default.Set(AuthStateKey, true);
                Preferences.Default.Set(UserNameKey, userName);
                Preferences.Default.Set(UserPasswordKey, userPassword);
                Preferences.Default.Set(UserIdKey, result);
                Preferences.Default.Set(RememberMe, rememberMe);
                Console.WriteLine($"Logged in user: {userName} with ID: {result}");
                return result;
            }

            return result;
        }
        public void Logout()
        {
            Preferences.Remove("FlashcardBackgroundColor");
            Preferences.Remove("FlashcardTextColor");
            Preferences.Default.Remove(AuthStateKey);
        }
    }
}