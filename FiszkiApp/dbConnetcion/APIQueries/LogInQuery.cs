using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> UserLogIn(string name, string password)
        {
            try
            {
                var userResponse = await _httpClient.GetAsync($"user/{name}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    return "Hasło lub login jest niepoprawne";
                }

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(userJson);

                if (user == null)
                {
                    return "Hasło lub login jest niepoprawne";
                }

                var keysResponse = await _httpClient.GetAsync($"encryptionkeys/{user.ID_User}");
                if (!keysResponse.IsSuccessStatusCode)
                {
                    return "Wystąpił problem z pobraniem kluczy szyfrowania.";
                }

                var keysJson = await keysResponse.Content.ReadAsStringAsync();
                var encryptionKeys = JsonConvert.DeserializeObject<EncryptionKeys>(keysJson);

                if (encryptionKeys == null)
                {
                    return "Wystąpił problem z pobraniem kluczy szyfrowania.";
                }

                string decryptedPassword = EntityClasses.AesManaged.Decryption(user.UserPassword, encryptionKeys.EncryptionKey, encryptionKeys.IV);

                if (!user.Is_active)
                {
                    return "Konto jest nieaktywne";
                }

                if (decryptedPassword != null && decryptedPassword == password)
                {
                    return Convert.ToString(user.ID_User);
                }
                else
                {
                    return "Hasło lub login jest niepoprawne";
                }
            }
            catch (Exception)
            {
                return "Wystąpił błąd podczas logowania";
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
    }
}