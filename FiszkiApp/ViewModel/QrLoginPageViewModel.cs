using CommunityToolkit.Mvvm.ComponentModel;
using FiszkiApp.Services;
using FiszkiApp.dbConnetcion.APIQueries;
using System.Windows.Input;

namespace FiszkiApp.ViewModel
{
    public partial class QrLoginPageViewModel : ObservableObject
    {
        private readonly LogInQuery _logInQuery;
        private readonly AuthService _authService;

        [ObservableProperty]
        private string statusMessage = "Czekam na skanowanie...";

        public QrLoginPageViewModel()
        {
            _logInQuery = new LogInQuery();
            _authService = new AuthService();
        }

        public ICommand TestQrCommand => new Command(async () => await OnQrDetectedAsync("TEST-TOKEN-1234"));


        public async Task OnQrDetectedAsync(string qrValue)
        {
            StatusMessage = "Trwa weryfikacja kodu QR...";

            var (isAuthenticated, userId) = await _authService.IsAuthenticatedAsync();

            if (isAuthenticated && int.TryParse(userId, out int parsedUserId))
            {
                var resultMessage = await _logInQuery.LoginByQrCodeAsync(qrValue, parsedUserId);
                StatusMessage = resultMessage;

                if (resultMessage == "Zalogowano pomyślnie")
                {
                    await Task.Delay(1000);
                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
        }
    }
}