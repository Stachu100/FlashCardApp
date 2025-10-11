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

        private void FrontCard_SizeChanged(object sender, EventArgs e)
        {
            AdjustFontFlexible(FrontCard, FlashCardFrame);
        }

        private void BackCard_SizeChanged(object sender, EventArgs e)
        {
            AdjustFontFlexible(BackCard, FlashCardFrame);
        }

        private void AdjustFontFlexible(Label label, Frame frame)
        {
            if (label == null || string.IsNullOrEmpty(label.Text) || frame == null)
                return;

            double maxFont = 100;
            double minFont = 38;
            int maxChars = 50;

            int length = label.Text.Length;

            double fontSize;
            if (length <= 5)
                fontSize = maxFont;
            else if (length >= maxChars)
                fontSize = minFont;
            else
                fontSize = maxFont - ((length - 5) / (double)(maxChars - 5)) * (maxFont - minFont);

            label.FontSize = fontSize;

            double maxWidth = frame.Width - frame.Padding.Left - frame.Padding.Right;
            double maxHeight = frame.Height - frame.Padding.Top - frame.Padding.Bottom;

            var request = label.Measure(double.PositiveInfinity, double.PositiveInfinity);
            while ((request.Request.Width > maxWidth || request.Request.Height > maxHeight) && fontSize > minFont)
            {
                fontSize -= 1;
                label.FontSize = fontSize;
                request = label.Measure(double.PositiveInfinity, double.PositiveInfinity);
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