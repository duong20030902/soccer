using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private readonly ColorService _colorService;

        public ColorController(ColorService colorService)
        {
            _colorService = colorService;
        }

        // Hiển thị danh sách colors
        public async Task<IActionResult> Index()
        {
            try
            {
                var colors = await _colorService.GetAllColorsAsync();
                return View(colors);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<ColorDto>());
            }
        }

        // Hiển thị form tạo color
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý tạo color
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateColorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _colorService.CreateColorAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo màu sắc thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Không thể tạo màu sắc!";
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
                var color = await _colorService.GetColorByIdAsync(id);
                if (color == null)
                {
                    TempData["Error"] = "Không tìm thấy màu sắc!";
                    return RedirectToAction("Index");
                }

                var updateRequest = new UpdateColorRequest
                {
                    ColorName = color.ColorName,
                    ColorCode = color.ColorCode
                };

                return View(updateRequest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Xử lý cập nhật color
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateColorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var result = await _colorService.UpdateColorAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật màu sắc thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật màu sắc!";
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(request);
            }
        }

        // Xóa color
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _colorService.DeleteColorAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa màu sắc thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa màu sắc!";
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
