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
    public partial class MainPageViewModel : ObservableValidator
    {
        private readonly DatabaseService _databaseService;
        private readonly CategoryPost _categoryPost;
        private readonly AuthService _authService;

        public MainPageViewModel()
        {
            _databaseService = App.Database;
            _categoryPost = new CategoryPost();
            _authService = new AuthService();
            Categories = new ObservableCollection<LocalCategoryTable>();
            ViewFlashcardsCommand = new AsyncRelayCommand<LocalCategoryTable>(ViewFlashcardsAsync);
            SendCategoryCommand = new AsyncRelayCommand<LocalCategoryTable>(SendCategoryAsync);
            DeleteCategoryCommand = new AsyncRelayCommand<LocalCategoryTable>(DeleteCategoryAsync);
            LoadCategoriesCommand = new AsyncRelayCommand(LoadCategoriesAsync);
            LoadCategoriesCommand.Execute(null);
            CategoryTappedCommand = new AsyncRelayCommand<LocalCategoryTable>(OnCategoryTappedAsync);
        }

        [ObservableProperty]
        private ObservableCollection<LocalCategoryTable> categories;

        [ObservableProperty]
        private LocalCategoryTable selectedCategory;

        public IAsyncRelayCommand LoadCategoriesCommand { get; }
        public IAsyncRelayCommand<LocalCategoryTable> SendCategoryCommand { get; }
        public IAsyncRelayCommand<LocalCategoryTable> DeleteCategoryCommand { get; }
        public IAsyncRelayCommand<LocalCategoryTable> ViewFlashcardsCommand { get; }
        public IAsyncRelayCommand<LocalCategoryTable> CategoryTappedCommand { get; }

        [RelayCommand]
        public async Task AddCategory()
        {
            await Shell.Current.GoToAsync("AddCategoryPage");
        }

        public async Task LoadCategoriesAsync()
        {
            var (isAuthenticated, userIdString) = await _authService.IsAuthenticatedAsync();

            if (isAuthenticated && int.TryParse(userIdString, out int userId) && userId > 0)
            {
                var categoriesFromDb = await _databaseService.GetCategoriesByUserIdAsync(userId);

                var countryUrls = App.CountriesDic.Countries;

                Categories.Clear();
                foreach (var category in categoriesFromDb)
                {
                    var frontFlag = countryUrls.FirstOrDefault(c => c.Country == category.FrontLanguage).Url;
                    var backFlag = countryUrls.FirstOrDefault(c => c.Country == category.BackLanguage).Url;

                    category.FrontFlagUrl = frontFlag;
                    category.BackFlagUrl = backFlag;

                    Categories.Add(category);
                }
            }
        }

        private async Task SendCategoryAsync(LocalCategoryTable category)
        {
            if (category != null)
            {
                var (isAuthenticated, userIdString) = await _authService.IsAuthenticatedAsync();

                if (isAuthenticated && int.TryParse(userIdString, out int userId) && userId > 0)
                {
                    var newCategory = new Category
                    {
                        UserID = userId,
                        CategoryName = category.CategoryName,
                        FrontLanguage = category.FrontLanguage,
                        BackLanguage = category.BackLanguage,
                        LanguageLevel = category.LanguageLevel
                    };

                    var flashcards = await _databaseService.GetFlashcardsByCategoryIdAsync(category.IdCategory);

                    var categoryPost = new CategoryPost();
                    var result = await categoryPost.AddCategoryAndFlashcardsAsync(newCategory, flashcards);

                    if (result)
                    {
                        category.IsSent = 1;
                        await _databaseService.UpdateCategoryAsync(category);
                        await LoadCategoriesAsync();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", $"Ups... Coś poszło nie tak.", "OK");
                    }
                }
            }
        }

        private async Task DeleteCategoryAsync(LocalCategoryTable category)
        {
            if (category != null)
            {
                await _databaseService.DeleteCategoryAsync(category);
                Categories.Remove(category);
            }
        }

        private async Task ViewFlashcardsAsync(LocalCategoryTable category)
        {
            if (category != null)
            {
                await Shell.Current.GoToAsync($"{nameof(AddFlashcardsPage)}?CategoryId={category.IdCategory}");
            }
        }

        private async Task OnCategoryTappedAsync(LocalCategoryTable selectedCategory)
        {
            if (selectedCategory != null)
            {
                await Shell.Current.GoToAsync($"{nameof(FlipCardPage)}?CategoryId={selectedCategory.IdCategory}");
            }
        }
    }
}