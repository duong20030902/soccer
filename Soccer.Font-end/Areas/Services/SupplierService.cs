using System.Text.Json;
using System.Text;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Services
{
    public class SupplierService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public SupplierService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách tất cả nhà cung cấp
        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/suppliers");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<SupplierDto>>(json, _jsonOptions) ?? new List<SupplierDto>();
                }
                return new List<SupplierDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách nhà cung cấp: {ex.Message}");
            }
        }

        // Lấy chi tiết nhà cung cấp theo ID
        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            try
            {
                var suppliers = await GetAllSuppliersAsync();
                return suppliers.FirstOrDefault(s => s.SupplierID == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy nhà cung cấp ID {id}: {ex.Message}");
            }
        }

        // Tạo nhà cung cấp mới
        public async Task<SupplierDto?> CreateSupplierAsync(CreateSupplierRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/suppliers", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SupplierDto>(responseJson, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo nhà cung cấp: {ex.Message}");
            }
        }

        // Cập nhật nhà cung cấp
        public async Task<bool> UpdateSupplierAsync(int id, UpdateSupplierRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/suppliers/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật nhà cung cấp ID {id}: {ex.Message}");
            }
        }

        // Xóa nhà cung cấp
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/suppliers/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhà cung cấp ID {id}: {ex.Message}");
            }
        }

        // Kiểm tra nhà cung cấp có tồn tại không
        public async Task<bool> SupplierExistsAsync(string supplierName)
        {
            try
            {
                var suppliers = await GetAllSuppliersAsync();
                return suppliers.Any(s => s.SupplierName.Equals(supplierName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        // Tìm kiếm nhà cung cấp
        public async Task<List<SupplierDto>> SearchSuppliersAsync(string searchTerm)
        {
            try
            {
                var suppliers = await GetAllSuppliersAsync();
                if (string.IsNullOrEmpty(searchTerm))
                    return suppliers;

                return suppliers.Where(s =>
                    s.SupplierName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(s.ContactInfo) && s.ContactInfo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            catch
            {
                return new List<SupplierDto>();
            }
        }
    }
}
