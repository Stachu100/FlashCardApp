using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is MainPageViewModel viewModel)
            {
                await viewModel.LoadCategoriesAsync();
                await viewModel.LoadAvatarAsync();
            }
        }
    }
}