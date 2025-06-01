using System.Text.Json;
using System.Text;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Services
{
    public class FieldService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public FieldService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy tất cả fields
        public async Task<List<FieldDto>> GetAllFieldsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/fields");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<FieldDto>>(jsonContent, _jsonOptions) ?? new List<FieldDto>();
                }

                return new List<FieldDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sân bóng: {ex.Message}");
            }
        }

        // Lấy field theo ID
        public async Task<FieldDto?> GetFieldByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/fields/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FieldDto>(jsonContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sân bóng ID {id}: {ex.Message}");
            }
        }

        // Tạo field mới
        public async Task<FieldDto?> CreateFieldAsync(CreateFieldRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/fields", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<FieldDto>(responseContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo sân bóng: {ex.Message}");
            }
        }

        // Cập nhật field
        public async Task<bool> UpdateFieldAsync(int id, UpdateFieldRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/fields/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sân bóng ID {id}: {ex.Message}");
            }
        }

        // Xóa field
        public async Task<bool> DeleteFieldAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/fields/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sân bóng ID {id}: {ex.Message}");
            }
        }

        // Tìm kiếm fields theo tên hoặc địa điểm
        public async Task<List<FieldDto>> SearchFieldsAsync(string keyword)
        {
            try
            {
                var allFields = await GetAllFieldsAsync();
                if (string.IsNullOrEmpty(keyword))
                    return allFields;

                return allFields.Where(f =>
                    f.FieldName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    f.Location.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tìm kiếm sân bóng: {ex.Message}");
            }
        }

        // Lấy fields theo khoảng giá
        public async Task<List<FieldDto>> GetFieldsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                var allFields = await GetAllFieldsAsync();
                return allFields.Where(f => f.PricePerHour >= minPrice && f.PricePerHour <= maxPrice).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lọc sân bóng theo giá: {ex.Message}");
            }
        }
    }
}
