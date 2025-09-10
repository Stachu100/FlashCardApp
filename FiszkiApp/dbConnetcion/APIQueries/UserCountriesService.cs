using System.Text;
using Newtonsoft.Json;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class UserCountriesService
    {
        private readonly HttpClient _httpClient;

        public UserCountriesService()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<List<UserCountries>> GetUserCountriesByUserIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"usercountries/user/{userId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var userCountries = JsonConvert.DeserializeObject<List<UserCountries>>(responseContent);

                return userCountries;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return new List<UserCountries>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return new List<UserCountries>();
            }
        }

        public async Task<bool> AddUserCountryAsync(UserCountries userCountry)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(userCountry);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("usercountries", content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserCountryAsync(int userId, int countryId)
        {
            try
            {
                var url = $"usercountries?userId={userId}&countryId={countryId}";

                var response = await _httpClient.DeleteAsync(url);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return false;
            }
        }
    }
}