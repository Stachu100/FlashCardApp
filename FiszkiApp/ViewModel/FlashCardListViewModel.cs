using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.dbConnetcion.APIQueries;
using FiszkiApp.View;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using Microsoft.Maui.Controls;

namespace FiszkiApp.ViewModel
{
    public partial class FlashCardListViewModel : ObservableValidator
    {
        private readonly CategorySearchService _categorySearchService;
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;
        private readonly FlashCardService _flashCardService;

        public FlashCardListViewModel()
        {
            _categorySearchService = new CategorySearchService();
            _databaseService = App.Database;
            _authService = new AuthService();
            _flashCardService = new FlashCardService();

            LanguageLevels = new ObservableCollection<string> { "Brak", "A1", "A2", "B1", "B2", "C1", "C2" };
            UserLanguages = new ObservableCollection<string>();

            SearchCommand = new AsyncRelayCommand(SearchCategoriesAsync);
            AddToLocalCommand = new AsyncRelayCommand<LocalCategoryTable>(AddToLocalAsync);
            LookFlashCardTappedCommand = new AsyncRelayCommand<LocalCategoryTable>(OnLookFlashCardTappedAsync);

        }

        [ObservableProperty]
        private ObservableCollection<LocalCategoryTable> searchResults = new();

        [ObservableProperty]
        private string categorySearch;

        [ObservableProperty]
        private string userSearch;

        [ObservableProperty]
        private string selectedLanguageLevel;

        [ObservableProperty]
        private string selectedLanguage;

        [ObservableProperty]
        private ObservableCollection<string> userLanguages;

        public ObservableCollection<string> LanguageLevels { get; }

        public IAsyncRelayCommand SearchCommand { get; }
        public IAsyncRelayCommand<LocalCategoryTable> AddToLocalCommand { get; }

        public IAsyncRelayCommand<LocalCategoryTable> LookFlashCardTappedCommand { get; }

        [RelayCommand]
        private void ClearCategorySearch() => CategorySearch = string.Empty;

        [RelayCommand]
        private void ClearUserSearch() => UserSearch = string.Empty;

        [RelayCommand]
        private void ClearSelectedLanguageLevel() => SelectedLanguageLevel = null;

        [RelayCommand]
        private void ClearSelectedLanguage() => SelectedLanguage = null;

        private async Task SearchCategoriesAsync()
        {
            if (UserLanguages == null || UserLanguages.Count == 0)
            {
                return;
            }

            var categories = await _categorySearchService.SearchCategoriesAsync(
                CategorySearch,
                UserSearch,
                SelectedLanguageLevel,
                SelectedLanguage);

            SearchResults.Clear();

            foreach (var category in categories)
            {
                if (UserLanguages.Contains(category.FrontLanguage) || UserLanguages.Contains(category.BackLanguage))
                {
                    SearchResults.Add(new LocalCategoryTable
                    {
                        CategoryName = category.CategoryName,
                        FrontLanguage = category.FrontLanguage,
                        BackLanguage = category.BackLanguage,
                        LanguageLevel = category.LanguageLevel,
                        UserID = category.UserID,
                        IsSent = 1,
                        API_ID_Category = category.ID_Category
                    });
                }
            }
        }

        private async Task AddToLocalAsync(LocalCategoryTable category)
        {
            if (category != null)
            {
                var (isAuthenticated, userIdString) = await _authService.IsAuthenticatedAsync();

                if (isAuthenticated && int.TryParse(userIdString, out int userId) && userId > 0)
                {
                    category.UserID = userId;

                    var localCategoryId = await _databaseService.AddCategoryAndGetIdAsync(category);

                    if (localCategoryId > 0 && category.API_ID_Category > 0)
                    {
                        var apiFlashcards = await _flashCardService.GetFlashCardsByCategoryAsync(category.API_ID_Category);

                        foreach (var flashcard in apiFlashcards)
                        {
                            var localFlashcard = new LocalFlashcardTable
                            {
                                FrontFlashCard = flashcard.FrontFlashCard,
                                BackFlashCard = flashcard.BackFlashCard,
                                IdCategory = localCategoryId
                            };

                            await _databaseService.AddFlashcardAsync(localFlashcard);
                        }
                    }

                    var categoryToRemove = SearchResults.FirstOrDefault(c => c.API_ID_Category == category.API_ID_Category);

                    if (categoryToRemove != null)
                    {
                        SearchResults.Remove(categoryToRemove);

                        SearchResults = new ObservableCollection<LocalCategoryTable>(SearchResults);
                    }
                }
            }
        }

        public async Task LoadUserLanguagesAsync()
        {
            var (isAuthenticated, userIdString) = await _authService.IsAuthenticatedAsync();

            if (isAuthenticated && int.TryParse(userIdString, out int userId) && userId > 0)
            {
                var userCountriesService = new UserCountriesService();
                var userCountries = await userCountriesService.GetUserCountriesByUserIdAsync(userId);

                if (userCountries != null)
                {
                    var countriesDic = new CountriesDic();
                    var allCountries = await countriesDic.GetCountriesWithFlagsAsync();

                    var filteredLanguages = allCountries
                        .Where(c => userCountries.Any(uc => uc.ID_Country == c.ID_Country))
                        .Select(c => c.Country)
                        .ToList();

                    UserLanguages.Clear();
                    foreach (var language in filteredLanguages)
                    {
                        UserLanguages.Add(language);
                    }
                }
            }
        }

        private async Task OnLookFlashCardTappedAsync(LocalCategoryTable selectedCategory)
        {
            if (selectedCategory != null)
            {
                await Shell.Current.GoToAsync($"{nameof(LookFlashCardPage)}?API_ID_Category={selectedCategory.API_ID_Category}");
            }
        }
    }
}