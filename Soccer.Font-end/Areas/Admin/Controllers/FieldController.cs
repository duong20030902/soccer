using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FieldController : Controller
    {
        private readonly FieldService _fieldService;

        public FieldController(FieldService fieldService)
        {
            _fieldService = fieldService;
        }

        // Hiển thị danh sách fields
        public async Task<IActionResult> Index(string search, decimal? minPrice, decimal? maxPrice)
        {
            try
            {
                List<FieldDto> fields;

                if (!string.IsNullOrEmpty(search))
                {
                    fields = await _fieldService.SearchFieldsAsync(search);
                }
                else if (minPrice.HasValue && maxPrice.HasValue)
                {
                    fields = await _fieldService.GetFieldsByPriceRangeAsync(minPrice.Value, maxPrice.Value);
                }
                else
                {
                    fields = await _fieldService.GetAllFieldsAsync();
                }

                ViewBag.Search = search;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;

                return View(fields);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<FieldDto>());
            }
        }

        // Hiển thị chi tiết field
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var field = await _fieldService.GetFieldByIdAsync(id);
                if (field == null)
                {
                    TempData["Error"] = "Không tìm thấy sân bóng!";
                    return RedirectToAction("Index");
                }

                return View(field);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Hiển thị form tạo field
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo field
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFieldRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _fieldService.CreateFieldAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo sân bóng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Không thể tạo sân bóng!";
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var field = await _fieldService.GetFieldByIdAsync(id);
                if (field == null)
                {
                    TempData["Error"] = "Không tìm thấy sân bóng!";
                    return RedirectToAction("Index");
                }

                var updateRequest = new UpdateFieldRequest
                {
                    FieldName = field.FieldName,
                    Location = field.Location,
                    PricePerHour = field.PricePerHour
                };

                ViewBag.FieldId = id;
                ViewBag.CurrentField = field;

                return View(updateRequest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Xử lý cập nhật field
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateFieldRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _fieldService.UpdateFieldAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật sân bóng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật sân bóng!";
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // Xóa field
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _fieldService.DeleteFieldAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa sân bóng thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa sân bóng!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
