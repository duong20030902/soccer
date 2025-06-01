using Soccer.Font_end.Areas.ViewModels;
using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class ColorService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ColorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy tất cả colors
        public async Task<List<ColorDto>> GetAllColorsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/colors");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ColorDto>>(jsonContent, _jsonOptions) ?? new List<ColorDto>();
                }

                return new List<ColorDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách màu sắc: {ex.Message}");
            }
        }

        // Lấy color theo ID
        public async Task<ColorDto?> GetColorByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/colors/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ColorDto>(jsonContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy màu sắc ID {id}: {ex.Message}");
            }
        }

        // Tạo color mới
        public async Task<ColorDto?> CreateColorAsync(CreateColorRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/colors", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ColorDto>(responseContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo màu sắc: {ex.Message}");
            }
        }

        // Cập nhật color
        public async Task<bool> UpdateColorAsync(int id, UpdateColorRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/colors/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật màu sắc ID {id}: {ex.Message}");
            }
        }

        // Xóa color
        public async Task<bool> DeleteColorAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/colors/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa màu sắc ID {id}: {ex.Message}");
            }
        }
    }
}
