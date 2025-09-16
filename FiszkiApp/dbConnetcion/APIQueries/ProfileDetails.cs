using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.Services;
using Newtonsoft.Json;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class ProfileDetails : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private UserDetails? _currentUser;

        public UserDetails? CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public ProfileDetails()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<UserDetails?> GetUserDetailsAsync(int userId, bool forceReload = false)
        {
            if (!forceReload && CurrentUser != null && CurrentUser.ID_User == userId)
                return CurrentUser;

            try
            {
                var response = await _httpClient.GetAsync($"userdetails/{userId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var userDetails = JsonConvert.DeserializeObject<UserDetails>(content);

                if (userDetails != null)
                    CurrentUser = userDetails;

                return CurrentUser;
            }
            catch
            {
                return CurrentUser;
            }
        }

        public async Task<bool> UpdateAvatarAsync(byte[] avatar)
        {
            if (CurrentUser == null) return false;

            try
            {
                var json = JsonConvert.SerializeObject(avatar);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"userdetails/{CurrentUser.ID_User}/avatar", content);

                if (response.IsSuccessStatusCode)
                {
                    CurrentUser.Avatar = avatar;
                    OnPropertyChanged(nameof(CurrentUser));
                }

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAvatarAsync()
        {
            if (CurrentUser == null) return false;

            try
            {
                var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"userdetails/{CurrentUser.ID_User}/delete-avatar", content);

                if (response.IsSuccessStatusCode)
                {
                    CurrentUser.Avatar = null;
                    OnPropertyChanged(nameof(CurrentUser));
                }

                return response.IsSuccessStatusCode;
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