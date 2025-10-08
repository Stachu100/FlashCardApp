using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiszkiApp.EntityClasses
{
    public partial class ChangePassowrd : ObservableValidator
    {
        [ObservableProperty]
        [Required(ErrorMessage = "Stare hasło jest wymagane")]
        private string oldPassword;

        [ObservableProperty]
        [Required(ErrorMessage = "Nowe hasło jest wymagane")]
        [MinLength(8, ErrorMessage = "Hasło musi mieć conajmniej 8 znaków")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage = "Hasło musi zawierać dużą literę, liczbę oraz znak specjalny")]
        private string newPassword;

        [ObservableProperty]
        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane")] 
        private string repeatNewPassword;

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}
