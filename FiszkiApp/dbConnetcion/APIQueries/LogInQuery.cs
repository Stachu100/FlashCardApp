using System.Text;
using FiszkiApp.Services;
using Newtonsoft.Json;
using FiszkiApp.EntityClasses.Models;
using System.Net;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    internal class LogInQuery
    {
        private readonly HttpClient _httpClient;

        public LogInQuery()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<UserLoginResult> UserLogIn(string name, string password)
        {
            try
            {
                var userResponse = await _httpClient.GetAsync($"user/{name}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    return new UserLoginResult { Message = "Hasło lub login jest niepoprawne" };
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(userJson);

                if (user == null)
                {
                    return new UserLoginResult { Message = "Hasło lub login jest niepoprawne" };
                }

                if (!user.Is_active)
                {
                    return new UserLoginResult { Message = "Konto jest nieaktywne" };
                }

                var keysResponse = await _httpClient.GetAsync($"encryptionkeys/{user.ID_User}");
                if (!keysResponse.IsSuccessStatusCode)
                {
                    return new UserLoginResult { Message = "Wystąpił problem z pobraniem kluczy szyfrowania." };
                }

                var keysJson = await keysResponse.Content.ReadAsStringAsync();
                var encryptionKeys = JsonConvert.DeserializeObject<EncryptionKeys>(keysJson);

                if (encryptionKeys == null)
                {
                    return new UserLoginResult { Message = "Wystąpił problem z pobraniem kluczy szyfrowania." };
                }

                byte[] encryptedPassword = EntityClasses.AesManaged.EncryptStringToBytes_Aes(password, encryptionKeys.EncryptionKey, encryptionKeys.IV);

                var loginRequest = new
                {
                    UserId = user.ID_User,
                    EncryptedPassword = encryptedPassword
                };

                var content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");
                var loginResponse = await _httpClient.PostAsync("user/login", content);

                if (loginResponse.IsSuccessStatusCode)
                {
                    return new UserLoginResult
                    {
                        UserId = user.ID_User,
                        IsAdmin = user.Is_admin,
                        Message = "Zalogowano poprawnie"
                    };
                }

                return new UserLoginResult { Message = "Hasło lub login jest niepoprawne" };
            }
            catch
            {
                return new UserLoginResult { Message = "Wystąpił błąd podczas logowania" };
            }
        }

        public async Task<bool?> IsUserActiveAsync(string username)
        {
            try
            {
                var response = await _httpClient.GetAsync($"user/active/{username}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                bool isActive = JsonConvert.DeserializeObject<bool>(json);
                return isActive;
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> LoginByQrCodeAsync(string qrToken, int userId)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(new { token = qrToken, userID = userId }),Encoding.UTF8,"application/json");

                var response = await _httpClient.PostAsync("qrlogin/verify", content);

                if (!response.IsSuccessStatusCode)
                {
                    return "Niepoprawny kod QR lub błąd logowania";
                }

                return "Zalogowano pomyślnie";
            }
            catch (Exception)
            {
                return "Błąd podczas logowania QR";
            }
        }
    }
}