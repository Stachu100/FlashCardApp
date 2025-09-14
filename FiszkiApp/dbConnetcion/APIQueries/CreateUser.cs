using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using FiszkiApp.EntityClasses;
using static FiszkiApp.EntityClasses.AesManaged;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    internal class CreateUser
    {
        private readonly HttpClient _httpClient;

        public CreateUser()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<string> UserInsertAsync(string name, byte[] encryptedPassword, string firstName, string lastName, string country,
                                                  string email, byte[] uploadedImage, bool isAcceptedPolicy, byte[] iv, byte[] key)
        {
            try
            {
                var emailCheckResponse = await _httpClient.GetAsync($"userdetails/check-email/{email}");
                if (emailCheckResponse.IsSuccessStatusCode && (await emailCheckResponse.Content.ReadAsStringAsync()) == "exists")
                {
                    return "Istnieje już użytkownik o podanym emailu";
                }

                var usernameCheckResponse = await _httpClient.GetAsync($"user/check-username/{name}");
                if (usernameCheckResponse.IsSuccessStatusCode && (await usernameCheckResponse.Content.ReadAsStringAsync()) == "exists")
                {
                    return "Istnieje już użytkownik o podanej nazwie";
                }

                var user = new User { UserName = name, UserPassword = encryptedPassword };
                var userContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                var userResponse = await _httpClient.PostAsync("User", userContent);

                if (!userResponse.IsSuccessStatusCode)
                {
                    return "Wystąpił błąd podczas rejestracji użytkownika";
                }

                var createdUser = JsonConvert.DeserializeObject<User>(await userResponse.Content.ReadAsStringAsync());
                int userId = createdUser.ID_User;

                var userDetails = new UserDetails
                {
                    ID_User = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Country = country,
                    Email = email,
                    Avatar = uploadedImage,
                    IsAcceptedPolicy = isAcceptedPolicy
                };
                var detailsContent = new StringContent(JsonConvert.SerializeObject(userDetails), Encoding.UTF8, "application/json");
                var detailsResponse = await _httpClient.PostAsync("userdetails", detailsContent);

                if (!detailsResponse.IsSuccessStatusCode)
                {
                    return "Wystąpił błąd podczas rejestracji szczegółów użytkownika";
                }

                var encryptionKeys = new EncryptionKeys
                {
                    ID_User = userId,
                    EncryptionKey = key,
                    IV = iv
                };

                var keysContent = new StringContent(JsonConvert.SerializeObject(encryptionKeys), Encoding.UTF8, "application/json");
                var keysResponse = await _httpClient.PostAsync("encryptionkeys", keysContent);

                if (!keysResponse.IsSuccessStatusCode)
                {
                    return "Wystąpił błąd podczas dodawania kluczy szyfrowania";
                }

                return "Rejestracja zakończyła się sukcesem";
            }
            catch (Exception)
            {
                return "Wystąpił błąd podczas rejestracji";
            }
        }
    }
}