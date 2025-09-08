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

        public ICommand SetColorCommand { get; }
        public ICommand SetTextColorCommand { get; }
        public IAsyncRelayCommand DeleteDataCommand { get; }

        public SettingsPageViewModel()
        {
            _databaseService = App.Database;
            _authService = new AuthService();

            SetColorCommand = new RelayCommand<string>(SetColor);
            SetTextColorCommand = new RelayCommand<string>(SetTextColor);
            DeleteDataCommand = new AsyncRelayCommand(DeleteDataAsync);
        }

        private void SetColor(string color)
        {
            Preferences.Set("FlashcardBackgroundColor", color);
        }

        private void SetTextColor(string color)
        {
            Preferences.Set("FlashcardTextColor", color);
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
                await Shell.Current.DisplayAlert("B³¹d", "Nie uda³o siê pobraæ ID u¿ytkownika.", "OK");
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

                await Shell.Current.DisplayAlert("Sukces", "Dane zosta³y usuniête.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("B³¹d", $"Wyst¹pi³ b³¹d podczas usuwania danych: {ex.Message}", "OK");
            }
        }
    }
}