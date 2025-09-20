using Android.App;
using Android.Content.PM;
using Android.OS;
using FiszkiApp.Resources.Styles.Colors;
using Microsoft.Maui.Platform;

namespace FiszkiApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var theme = Preferences.Get("AppTheme", "Pink");

            ResourceDictionary dict = theme switch
            {
                "Pink" => new Pink(),
                "Blue" => new Blue(),
                "Green" => new Green(),
                _ => new Pink()
            };

            if (dict.TryGetValue("StatusBarColor", out var colorObj) && colorObj is Color statusBarColor)
            {
                Window?.SetStatusBarColor(statusBarColor.ToPlatform());
            }
        }
    }
}
