using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    [QueryProperty(nameof(API_ID_Category), "API_ID_Category")]
    public partial class LookFlashCardPage : ContentPage
    {
        private int _idCategory;
        public int API_ID_Category
        {
            get => _idCategory;
            set
            {
                _idCategory = value;
                OnCategoryIdSet();
            }
        }

        public LookFlashCardPage()
        {
            InitializeComponent();
        }

        private void OnCategoryIdSet()
        {
            BindingContext = new LookFlashCardPageViewModel(API_ID_Category);
        }
    }
}