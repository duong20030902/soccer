using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizeController : Controller
    {
        private readonly SizeService _sizeService;

        public SizeController(SizeService sizeService)
        {
            _sizeService = sizeService;
        }

        // GET: Hiển thị danh sách kích thước
        public async Task<IActionResult> Index()
        {
            try
            {
                var sizes = await _sizeService.GetAllSizesAsync();
                return View(sizes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<SizeDto>());
            }
        }

        // GET: Hiển thị chi tiết kích thước
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var size = await _sizeService.GetSizeByIdAsync(id);
                if (size == null)
                {
                    return NotFound();
                }
                return View(size);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Hiển thị form tạo kích thước
        public async Task<IActionResult> Create()
        {
            var model = new CreateSizeViewModel
            {
                SizeOrder = await _sizeService.GetNextOrderAsync()
            };
            return View(model);
        }

        // POST: Xử lý tạo kích thước
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSizeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra kích thước đã tồn tại
                var sizeExists = await _sizeService.SizeExistsAsync(model.SizeName);
                if (sizeExists)
                {
                    ModelState.AddModelError("SizeName", "Kích thước này đã tồn tại");
                    return View(model);
                }

                var request = new CreateSizeRequest
                {
                    SizeName = model.SizeName,
                    SizeOrder = model.SizeOrder
                };

                var result = await _sizeService.CreateSizeAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo kích thước thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể tạo kích thước";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        // GET: Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var size = await _sizeService.GetSizeByIdAsync(id);
                if (size == null)
                {
                    return NotFound();
                }

                var editModel = new EditSizeViewModel
                {
                    SizeID = size.SizeID,
                    SizeName = size.SizeName,
                    SizeOrder = size.SizeOrder
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Xử lý cập nhật kích thước
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditSizeViewModel model)
        {
            if (id != model.SizeID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra tên kích thước mới có trùng không (trừ chính nó)
                var existingSizes = await _sizeService.GetAllSizesAsync();
                var duplicateSize = existingSizes.FirstOrDefault(s =>
                    s.SizeName.Equals(model.SizeName, StringComparison.OrdinalIgnoreCase) &&
                    s.SizeID != model.SizeID);

                if (duplicateSize != null)
                {
                    ModelState.AddModelError("SizeName", "Tên kích thước này đã tồn tại");
                    return View(model);
                }

                var request = new UpdateSizeRequest
                {
                    SizeName = model.SizeName,
                    SizeOrder = model.SizeOrder
                };

                var result = await _sizeService.UpdateSizeAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật kích thước thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật kích thước";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        // POST: Xóa kích thước
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _sizeService.DeleteSizeAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa kích thước thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa kích thước";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Sắp xếp lại thứ tự
        [HttpPost]
        public async Task<IActionResult> ReorderSizes([FromBody] List<ReorderSizeModel> sizes)
        {
            try
            {
                foreach (var item in sizes)
                {
                    var size = await _sizeService.GetSizeByIdAsync(item.SizeID);
                    if (size != null)
                    {
                        var request = new UpdateSizeRequest
                        {
                            SizeName = size.SizeName,
                            SizeOrder = item.NewOrder
                        };
                        await _sizeService.UpdateSizeAsync(item.SizeID, request);
                    }
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class ReorderSizeModel
    {
        public int SizeID { get; set; }
        public int NewOrder { get; set; }
    }
}
