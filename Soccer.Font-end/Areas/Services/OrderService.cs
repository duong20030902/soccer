using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy tất cả đơn hàng
        public async Task<List<OrderDto>?> GetAllOrdersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/orders");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<OrderDto>>(jsonString, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                // Log exception here
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return null;
            }
        }

        // Lấy đơn hàng theo ID
        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/admin/orders/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OrderDto>(jsonString, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching order {id}: {ex.Message}");
                return null;
            }
        }

        // Cập nhật trạng thái đơn hàng
        public async Task<bool> UpdateOrderStatusAsync(int orderId, int statusId)
        {
            try
            {
                var json = JsonSerializer.Serialize(statusId, _jsonOptions);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/admin/orders/{orderId}/status", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return false;
            }
        }

        // Xóa đơn hàng
        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/admin/orders/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting order {id}: {ex.Message}");
                return false;
            }
        }
    }
}
