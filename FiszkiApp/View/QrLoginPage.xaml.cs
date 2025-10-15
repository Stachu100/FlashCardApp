using ZXing.Net.Maui;
using FiszkiApp.ViewModel;

namespace FiszkiApp.View
{
    public partial class QrLoginPage : ContentPage
    {
        public QrLoginPage()
        {
            InitializeComponent();
            BindingContext = new QrLoginPageViewModel();
        }

        private void CameraView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var first = e.Results?.FirstOrDefault();
            if (first == null)
                return;

            Dispatcher.Dispatch(async () =>
            {
                var vm = BindingContext as QrLoginPageViewModel;
                if (vm != null)
                    await vm.OnQrDetectedAsync(first.Value);

                cameraView.IsDetecting = false;
            });
        }
    }
}