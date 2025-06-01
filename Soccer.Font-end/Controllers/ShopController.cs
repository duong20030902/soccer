using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductService _productService;

        public ShopController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string sort = null, int? colorId = null, int? sizeId = null, int? categoryId = null, int? brandId = null, decimal? minPrice = null, decimal? maxPrice = null, int page = 1, int pageSize = 12)
        {
            var (products, totalItems, productsError) = await _productService.GetProductsAsync(sort, colorId, sizeId, categoryId, brandId, minPrice, maxPrice, page, pageSize);
            var (brands, brandsError) = await _productService.GetBrandsAsync();
            var (categories, categoriesError) = await _productService.GetCategoriesAsync();
            var (colors, colorsError) = await _productService.GetColorsAsync();
            var (sizes, sizesError) = await _productService.GetSizesAsync();

            if (!string.IsNullOrEmpty(productsError) || !string.IsNullOrEmpty(brandsError) || !string.IsNullOrEmpty(categoriesError) || !string.IsNullOrEmpty(colorsError) || !string.IsNullOrEmpty(sizesError))
            {
                ViewBag.ErrorMessage = "Lỗi khi tải dữ liệu. Vui lòng thử lại sau.";
            }

            var viewModel = new ShopIndexViewModel
            {
                Products = products ?? new List<ProductViewModel>(),
                Brands = brands ?? new List<Brand>(),
                Categories = categories ?? new List<Category>(),
                Colors = colors ?? new List<ColorViewModel>(),
                Sizes = sizes ?? new List<SizeViewModel>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var (product, error) = await _productService.GetProductByIdAsync(id);
            if (product == null || product.ProductID == 0)
            {
                return NotFound("Sản phẩm không tồn tại.");
            }
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.ErrorMessage = error;
            }
            return View(product);
        }
    }
}