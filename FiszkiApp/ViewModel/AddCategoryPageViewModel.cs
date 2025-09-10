using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.dbConnetcion.APIQueries;
using FiszkiApp.Services;
using System.Collections.ObjectModel;
using FiszkiApp.EntityClasses.Models;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FiszkiApp.ViewModel
{
    public partial class AddCategoryViewModel : ObservableValidator
    {
        private readonly CountriesDic _countriesDic;
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        public AddCategoryViewModel()
        {
            _countriesDic = new CountriesDic();
            _authService = new AuthService();
            _databaseService = App.Database;

            LanguageLevel = new ObservableCollection<string> { "Brak", "A1", "A2", "B1", "B2", "C1", "C2" };
            LoadLanguagesCommand = new AsyncRelayCommand(LoadLanguages);
            LoadLanguagesCommand.Execute(null);
        }

        [ObservableProperty]
        private string categoryName;

        [ObservableProperty]
        private string selectedLanguageLevel;

        [ObservableProperty]
        private string selectedFrontLanguage;

        [ObservableProperty]
        private string selectedBackLanguage;

        [ObservableProperty]
        private ObservableCollection<string> languageLevel;

        [ObservableProperty]
        private ObservableCollection<string> frontLanguages;

        [ObservableProperty]
        private ObservableCollection<string> backLanguages;

        [ObservableProperty]
        private ObservableCollection<string> _allLanguages;

        [ObservableProperty]
        private bool isBackLanguageEnabled;

        [ObservableProperty]
        private bool isFrontLanguageEmpty;

        public IAsyncRelayCommand LoadLanguagesCommand { get; }

        public IAsyncRelayCommand SubmitCategoryCommand => new AsyncRelayCommand(SubmitCategory);

        public IAsyncRelayCommand CancelCategoryCommand => new AsyncRelayCommand(CancelCategory);

        private async Task LoadLanguages()
        {
            var countries = await _countriesDic.GetCountriesWithFlagsAsync();
            _allLanguages = new ObservableCollection<string>(countries.Select(c => c.Country).ToList());

            FrontLanguages = new ObservableCollection<string>(_allLanguages);
            BackLanguages = new ObservableCollection<string>();
            IsBackLanguageEnabled = false;
            IsFrontLanguageEmpty = true;
        }

        private async Task SubmitCategory()
        {
            if (string.IsNullOrWhiteSpace(CategoryName) ||
                string.IsNullOrWhiteSpace(SelectedFrontLanguage) ||
                string.IsNullOrWhiteSpace(SelectedBackLanguage) ||
                string.IsNullOrWhiteSpace(SelectedLanguageLevel))
            {
                await Shell.Current.DisplayAlert("Błąd", "Wszystkie pola oznaczone gwiazdką (*) są wymagane.", "OK");
                return;
            }

            if (SelectedFrontLanguage == SelectedBackLanguage)
            {
                await Shell.Current.DisplayAlert("Błąd", "Język przodu i tyłu muszą być różne.", "OK");
                return;
            }

            var (isAuthenticated, userIdString) = await _authService.IsAuthenticatedAsync();
            if (!isAuthenticated || !int.TryParse(userIdString, out int userId) || userId <= 0)
            {
                await Shell.Current.DisplayAlert("Błąd", "Nie udało się pobrać identyfikatora użytkownika.", "OK");
                return;
            }

            var newCategory = new LocalCategoryTable
            {
                CategoryName = CategoryName,
                FrontLanguage = SelectedFrontLanguage,
                BackLanguage = SelectedBackLanguage,
                LanguageLevel = SelectedLanguageLevel,
                UserID = userId
            };

            await _databaseService.AddCategoryAsync(newCategory);
            ResetForm();

            await Shell.Current.GoToAsync("//MainPage");
        }

        public void OnSelectedFrontLanguageChanged()
        {
            if (SelectedFrontLanguage != null)
            {
                if (SelectedFrontLanguage == "Polska")
                {
                    BackLanguages = new ObservableCollection<string>(_allLanguages.Where(f => f != "Polska"));
                    SelectedBackLanguage = null;
                    IsBackLanguageEnabled = true;
                    IsFrontLanguageEmpty = false;
                }
                else
                {
                    BackLanguages = new ObservableCollection<string> { "Polska" };
                    SelectedBackLanguage = null;
                    IsBackLanguageEnabled = true;
                    IsFrontLanguageEmpty = false;
                }
            }              
        }

        private async Task CancelCategory()
        {
            ResetForm();

            await Shell.Current.GoToAsync("//MainPage");
        }

        private void ResetForm()
        {
            CategoryName = string.Empty;
            SelectedLanguageLevel = null;
            SelectedFrontLanguage = null;
            SelectedBackLanguage = null;
        }
    }
}