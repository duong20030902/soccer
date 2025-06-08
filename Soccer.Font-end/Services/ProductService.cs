using Soccer.Font_end.ViewModels;
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
