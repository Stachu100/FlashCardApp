using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.ComponentModel.DataAnnotations;
using FiszkiApp.Services;
using FiszkiApp.View;

namespace FiszkiApp.ViewModel
{
    public partial class LoginPageViewModel : ObservableValidator
    {
        private readonly AuthService _authService;
        public LoginPageViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [ObservableProperty]
        [Required(ErrorMessage = "Nazwa wymagane")]
        private string userName;

        [ObservableProperty]
        [Required(ErrorMessage = "Hasło wymagane")]
        public string userPassword;

        [ObservableProperty]
        private string errorMessages;

        [ObservableProperty]
        private bool rememberMe;

        [RelayCommand]
        public async Task LoginCommand()
        {
            ValidateAllProperties();
            if (HasErrors)
            {
                var errors = new List<string>();
                foreach (var propertyName in new[] { nameof(UserName), nameof(UserPassword)})
                {             
                    foreach (var error in GetErrors(propertyName))
                    {
                        if (error is ValidationResult validationResult)
                        {
                            errors.Add(validationResult.ErrorMessage);
                        }
                    }
                }

                ErrorMessages = string.Join("\n", errors);
            }
            else
            {
                string result = await _authService.Login(UserName, UserPassword, RememberMe);

                if (result != "Hasło lub login jest niepoprawne" && result != "Wystąpił błąd podczas logowania" && result != "Konto jest nie aktywne")
                {
                    int userId = int.Parse(result);

                    await App.ProfileDetails.GetUserDetailsAsync(userId);
                    await App.UserCountriesService.GetUserCountriesByUserIdAsync(userId);

                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    ErrorMessages = result;
                }
            }
        }
    }
}