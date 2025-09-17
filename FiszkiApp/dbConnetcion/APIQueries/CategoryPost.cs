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

        public async Task<int?> AddCategoryAndFlashcardsAsync(Category category, List<LocalFlashcardTable> flashcards)
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

                    if (newCategoryId > 0)
                    {
                        if (flashcards != null && flashcards.Any())
                        {
                            var flashcardsToSend = flashcards.Select(f => new FlashCard
                            {
                                ID_Category = newCategoryId.Value,
                                FrontFlashCard = f.FrontFlashCard,
                                BackFlashCard = f.BackFlashCard
                            }).ToList();

                            var jsonFlashcards = JsonConvert.SerializeObject(flashcardsToSend);
                            var contentFlashcards = new StringContent(jsonFlashcards, Encoding.UTF8, "application/json");
                            await _httpClient.PostAsync("Flashcard/batch", contentFlashcards);
                        }

                        return newCategoryId.Value;
                    }
                }
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}