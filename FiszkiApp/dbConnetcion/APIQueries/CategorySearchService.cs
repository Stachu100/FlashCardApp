using System.Text;
using Newtonsoft.Json;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.Services;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class CategorySearchService
    {
        private readonly HttpClient _httpClient;

        public CategorySearchService()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<List<Category>> SearchCategoriesAsync(string categoryName, string userName, string languageLevel, string userLanguage)
        {
            try
            {
                var filters = new
                {
                    CategoryName = string.IsNullOrWhiteSpace(categoryName) ? null : categoryName,
                    UserName = string.IsNullOrWhiteSpace(userName) ? null : userName,
                    LanguageLevel = string.IsNullOrWhiteSpace(languageLevel) ? null : languageLevel,
                    UserLanguage = userLanguage != null && userLanguage.Any() ? userLanguage : null
                };

                if (string.IsNullOrEmpty(categoryName) && string.IsNullOrEmpty(userName) &&
                    string.IsNullOrEmpty(languageLevel) && (userLanguage == null || !userLanguage.Any()))
                {
                    throw new ArgumentException("At least one filter must be provided.");
                }

                var json = JsonConvert.SerializeObject(filters);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("CategoryList/search", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var categories = JsonConvert.DeserializeObject<List<Category>>(responseContent);
                    return categories ?? new List<Category>();
                }
                else
                {
                    Console.WriteLine($"Błąd API: {response.StatusCode} - {responseContent}");
                    return new List<Category>();
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                return new List<Category>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return new List<Category>();
            }
        }
    }
}