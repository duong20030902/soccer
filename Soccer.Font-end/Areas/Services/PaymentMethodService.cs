using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class PaymentMethodService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public PaymentMethodService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api/admin/PaymentMethods";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<PaymentMethodDto>> GetPaymentMethodsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                response.EnsureSuccessStatusCode();
                var paymentMethods = await response.Content.ReadFromJsonAsync<List<PaymentMethodDto>>(_jsonOptions);
                return paymentMethods ?? new List<PaymentMethodDto>();
            }
            catch (HttpRequestException ex)
            {
                // Log the error (e.g., using ILogger)
                throw new Exception("Failed to retrieve payment methods.", ex);
            }
        }

        public async Task<PaymentMethodDto> CreatePaymentMethodAsync(string methodName)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, methodName, _jsonOptions);
                response.EnsureSuccessStatusCode();
                var createdMethod = await response.Content.ReadFromJsonAsync<PaymentMethodDto>(_jsonOptions);
                return createdMethod;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to create payment method.", ex);
            }
        }

        public async Task<bool> UpdatePaymentMethodAsync(int id, string methodName)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", methodName, _jsonOptions);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to update payment method.", ex);
            }
        }

        public async Task<bool> DeletePaymentMethodAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Failed to delete payment method.", ex);
            }
        }
    }
}
