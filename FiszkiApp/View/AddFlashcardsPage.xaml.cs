using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    [QueryProperty(nameof(CategoryId), "CategoryId")]
    public partial class AddFlashcardsPage : ContentPage
    {
        private AddFlashcardsPageViewModel _viewModel;
        private int _categoryId;

        public int CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
            }
        }

        public AddFlashcardsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new AddFlashcardsPageViewModel(_categoryId);
                BindingContext = _viewModel;
            }
            if (_viewModel.LoadFlashcardsCommand.CanExecute(null))
                await _viewModel.LoadFlashcardsCommand.ExecuteAsync(null);
        }
    }
}