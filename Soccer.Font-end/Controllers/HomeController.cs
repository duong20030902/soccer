using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Controllers
{
    public class HomeController : Controller
    {
        private readonly FieldService _fieldService;
        private readonly ProductService _productService; 

        public HomeController(FieldService fieldService, ProductService productService)
        {
            _fieldService = fieldService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy dữ liệu song song để tối ưu performance
                var productsTask = _productService.GetProductsAsync();
                var fieldsTask = _fieldService.GetSampleFieldsForHomeAsync();

                await Task.WhenAll(productsTask, fieldsTask);

                // Lấy kết quả products
                var (products, totalItems, productError) = await productsTask;
                List<ProductViewModel> randomProducts = new List<ProductViewModel>();

                if (products != null && products.Any())
                {
                    randomProducts = products.OrderBy(x => Guid.NewGuid()).Take(4).ToList();
                }

                // Lấy kết quả fields
                var (fields, fieldError) = await fieldsTask;
                List<FieldSearchResultViewModel> featuredFields = new List<FieldSearchResultViewModel>();

                if (fields != null && fields.Any())
                {
                    // Lấy 6 sân đầu tiên làm sân nổi bật
                    featuredFields = fields.Take(6).ToList();
                }

                // Tạo HomeViewModel để chứa cả products và fields
                var homeViewModel = new HomeViewModel
                {
                    Products = randomProducts,
                    FeaturedFields = featuredFields
                };

                // Set error messages nếu có
                if (!string.IsNullOrEmpty(productError))
                {
                    ViewBag.ProductError = productError;
                }

                if (!string.IsNullOrEmpty(fieldError))
                {
                    ViewBag.FieldError = fieldError;
                }

                return View(homeViewModel);
            }
            catch (Exception ex)
            {
                // Log exception nếu cần
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải dữ liệu trang chủ";

                return View(new HomeViewModel
                {
                    Products = new List<ProductViewModel>(),
                    FeaturedFields = new List<FieldSearchResultViewModel>()
                });
            }
        }

        // Action để tìm kiếm sân
        [HttpGet]
        public async Task<IActionResult> SearchFields(DateOnly? date, int? timeslotId, string? fieldName)
        {
            try
            {
                // Lấy danh sách timeslots để hiển thị trong dropdown
                var (timeslots, timeslotError) = await _fieldService.GetTimeslotsAsync();
                ViewBag.Timeslots = timeslots ?? new List<TimeslotViewModel>();

                // Nếu không có tham số tìm kiếm, chỉ hiển thị form
                if (!date.HasValue || !timeslotId.HasValue)
                {
                    var searchRequest = new FieldSearchRequest
                    {
                        Date = date ?? DateOnly.FromDateTime(DateTime.Now),
                        TimeslotId = timeslotId ?? (timeslots?.FirstOrDefault()?.TimeslotID ?? 1),
                        FieldName = fieldName
                    };

                    return View("SearchFields", (new List<FieldSearchResultViewModel>(), searchRequest));
                }

                // Thực hiện tìm kiếm
                var (fields, error) = string.IsNullOrEmpty(fieldName)
                    ? await _fieldService.GetAvailableFieldsAsync(date.Value, timeslotId.Value)
                    : await _fieldService.SearchFieldsByNameAsync(fieldName, date.Value, timeslotId.Value);

                var request = new FieldSearchRequest
                {
                    Date = date.Value,
                    TimeslotId = timeslotId.Value,
                    FieldName = fieldName
                };

                if (!string.IsNullOrEmpty(error))
                {
                    ViewBag.ErrorMessage = error;
                }

                return View("SearchFields", (fields ?? new List<FieldSearchResultViewModel>(), request));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tìm kiếm sân";
                return View("SearchFields", (new List<FieldSearchResultViewModel>(), new FieldSearchRequest()));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}