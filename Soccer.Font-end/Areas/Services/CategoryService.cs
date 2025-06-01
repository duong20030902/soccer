using Soccer.Font_end.Areas.ViewModels;
using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy tất cả categories
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<CategoryDto>>(jsonContent, _jsonOptions) ?? new List<CategoryDto>();
                }

                return new List<CategoryDto>();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception($"Lỗi khi lấy danh sách categories: {ex.Message}");
            }
        }

        // Lấy category theo ID
        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CategoryDto>(jsonContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy category ID {id}: {ex.Message}");
            }
        }

        // Tạo category mới
        public async Task<CategoryDto?> CreateCategoryAsync(CreateCategoryRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/categories", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<CategoryDto>(responseContent, _jsonOptions);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo category: {ex.Message}");
            }
        }

        // Cập nhật category
        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/categories/{id}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật category ID {id}: {ex.Message}");
            }
        }

        // Xóa category
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/categories/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa category ID {id}: {ex.Message}");
            }
        }

        // Lấy categories cha (không có ParentID)
        public async Task<List<CategoryDto>> GetParentCategoriesAsync()
        {
            try
            {
                var allCategories = await GetAllCategoriesAsync();
                return allCategories.Where(c => c.ParentID == null).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách categories cha: {ex.Message}");
            }
        }

        // Lấy categories con theo ParentID
        public async Task<List<CategoryDto>> GetChildCategoriesAsync(int parentId)
        {
            try
            {
                var allCategories = await GetAllCategoriesAsync();
                return allCategories.Where(c => c.ParentID == parentId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy categories con của ID {parentId}: {ex.Message}");
            }
        }
    }
}
