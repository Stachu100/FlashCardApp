using System.Text;
using Newtonsoft.Json;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class UserCountriesService : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private List<UserCountries>? _currentUserCountries;

        public List<UserCountries>? CurrentUserCountries
        {
            get => _currentUserCountries;
            private set
            {
                _currentUserCountries = value;
                OnPropertyChanged();
            }
        }

        public void ResetCache()
        {
            _currentUserCountries = null;
            OnPropertyChanged(nameof(CurrentUserCountries));
        }

        public UserCountriesService()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<List<UserCountries>> GetUserCountriesByUserIdAsync(int userId, bool forceReload = false)
        {
            if (!forceReload && CurrentUserCountries != null)
                return CurrentUserCountries;

            try
            {
                var response = await _httpClient.GetAsync($"usercountries/user/{userId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var userCountries = JsonConvert.DeserializeObject<List<UserCountries>>(responseContent) ?? new List<UserCountries>();

                CurrentUserCountries = userCountries;

                return CurrentUserCountries;
            }
            catch
            {
                return CurrentUserCountries ?? new List<UserCountries>();
            }
        }

        public async Task<bool> AddUserCountryAsync(UserCountries userCountry)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(userCountry);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("usercountries", content);

                if (response.IsSuccessStatusCode)
                {
                    if (CurrentUserCountries == null)
                    {
                        CurrentUserCountries = new List<UserCountries>();
                    }                        

                    CurrentUserCountries.Add(userCountry);
                    OnPropertyChanged(nameof(CurrentUserCountries));
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserCountryAsync(int userId, int countryId)
        {
            try
            {
                var url = $"usercountries?userId={userId}&countryId={countryId}";
                var response = await _httpClient.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    CurrentUserCountries?.RemoveAll(c => c.ID_Country == countryId);
                    OnPropertyChanged(nameof(CurrentUserCountries));
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}