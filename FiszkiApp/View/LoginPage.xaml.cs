using FiszkiApp.Services;
using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
	public partial class LoginPage : ContentPage
	{
        private readonly AuthService _authService;
        public LoginPage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            var loginPageViewModel = new LoginPageViewModel(_authService);
            BindingContext = loginPageViewModel;
        }

        private async void OnLabelTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}