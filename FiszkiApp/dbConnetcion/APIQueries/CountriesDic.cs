using System.Threading.Tasks;
using FiszkiApp.Services;
using FiszkiApp.EntityClasses.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FiszkiApp.dbConnetcion.APIQueries
{
    public class CountriesDic
    {
        private readonly HttpClient _httpClient;

        public ObservableCollection<Countries> Countries { get; } = new ObservableCollection<Countries>();

        public CountriesDic()
        {
            _httpClient = HttpClientService.Instance.HttpClient;
        }

        public async Task<List<Countries>> GetCountriesWithFlagsAsync(bool forceReload = false)
        {
            if (!forceReload && Countries.Count > 0)
                return Countries.ToList();

            try
            {
                var response = await _httpClient.GetAsync("countries");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<Countries>>(content) ?? new List<Countries>();

                Countries.Clear();
                foreach (var c in list)
                    Countries.Add(c);

                return Countries.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching countries: {ex.Message}");
                return Countries.ToList();
            }
        }
    }
}