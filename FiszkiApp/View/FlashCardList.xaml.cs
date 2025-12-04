using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    public partial class FlashCardList : ContentPage
    {
        private FlashCardListViewModel _viewModel;
        public FlashCardList()
        {
            InitializeComponent();
            _viewModel = new FlashCardListViewModel();
            BindingContext = _viewModel;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadUserLanguagesAsync();
        }
    }
}