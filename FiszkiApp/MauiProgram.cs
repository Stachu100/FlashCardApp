using CommunityToolkit.Maui;
using FiszkiApp.Services;
using FiszkiApp.View;
using Microsoft.Extensions.Logging;

namespace FiszkiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<AuthService>();
            builder.Services.AddTransient<LoadingPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<FlashCardList>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<LookFlashCardPage>();
            builder.Services.AddTransient<AddFlashcardsPage>();
            builder.Services.AddTransient<AddCategoryPage>();
            builder.Services.AddTransient <PasswordChangePage>();
            builder.Services.AddTransient<FlipCardPage>();;

            return builder.Build();
        }
    }
}