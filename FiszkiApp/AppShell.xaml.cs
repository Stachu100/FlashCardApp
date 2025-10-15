using FiszkiApp.Services;
using FiszkiApp.View;

namespace FiszkiApp
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;

        public AppShell()
        {
            InitializeComponent();


            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(FlashCardList), typeof(FlashCardList));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(AddFlashcardsPage), typeof(AddFlashcardsPage));
            Routing.RegisterRoute(nameof(AddCategoryPage), typeof(AddCategoryPage));
            Routing.RegisterRoute(nameof(FlipCardPage), typeof(FlipCardPage));
            Routing.RegisterRoute(nameof(LookFlashCardPage), typeof(LookFlashCardPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(PasswordChangePage), typeof(PasswordChangePage));
            Routing.RegisterRoute(nameof(QrLoginPage), typeof(QrLoginPage));

            BindingContext = this;

            _authService = new AuthService();
        }

        public Command LogoutCommand => new Command(async () => await LogoutAsync());

        public Command SettingsCommand => new Command(async () => await SettingsAsync());

        public Command ChangePassowrdCommand => new Command(async () => await ChangePasswordAsync());

        public Command QrLoginCommand => new Command(async () => await ChangeQrLoginAsync());

        private async Task LogoutAsync()
        {
            _authService.Logout();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task SettingsAsync()
        {
            Shell.Current.FlyoutIsPresented = false;
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async Task ChangePasswordAsync()
        {
            Shell.Current.FlyoutIsPresented = false;
            await Shell.Current.GoToAsync("PasswordChangePage");
        }

        private async Task ChangeQrLoginAsync()
        {
            Shell.Current.FlyoutIsPresented = false;
            await Shell.Current.GoToAsync("QrLoginPage");
        }
    }
}