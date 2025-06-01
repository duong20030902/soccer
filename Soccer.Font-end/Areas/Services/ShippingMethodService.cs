using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class ShippingMethodService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ShippingMethodService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiBaseUrl = "https://localhost:7237/api/admin/ShippingMethods";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<ShippingMethodDto>> GetShippingMethodsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                response.EnsureSuccessStatusCode();
                var shippingMethods = await response.Content.ReadFromJsonAsync<List<ShippingMethodDto>>(_jsonOptions);
                return shippingMethods ?? new List<ShippingMethodDto>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to retrieve shipping methods.", ex);
            }
        }

        public async Task<ShippingMethodDto> CreateShippingMethodAsync(CreateShippingMethodRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, request, _jsonOptions);
                response.EnsureSuccessStatusCode();
                var createdMethod = await response.Content.ReadFromJsonAsync<ShippingMethodDto>(_jsonOptions);
                return createdMethod;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to create shipping method.", ex);
            }
        }

        public async Task<bool> UpdateShippingMethodAsync(int id, UpdateShippingMethodRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", request, _jsonOptions);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to update shipping method.", ex);
            }
        }

        public async Task<bool> DeleteShippingMethodAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete shipping method.", ex);
            }
        }
    }
}
