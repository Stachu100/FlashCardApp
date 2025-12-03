using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    [QueryProperty(nameof(API_ID_Category), "API_ID_Category")]
    public partial class LookFlashCardPage : ContentPage
    {
        private LookFlashCardPageViewModel _viewModel;
        private int _idCategory;

        public int API_ID_Category
        {
            get => _idCategory;
            set
            {
                _idCategory = value;
            }
        }

        public LookFlashCardPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel == null)
            {
                _viewModel = new LookFlashCardPageViewModel(API_ID_Category);
                BindingContext = _viewModel;
            }
            if (_viewModel.Flashcards.Count == 0 && _viewModel.LoadFlashcardsCommand.CanExecute(null))
                await _viewModel.LoadFlashcardsCommand.ExecuteAsync(null);         
        }
    }
}