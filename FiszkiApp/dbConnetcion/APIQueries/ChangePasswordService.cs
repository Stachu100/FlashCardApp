using FiszkiApp.EntityClasses.Models;
using FiszkiApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FiszkiApp.EntityClasses.AesManaged;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class ChangePasswordService
    {
        private readonly HttpClient _httpClient;

        public ChangePasswordService()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<bool> UpdatePasswordAsync(int UserId, EncryptionResult encryptionResult)
        {
            try
            {


                var requestBody = new EncryptionKeys
                {
                    ID_User = UserId,
                    EncryptionKey = encryptionResult.Key,
                    IV = encryptionResult.IV
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var responseEncryption = await _httpClient.PutAsync($"EncryptionKeys/{UserId}/ChangeEncryption", content);

                if (responseEncryption.IsSuccessStatusCode)
                {
                    var jsonUser = JsonConvert.SerializeObject(encryptionResult.EncryptedData);
                    var contentUser = new StringContent(jsonUser, System.Text.Encoding.UTF8, "application/json");
                    var responseUser = await _httpClient.PutAsync($"User/{UserId}/ChangePassword", contentUser);

                    return responseUser.IsSuccessStatusCode;

                }
                throw new Exception("Nie udało się zminić hasła");
            }
            catch
            {
                return false;
            }
        }
    }
}
