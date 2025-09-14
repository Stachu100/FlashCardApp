using System.Text;
using Newtonsoft.Json;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class CategoryPost
    {
        private readonly HttpClient _httpClient;

        public CategoryPost()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<bool> AddCategoryAndFlashcardsAsync(Category category, List<LocalFlashcardTable> flashcards)
        {
            try
            {
                var jsonCategory = JsonConvert.SerializeObject(category);
                var contentCategory = new StringContent(jsonCategory, Encoding.UTF8, "application/json");
                var responseCategory = await _httpClient.PostAsync("Category", contentCategory);
                var responseCategoryContent = await responseCategory.Content.ReadAsStringAsync();

                if (responseCategory.IsSuccessStatusCode)
                {
                    var categoryResponse = JsonConvert.DeserializeObject<Category>(responseCategoryContent);

                    var newCategoryId = categoryResponse?.ID_Category;
                    if (newCategoryId.HasValue)
                    {
                        var flashcardsToSend = flashcards.Select(f => new FlashCard
                        {
                            ID_Category = newCategoryId.Value,
                            FrontFlashCard = f.FrontFlashCard,
                            BackFlashCard = f.BackFlashCard
                        }).ToList();

                        var jsonFlashcards = JsonConvert.SerializeObject(flashcardsToSend);
                        var contentFlashcards = new StringContent(jsonFlashcards, Encoding.UTF8, "application/json");
                        var responseFlashcards = await _httpClient.PostAsync("Flashcard/batch", contentFlashcards);
                        var responseFlashcardsContent = await responseFlashcards.Content.ReadAsStringAsync();

                        if (responseFlashcards.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}