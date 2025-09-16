using FiszkiApp.Services;

namespace FiszkiApp.View;

public partial class LoadingPage : ContentPage
{
    private readonly AuthService _authService;
    public LoadingPage()
	{
		InitializeComponent();
        _authService = new AuthService();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var (isAuthenticated, userId) = await _authService.IsAuthenticatedAsync();

        if (isAuthenticated && int.TryParse(userId, out int parsedUserId))
        {
            await App.CountriesDic.GetCountriesWithFlagsAsync();
            await App.ProfileDetails.GetUserDetailsAsync(parsedUserId);
            await App.UserCountriesService.GetUserCountriesByUserIdAsync(parsedUserId);

            await Task.Delay(500);
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            await App.CountriesDic.GetCountriesWithFlagsAsync();

            await Task.Delay(500);
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}