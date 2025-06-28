/*using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ProductService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7237/api";
        }

        public async Task<(List<ProductViewModel> Products, int TotalItems, string Error)> GetProductsAsync(
            string sort = null,
            int? colorId = null,
            int? sizeId = null,
            int? categoryId = null,
            int? brandId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string search = null,
            int page = 1,
            int pageSize = 10)
        {
            var url = $"{_apiBaseUrl}/Product?";

            if (!string.IsNullOrEmpty(sort)) url += $"sort={sort}&";
            if (colorId.HasValue) url += $"colorId={colorId.Value}&";
            if (sizeId.HasValue) url += $"sizeId={sizeId.Value}&";
            if (categoryId.HasValue) url += $"categoryId={categoryId.Value}&";
            if (brandId.HasValue) url += $"brandId={brandId.Value}&";
            if (minPrice.HasValue) url += $"minPrice={minPrice.Value}&";
            if (maxPrice.HasValue) url += $"maxPrice={maxPrice.Value}&";
            if (!string.IsNullOrEmpty(search)) url += $"search={Uri.EscapeDataString(search)}&";

            url += $"page={page}&pageSize={pageSize}";
            url = url.TrimEnd('&');

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<PagedProductResponse>(content, options);
                    return (result?.Data ?? new List<ProductViewModel>(), result?.TotalItems ?? 0, null);
                }
                return (new List<ProductViewModel>(), 0, $"Lỗi khi lấy sản phẩm: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<ProductViewModel>(), 0, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(ProductViewModel Product, string Error)> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<ProductViewModel>(content, options);
                    return (result ?? new ProductViewModel(), null);
                }
                return (new ProductViewModel(), $"Lỗi khi lấy sản phẩm: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new ProductViewModel(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<Brand> Brands, string Error)> GetBrandsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/brands");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<List<Brand>>(content, options);
                    return (result ?? new List<Brand>(), null);
                }
                return (new List<Brand>(), $"Lỗi khi lấy thương hiệu: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<Brand>(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<Category> Categories, string Error)> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/categories");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<List<Category>>(content, options);
                    return (result ?? new List<Category>(), null);
                }
                return (new List<Category>(), $"Lỗi khi lấy danh mục: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<Category>(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<ColorViewModel> Colors, string Error)> GetColorsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/colors");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<List<ColorViewModel>>(content, options);
                    return (result ?? new List<ColorViewModel>(), null);
                }
                return (new List<ColorViewModel>(), $"Lỗi khi lấy màu sắc: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<ColorViewModel>(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<SizeViewModel> Sizes, string Error)> GetSizesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/sizes");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<List<SizeViewModel>>(content, options);
                    return (result ?? new List<SizeViewModel>(), null);
                }
                return (new List<SizeViewModel>(), $"Lỗi khi lấy kích thước: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<SizeViewModel>(), $"Lỗi: {ex.Message}");
            }
        }

        private class PagedProductResponse
        {
            public List<ProductViewModel> Data { get; set; } = new List<ProductViewModel>();
            public int TotalItems { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }
    }
}
*/
using Soccer.Font_end.ViewModels;
using System.Text.Json;

namespace Soccer.Font_end.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7237/api";

            // Cấu hình JSON options một lần
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<(List<ProductViewModel> Products, int TotalItems, string Error)> GetProductsAsync(
            string sort = null,
            int? colorId = null,
            int? sizeId = null,
            int? categoryId = null,
            int? brandId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string search = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                // Xây dựng URL với query parameters
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(sort))
                    queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
                if (colorId.HasValue)
                    queryParams.Add($"colorId={colorId.Value}");
                if (sizeId.HasValue)
                    queryParams.Add($"sizeId={sizeId.Value}");
                if (categoryId.HasValue)
                    queryParams.Add($"categoryId={categoryId.Value}");
                if (brandId.HasValue)
                    queryParams.Add($"brandId={brandId.Value}");
                if (minPrice.HasValue)
                    queryParams.Add($"minPrice={minPrice.Value}");
                if (maxPrice.HasValue)
                    queryParams.Add($"maxPrice={maxPrice.Value}");
                if (!string.IsNullOrEmpty(search))
                {
                    // Trim và escape search parameter
                    search = search.Trim();
                    queryParams.Add($"search={Uri.EscapeDataString(search)}");
                }

                queryParams.Add($"page={page}");
                queryParams.Add($"pageSize={pageSize}");

                var queryString = string.Join("&", queryParams);
                var url = $"{_apiBaseUrl}/Product?{queryString}";

                // Log để debug
                Console.WriteLine($"ProductService - Calling API: {url}");
                Console.WriteLine($"Search parameter: '{search}'");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {content}");

                    var result = JsonSerializer.Deserialize<PagedProductResponse>(content, _jsonOptions);

                    Console.WriteLine($"Deserialized - Products count: {result?.Products?.Count ?? 0}, Total: {result?.TotalItems ?? 0}");

                    return (result?.Products ?? new List<ProductViewModel>(), result?.TotalItems ?? 0, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");
                    return (new List<ProductViewModel>(), 0, $"Lỗi khi lấy sản phẩm: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetProductsAsync: {ex.Message}");
                return (new List<ProductViewModel>(), 0, $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(ProductViewModel Product, string Error)> GetProductByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ProductViewModel>(content, _jsonOptions);
                    return (result ?? new ProductViewModel(), null);
                }
                return (new ProductViewModel(), $"Lỗi khi lấy sản phẩm: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new ProductViewModel(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<Brand> Brands, string Error)> GetBrandsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/brands");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<Brand>>(content, _jsonOptions);
                    return (result ?? new List<Brand>(), null);
                }
                return (new List<Brand>(), $"Lỗi khi lấy thương hiệu: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<Brand>(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<Category> Categories, string Error)> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/categories");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<Category>>(content, _jsonOptions);
                    return (result ?? new List<Category>(), null);
                }
                return (new List<Category>(), $"Lỗi khi lấy danh mục: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<Category>(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<ColorViewModel> Colors, string Error)> GetColorsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/colors");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<ColorViewModel>>(content, _jsonOptions);
                    return (result ?? new List<ColorViewModel>(), null);
                }
                return (new List<ColorViewModel>(), $"Lỗi khi lấy màu sắc: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<ColorViewModel>(), $"Lỗi: {ex.Message}");
            }
        }

        public async Task<(List<SizeViewModel> Sizes, string Error)> GetSizesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Product/sizes");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<List<SizeViewModel>>(content, _jsonOptions);
                    return (result ?? new List<SizeViewModel>(), null);
                }
                return (new List<SizeViewModel>(), $"Lỗi khi lấy kích thước: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return (new List<SizeViewModel>(), $"Lỗi: {ex.Message}");
            }
        }

        // Dispose HttpClient properly
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        // **QUAN TRỌNG: Cập nhật PagedProductResponse để khớp với API response**
        private class PagedProductResponse
        {
            public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>(); // Đổi từ "Data" thành "Products"
            public int TotalItems { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }
    }
}


