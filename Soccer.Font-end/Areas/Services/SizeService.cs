using System.Text.Json;
using System.Text;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Services
{
    public class SizeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public SizeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách tất cả kích thước
        public async Task<List<SizeDto>> GetAllSizesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/sizes");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<SizeDto>>(json, _jsonOptions) ?? new List<SizeDto>();
                }
                return new List<SizeDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách kích thước: {ex.Message}");
            }
        }

        // Lấy chi tiết kích thước theo ID
        public async Task<SizeDto?> GetSizeByIdAsync(int id)
        {
            try
            {
                var sizes = await GetAllSizesAsync();
                return sizes.FirstOrDefault(s => s.SizeID == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy kích thước ID {id}: {ex.Message}");
            }
        }

        // Tạo kích thước mới
        public async Task<SizeDto?> CreateSizeAsync(CreateSizeRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/sizes", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SizeDto>(responseJson, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo kích thước: {ex.Message}");
            }
        }

        // Cập nhật kích thước
        public async Task<bool> UpdateSizeAsync(int id, UpdateSizeRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/sizes/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật kích thước ID {id}: {ex.Message}");
            }
        }

        // Xóa kích thước
        public async Task<bool> DeleteSizeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/sizes/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa kích thước ID {id}: {ex.Message}");
            }
        }

        // Kiểm tra kích thước có tồn tại không
        public async Task<bool> SizeExistsAsync(string sizeName)
        {
            try
            {
                var sizes = await GetAllSizesAsync();
                return sizes.Any(s => s.SizeName.Equals(sizeName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        // Lấy thứ tự tiếp theo
        public async Task<int> GetNextOrderAsync()
        {
            try
            {
                var sizes = await GetAllSizesAsync();
                return sizes.Any() ? sizes.Max(s => s.SizeOrder) + 1 : 1;
            }
            catch
            {
                return 1;
            }
        }
    }
}
