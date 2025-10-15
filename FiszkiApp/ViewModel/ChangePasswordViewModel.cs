using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.dbConnetcion.APIQueries;
using FiszkiApp.EntityClasses;
using FiszkiApp.Services;
using System.ComponentModel.DataAnnotations;
using static FiszkiApp.EntityClasses.AesManaged;

namespace FiszkiApp.ViewModel
{
    public partial class ChangePasswordViewModel : ObservableValidator
    {
        private ChangePassowrd changePassowrd;
        private readonly AuthService _authService;
        private byte[] encryptedPassword;

        public ChangePassowrd ChangePassowrd
        {
            get => changePassowrd;
            set => SetProperty(ref changePassowrd, value);
        }
        public ChangePasswordViewModel(AuthService authService)
        {
            changePassowrd = new ChangePassowrd();
            _authService = authService;
        }

        [ObservableProperty]
        private string errorMessages;

        [RelayCommand]
        public async Task ChangePassword()
        {
            ErrorMessages = null;
            ChangePassowrd.Validate();
            if (ChangePassowrd.HasErrors)
            {
                var errors = new List<string>();
                foreach (var propertyName in new[] { nameof(ChangePassowrd.OldPassword), nameof(ChangePassowrd.NewPassword), nameof(ChangePassowrd.RepeatNewPassword) })
                {
                    foreach (var error in changePassowrd.GetErrors(propertyName))
                    {
                        if (error is ValidationResult validationResult)
                        {
                            errors.Add(validationResult.ErrorMessage);
                        }
                    }
                }
                ErrorMessages = string.Join("\n", errors);


            }

            if (ChangePassowrd.NewPassword != ChangePassowrd.RepeatNewPassword && !string.IsNullOrEmpty(ChangePassowrd.NewPassword) && !string.IsNullOrEmpty(ChangePassowrd.RepeatNewPassword))
            {
                ErrorMessages = "Nowe hasło i powtórzone nowe hasło nie są takie same.";
                return;
            }
            if (ChangePassowrd.OldPassword == ChangePassowrd.NewPassword && !string.IsNullOrEmpty(ChangePassowrd.NewPassword) && !string.IsNullOrEmpty(ChangePassowrd.OldPassword))
            {
                ErrorMessages = "Nowe hasło nie może być takie samo jak stare hasło.";
                return;
            }
            var UserName = Preferences.Default.Get("UserName", "");


            var resultLoginin = await _authService.Login(UserName, ChangePassowrd.OldPassword, true);

            if (resultLoginin.Message == "Hasło lub login jest niepoprawne" || resultLoginin.Message == "Wystąpił błąd podczas logowania" || resultLoginin.Message == "Konto jest nieaktywne")
            {
                ErrorMessages = "Stare hasło nie jest poprawne";
            }
            if(string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorMessages = string.Empty;
                var UserId = Preferences.Default.Get("UserId", "");
                int UserIdint = int.Parse(UserId);

                EncryptionResult encryptionResult = AesManaged.Encryption((string)changePassowrd.NewPassword);
                encryptedPassword = encryptionResult.EncryptedData;

                var ChangePasswordService = new ChangePasswordService();
                bool result = await ChangePasswordService.UpdatePasswordAsync(UserIdint, encryptionResult);
                if (result) await Application.Current.MainPage.DisplayAlert("Sukcess","Zmiana hasła przebiegła pomyślnie", "OK");
            }
        }
    }
}
