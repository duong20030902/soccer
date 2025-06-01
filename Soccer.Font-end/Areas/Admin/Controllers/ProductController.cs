using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<ProductDto>());
            }
        }

        // GET: Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Hiển thị form tạo sản phẩm
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý tạo sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _productService.CreateProductAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể tạo sản phẩm";
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // GET: Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var updateRequest = new UpdateProductRequest
                {
                    CategoryID = product.CategoryID,
                    BrandID = product.BrandID,
                    SupplierID = product.SupplierID,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    CostPrice = product.CostPrice,
                    SalePrice = product.SalePrice,
                    StockQuantity = product.StockQuantity
                };

                return View(updateRequest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Xử lý cập nhật sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _productService.UpdateProductAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật sản phẩm";
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // POST: Xóa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa sản phẩm thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa sản phẩm";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
