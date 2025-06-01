using Soccer.Font_end.ViewModels;
using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class OrderStatusService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderStatusService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách tất cả OrderStatus
        public async Task<List<OrderStatusDto>> GetAllOrderStatusesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/orderstatus");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var orderStatuses = JsonSerializer.Deserialize<List<OrderStatusDto>>(jsonContent, _jsonOptions);
                    return orderStatuses ?? new List<OrderStatusDto>();
                }

                return new List<OrderStatusDto>();
            }
            catch (Exception)
            {
                return new List<OrderStatusDto>();
            }
        }

        // Tạo OrderStatus mới
        public async Task<bool> CreateOrderStatusAsync(string statusName)
        {
            try
            {
                var json = JsonSerializer.Serialize(statusName, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/admin/orderstatus", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Cập nhật OrderStatus
        public async Task<bool> UpdateOrderStatusAsync(int id, string statusName)
        {
            try
            {
                var json = JsonSerializer.Serialize(statusName, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/admin/orderstatus/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Xóa OrderStatus
        public async Task<bool> DeleteOrderStatusAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/admin/orderstatus/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Lấy OrderStatus theo ID
        public async Task<OrderStatusDto?> GetOrderStatusByIdAsync(int id)
        {
            try
            {
                var allStatuses = await GetAllOrderStatusesAsync();
                return allStatuses.FirstOrDefault(x => x.StatusID == id);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
