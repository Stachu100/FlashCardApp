using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FiszkiApp.Services;
using FiszkiApp.View;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using FiszkiApp.EntityClasses;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.dbConnetcion.APIQueries;

namespace FiszkiApp.ViewModel
{
    public partial class ProfileViewModel : MainViewModel
    {
        private readonly AuthService _authService;

        public IAsyncRelayCommand LoadCountriesCommand { get; }
        public IAsyncRelayCommand LoadCountriesUrlCommand { get; }
        public Command<object> DeleteCommand { get; set; }

        public int intUserId;

        private List<(int ID_Country, string Country, string Url)> countriesWithUrl;
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();

        [ObservableProperty]

        private byte[] imageAsBytes;

        [ObservableProperty]
        private string user;

        [ObservableProperty]
        private string country;

        [ObservableProperty]
        private ObservableCollection<string> countryPicker;

        [ObservableProperty]
        private string countryPicked;
        partial void OnCountryPickedChanged(string value)
        {
            var isAny = Items.FirstOrDefault(x => x.Name == value);

            if (isAny == null)
            {
                var result = countriesWithUrl.FirstOrDefault(x => x.Country == value);
                if (!string.IsNullOrEmpty(result.Country))
                {

                    AddItem(true ,intUserId, result.ID_Country ,result.Country, result.Url);
                }
            }
            CountryPicked = null;
        }

        public ProfileViewModel(AuthService authService)
        {
            _authService = authService;
            LoadCountriesCommand = new AsyncRelayCommand(LoadCountry);
            LoadCountriesCommand.Execute(null);
            DeleteCommand = new Command<object>(OnTapped);
        }

        private async void OnTapped(object obj)
        {
            if (obj is Item item)
            {
                Items.Remove(item);
                var service = new UserCountriesService();
                bool isDeleted = await service.DeleteUserCountryAsync(intUserId, item.ID_Country);
            }
        }

        public async Task OnNavigatedTo(NavigationEventArgs args)
        {
            try
            {
                var (isAuthenticated, userID) = await _authService.IsAuthenticatedAsync();
                intUserId = Convert.ToInt32(userID);
                var profileDetails = new ProfileDetails();

                var userDetails = await profileDetails.GetUserDetailsAsync(intUserId);

                if (userDetails != null)
                {
                    User = $"{userDetails.FirstName} {userDetails.LastName}";
                    Country = "Kraj pochodzenia: " + userDetails.Country;

                    if (userDetails.Avatar != null && userDetails.Avatar.Length > 0)
                    {
                        ImageAsBytes = userDetails.Avatar;
                    }
                }
                var service = new UserCountriesService();
                var userCountries = await service.GetUserCountriesByUserIdAsync(intUserId);
                if (userCountries != null && Items.Count == 0)
                {
                    var countriesDic = new CountriesDic();
                    var countries = await countriesDic.GetCountriesWithFlagsAsync();

                    foreach (var userCountry in userCountries)
                    {
                        var country = countries.FirstOrDefault(c => c.ID_Country == userCountry.ID_Country);

                        if (country != null)
                        {
                            AddItem(false, null, country.ID_Country, country.Country, country.Url);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd podczas pobierania szczegółów użytkownika: {ex.Message}");
            }
        }

        private async Task LoadCountry()
        {
            var countriesDic = new CountriesDic();
            var countries = await countriesDic.GetCountriesWithFlagsAsync();

            countriesWithUrl = countries.Select(c => (c.ID_Country ,c.Country, c.Url)).ToList();
            CountryPicker = new ObservableCollection<string>(countries.Select(c => c.Country));
        }

        [RelayCommand]
        public async Task LogoutCommand(AuthService authService)
        {
            _authService.Logout();
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }

        private async void AddItem(bool AddToDB, int? UserId, int CountryId, string name, string imageName)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (AddToDB)
                {
                    var newUserCountry = new UserCountries
                    {
                        ID_User = UserId.Value,
                        ID_Country = CountryId
                    };
                    var service = new UserCountriesService();
                    var isAdded = await service.AddUserCountryAsync(newUserCountry);
                }               

                ImageSource imgSource = ImageSource.FromFile(imageName);
                var item = new Item
                    { ID_Country = CountryId,
                      Name = name,
                      Image = imgSource };    
                
                Items.Add(item);
                
            }
        }
    }
}