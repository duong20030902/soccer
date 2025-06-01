using Soccer.Font_end.ViewModels;
using System.Text;
using System.Text.Json;

namespace Soccer.Font_end.Areas.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://localhost:7237/api/admin";
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Lấy danh sách tất cả sản phẩm
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/products");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ProductDto>>(json, _jsonOptions) ?? new List<ProductDto>();
                }
                return new List<ProductDto>();
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception($"Lỗi khi lấy danh sách sản phẩm: {ex.Message}");
            }
        }

        // Lấy chi tiết sản phẩm theo ID
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/products/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProductDto>(json, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sản phẩm ID {id}: {ex.Message}");
            }
        }

        // Tạo sản phẩm mới
        public async Task<ProductDto?> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/products", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProductDto>(responseJson, _jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo sản phẩm: {ex.Message}");
            }
        }

        // Cập nhật sản phẩm
        public async Task<bool> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/products/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sản phẩm ID {id}: {ex.Message}");
            }
        }

        // Xóa sản phẩm
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sản phẩm ID {id}: {ex.Message}");
            }
        }
    }
}
