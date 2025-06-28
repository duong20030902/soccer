/*using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index(
            string sort = null,
            int? colorId = null,
            int? sizeId = null,
            int? categoryId = null,
            int? brandId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string search = null,
            int page = 1,
            int pageSize = 12)
        {
            var validPageSizes = new List<int> { 6, 12, 24, 48 };
            if (!validPageSizes.Contains(pageSize))
            {
                pageSize = 12; // Giá trị mặc định
            }
            var (products, totalItems, productsError) = await _productService.GetProductsAsync(
                sort, colorId, sizeId, categoryId, brandId, minPrice, maxPrice, search, page, pageSize);

            var (brands, brandsError) = await _productService.GetBrandsAsync();
            var (categories, categoriesError) = await _productService.GetCategoriesAsync();
            var (colors, colorsError) = await _productService.GetColorsAsync();
            var (sizes, sizesError) = await _productService.GetSizesAsync();

            if (!string.IsNullOrEmpty(productsError) || !string.IsNullOrEmpty(brandsError) ||
                !string.IsNullOrEmpty(categoriesError) || !string.IsNullOrEmpty(colorsError) ||
                !string.IsNullOrEmpty(sizesError))
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
                TotalItems = totalItems,
                PageSizeOptions = validPageSizes,
                // Thêm các thuộc tính để giữ trạng thái filter
                CurrentSort = sort,
                CurrentColorId = colorId,
                CurrentSizeId = sizeId,
                CurrentCategoryId = categoryId,
                CurrentBrandId = brandId,
                CurrentMinPrice = minPrice,
                CurrentMaxPrice = maxPrice,
                CurrentSearch = search
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
*/

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

        public async Task<IActionResult> Index(
            string sort = null,
            int? colorId = null,
            int? sizeId = null,
            int? categoryId = null,
            int? brandId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string search = null,
            int page = 1,
            int pageSize = 12)
        {
            // Xử lý và làm sạch tham số search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                // Decode URL nếu cần thiết
                search = Uri.UnescapeDataString(search);

                // Log để debug
                Console.WriteLine($"Search parameter received: '{search}'");

                // Nếu search rỗng sau khi trim thì set về null
                if (string.IsNullOrWhiteSpace(search))
                {
                    search = null;
                }
            }

            var validPageSizes = new List<int> { 6, 12, 24, 48 };
            if (!validPageSizes.Contains(pageSize))
            {
                pageSize = 12;
            }

            // Log tất cả parameters để debug
            Console.WriteLine($"Filter parameters - Sort: {sort}, ColorId: {colorId}, SizeId: {sizeId}, " +
                            $"CategoryId: {categoryId}, BrandId: {brandId}, MinPrice: {minPrice}, " +
                            $"MaxPrice: {maxPrice}, Search: '{search}', Page: {page}, PageSize: {pageSize}");

            var (products, totalItems, productsError) = await _productService.GetProductsAsync(
                sort, colorId, sizeId, categoryId, brandId, minPrice, maxPrice, search, page, pageSize);

            // Log kết quả
            Console.WriteLine($"Products found: {products?.Count ?? 0}, Total items: {totalItems}");

            var (brands, brandsError) = await _productService.GetBrandsAsync();
            var (categories, categoriesError) = await _productService.GetCategoriesAsync();
            var (colors, colorsError) = await _productService.GetColorsAsync();
            var (sizes, sizesError) = await _productService.GetSizesAsync();

            if (!string.IsNullOrEmpty(productsError) || !string.IsNullOrEmpty(brandsError) ||
                !string.IsNullOrEmpty(categoriesError) || !string.IsNullOrEmpty(colorsError) ||
                !string.IsNullOrEmpty(sizesError))
            {
                ViewBag.ErrorMessage = "Lỗi khi tải dữ liệu. Vui lòng thử lại sau.";
                Console.WriteLine($"Errors - Products: {productsError}, Brands: {brandsError}, " +
                                $"Categories: {categoriesError}, Colors: {colorsError}, Sizes: {sizesError}");
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
                TotalItems = totalItems,
                PageSizeOptions = validPageSizes,
                CurrentSort = sort,
                CurrentColorId = colorId,
                CurrentSizeId = sizeId,
                CurrentCategoryId = categoryId,
                CurrentBrandId = brandId,
                CurrentMinPrice = minPrice,
                CurrentMaxPrice = maxPrice,
                CurrentSearch = search
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


