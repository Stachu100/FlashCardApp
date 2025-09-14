using System.Threading.Tasks;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.Services;
using Newtonsoft.Json;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class ProfileDetails
    {
        private readonly HttpClient _httpClient;

        public ProfileDetails()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<UserDetails?> GetUserDetailsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"userdetails/{userId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var userDetails = JsonConvert.DeserializeObject<UserDetails>(responseContent);

                return userDetails;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Błąd HTTP: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateAvatarAsync(int userId, byte[] avatar)
        {
            try
            {
                var json = JsonConvert.SerializeObject(avatar);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"userdetails/{userId}/avatar", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd przy aktualizacji avatara: {ex.Message}");
                return false;
            }
        }
    }
}