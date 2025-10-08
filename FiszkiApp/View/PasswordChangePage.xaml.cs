using FiszkiApp.Services;
using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    public partial class PasswordChangePage : ContentPage
    {
        private readonly AuthService _authService;
        public PasswordChangePage(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            var changePasswordViewModel = new ChangePasswordViewModel(_authService);
            BindingContext = changePasswordViewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();


            if (BindingContext is ProfileViewModel viewModel)
            {
                await viewModel.OnNavigatedTo(null);
            }
        }
    }
}