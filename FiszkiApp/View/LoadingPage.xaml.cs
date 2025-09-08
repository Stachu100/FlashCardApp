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

        await Task.Delay(1000);

        var (isAuthenticated, userID) = await _authService.IsAuthenticatedAsync();

        if (isAuthenticated)
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        else
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}