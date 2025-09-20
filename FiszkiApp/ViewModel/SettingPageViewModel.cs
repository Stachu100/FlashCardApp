using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;

namespace FiszkiApp.ViewModel
{
    public partial class SettingsPageViewModel : ObservableValidator
    {
        private readonly DatabaseService _databaseService;
        private readonly AuthService _authService;

        public IAsyncRelayCommand DeleteDataCommand { get; }

        public ICommand ChangeThemeCommand => new Command<string>(theme =>
        {
            (Application.Current as App)?.SetTheme(theme);
            Preferences.Set("AppTheme", theme);
        });

        public SettingsPageViewModel()
        {
            _databaseService = App.Database;
            _authService = new AuthService();

            DeleteDataCommand = new AsyncRelayCommand(DeleteDataAsync);
        }

        private async Task DeleteDataAsync()
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Potwierdzenie",
                "Czy na pewno chcesz usun¹æ wszystkie dane?",
                "Tak",
                "Nie");

            if (!confirm)
            {
                return;
            }

            var (isAuthenticated, userIdString) = await _authService.IsAuthenticatedAsync();
            if (!isAuthenticated || !int.TryParse(userIdString, out int userId) || userId <= 0)
            {
                await Shell.Current.DisplayAlert("Error", $"Ups... Coœ posz³o nie tak.", "OK");
                return;
            }

            try
            {
                var categories = await _databaseService.GetCategoriesByUserIdAsync(userId);
                var categoryIds = categories.Select(c => c.IdCategory).ToList();

                foreach (var categoryId in categoryIds)
                {
                    await _databaseService.DeleteFlashcardsByCategoryId(categoryId);
                }

                foreach (var category in categories)
                {
                    await _databaseService.DeleteCategoryAsync(category);
                }

                // await _databaseService.ResetDatabaseAsync(); // USUWANIE I BUDOWANIE CA£EJ BAZY NA NOWO, WYKONANIE W DATABASESERVICE ZAKOMENTOWANE

                await Shell.Current.DisplayAlert("Sukces", "Dane zosta³y usuniête.", "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", $"Ups... Coœ posz³o nie tak.", "OK");
            }
        }
    }
}