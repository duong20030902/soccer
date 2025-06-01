using Soccer.Font_end.Areas.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class BrandService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public BrandService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api/admin/brands";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // GET: Retrieve all brands
        public async Task<List<BrandDto>> GetBrandsAsync()
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl);
            response.EnsureSuccessStatusCode();
            var brands = await response.Content.ReadFromJsonAsync<List<BrandDto>>(_jsonOptions);
            return brands ?? new List<BrandDto>();
        }

        // POST: Create a new brand
        public async Task<BrandDto> CreateBrandAsync(string brandName)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, brandName, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var createdBrand = await response.Content.ReadFromJsonAsync<BrandDto>(_jsonOptions);
            return createdBrand;
        }

        // PUT: Update an existing brand
        public async Task<bool> UpdateBrandAsync(int id, string brandName)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", brandName, _jsonOptions);
            return response.IsSuccessStatusCode;
        }

        // DELETE: Delete a brand
        public async Task<bool> DeleteBrandAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
