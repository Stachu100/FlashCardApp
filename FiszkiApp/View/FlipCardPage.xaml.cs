using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    [QueryProperty(nameof(CategoryId), "CategoryId")]
    public partial class FlipCardPage : ContentPage
    {
        private int _categoryId;
        public int CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnCategoryIdSet();
            }
        }

        public FlipCardPage()
        {
            InitializeComponent();
        }

        private void OnCategoryIdSet()
        {
            BindingContext = new FlipCardPageViewModel(CategoryId);
        }

        private bool _isAnimating = false;
        private bool _isCardAnimating = false;

        private async void OnFlipCardTapped(object sender, EventArgs e)
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var flipView = FlashCardFrame;

            var viewModel = BindingContext as FlipCardPageViewModel;

            await Task.WhenAll(
                flipView.ScaleXTo(0, 400, Easing.CubicIn),
                flipView.FadeTo(0.6, 400, Easing.CubicIn)
            );

            if (viewModel != null)
            {
                viewModel.IsFrontVisible = !viewModel.IsFrontVisible;
                viewModel.IsBackVisible = !viewModel.IsBackVisible;
            }

            await Task.WhenAll(
                flipView.ScaleXTo(1, 400, Easing.CubicOut),
                flipView.FadeTo(1, 400, Easing.CubicOut)
            );

            _isAnimating = false;
        }

        private async Task AnimateCardChange(Func<Task> updateCardAction)
        {
            if (_isCardAnimating) return;
            _isCardAnimating = true;

            var flipView = FlashCardFrame;

            FrontCard.Opacity = 0;

            await Task.WhenAll(
                flipView.ScaleTo(0, 300, Easing.CubicIn),
                flipView.FadeTo(0.3, 300, Easing.CubicIn)
            );

            if (updateCardAction != null)
                await updateCardAction();

            FrontCard.Opacity = 1;

            await Task.WhenAll(
                flipView.ScaleTo(1, 300, Easing.CubicOut),
                flipView.FadeTo(1, 300, Easing.CubicOut)
            );

            _isCardAnimating = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var savedColor = Preferences.Get("FlashcardBackgroundColor", "#512BD4");

            FlashCardFrame.BackgroundColor = Color.FromArgb(savedColor);

            var savedTextColor = Preferences.Get("FlashcardTextColor", "#000000");

            FrontCard.TextColor = Color.FromArgb(savedTextColor);
            BackCard.TextColor = Color.FromArgb(savedTextColor);
        }

        private void FrontCard_SizeChanged(object sender, EventArgs e)
        {
            if (FrontCard == null || string.IsNullOrEmpty(FrontCard.Text) || FlashCardFrame == null)
                return;

            double maxFontSize = 100;
            double minFontSize = 10;
            double fontSize = maxFontSize;
            FrontCard.FontSize = fontSize;

            double maxWidth = FlashCardFrame.Width - FlashCardFrame.Padding.Left - FlashCardFrame.Padding.Right;
            double maxHeight = FlashCardFrame.Height - FlashCardFrame.Padding.Top - FlashCardFrame.Padding.Bottom;

            while (fontSize > minFontSize)
            {
                FrontCard.FontSize = fontSize;
                var request = FrontCard.Measure(double.PositiveInfinity, double.PositiveInfinity);

                if (request.Request.Width <= maxWidth && request.Request.Height <= maxHeight)
                    break;

                fontSize -= 1;
            }
        }

        private void BackCard_SizeChanged(object sender, EventArgs e)
        {
            if (BackCard == null || string.IsNullOrEmpty(BackCard.Text) || FlashCardFrame == null)
                return;

            double maxFontSize = 100;
            double minFontSize = 10;
            double fontSize = maxFontSize;
            BackCard.FontSize = fontSize;

            double maxWidth = FlashCardFrame.Width - FlashCardFrame.Padding.Left - FlashCardFrame.Padding.Right;
            double maxHeight = FlashCardFrame.Height - FlashCardFrame.Padding.Top - FlashCardFrame.Padding.Bottom;

            while (fontSize > minFontSize)
            {
                BackCard.FontSize = fontSize;
                var request = BackCard.Measure(double.PositiveInfinity, double.PositiveInfinity);

                if (request.Request.Width <= maxWidth && request.Request.Height <= maxHeight)
                    break;

                fontSize -= 1;
            }
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as FlipCardPageViewModel;
            if (viewModel != null)
            {
                await AnimateCardChange(async () =>
                {
                    await AnimateCardChange(() => viewModel.NextFlashcardAsync());
                });
            }
        }

        private async void OnPreviousButtonClicked(object sender, EventArgs e)
        {
            var viewModel = BindingContext as FlipCardPageViewModel;
            if (viewModel != null)
            {
                await AnimateCardChange(async () =>
                {
                    await AnimateCardChange(() => viewModel.PreviousFlashcardAsync());
                });
            }
        }
    }
}