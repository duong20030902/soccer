using System.Text.Json;
using System.Text;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Services
{
    public class RoleService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public RoleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách tất cả vai trò
        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/roles");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<RoleDto>>(json, _jsonOptions) ?? new List<RoleDto>();
                }
                return new List<RoleDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách vai trò: {ex.Message}");
            }
        }

        // Lấy chi tiết vai trò theo ID
        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/roles/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<RoleDto>(json, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy vai trò ID {id}: {ex.Message}");
            }
        }

        // Tạo vai trò mới
        public async Task<RoleDto?> CreateRoleAsync(string roleName)
        {
            try
            {
                var json = JsonSerializer.Serialize(roleName, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/roles", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<RoleDto>(responseJson, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo vai trò: {ex.Message}");
            }
        }

        // Cập nhật vai trò
        public async Task<bool> UpdateRoleAsync(int id, string roleName)
        {
            try
            {
                var json = JsonSerializer.Serialize(roleName, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/roles/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật vai trò ID {id}: {ex.Message}");
            }
        }

        // Xóa vai trò
        public async Task<bool> DeleteRoleAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/roles/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa vai trò ID {id}: {ex.Message}");
            }
        }

        // Kiểm tra vai trò có tồn tại không
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            try
            {
                var roles = await GetAllRolesAsync();
                return roles.Any(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }
    }
}
