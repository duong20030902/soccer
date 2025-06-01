using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly BookingService _bookingService;

        public BookingController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Trang chính với pagination và filter
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            string? status = null,
            string? fieldName = null,
            string? userName = null)
        {
            var result = await _bookingService.GetBookingsAsync(page, pageSize, status, fieldName, userName);

            // Truyền filter parameters
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentFieldName = fieldName;
            ViewBag.CurrentUserName = userName;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentPageSize = pageSize;

            // Danh sách trạng thái
            ViewBag.StatusList = new List<string> { "Pending", "Confirmed", "Cancelled", "Completed" };

            return View(result);
        }

        // Trang thống kê - CHỈ GIỮ LẠI 1 METHOD
        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            var stats = await _bookingService.GetBookingStatisticsAsync();
            return View(stats);
        }

        // Xem chi tiết booking
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy booking.";
                return RedirectToAction("Index");
            }

            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var schedules = await _bookingService.GetAvailableSchedulesAsync();
                if (!schedules.Any())
                {
                    TempData["ErrorMessage"] = "Hiện không có lịch nào khả dụng.";
                    return View(new CreateBookingRequest());
                }

                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                if (!int.TryParse(userId, out int parsedUserId))
                {
                    TempData["ErrorMessage"] = "Không thể xác định người dùng.";
                    return View(new CreateBookingRequest());
                }

                ViewBag.Schedules = schedules;
                return View(new CreateBookingRequest { UserId = parsedUserId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải danh sách lịch: {ex.Message}";
                return View(new CreateBookingRequest());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Schedules = await _bookingService.GetAvailableSchedulesAsync();
                    return View(request);
                }

                var result = await _bookingService.CreateBookingAsync(request);
                if (result)
                {
                    TempData["SuccessMessage"] = "Tạo booking thành công!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo booking.";
                ViewBag.Schedules = await _bookingService.GetAvailableSchedulesAsync();
                return View(request);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tạo booking: {ex.Message}";
                ViewBag.Schedules = await _bookingService.GetAvailableSchedulesAsync();
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookingDto booking)
        {
            if (ModelState.IsValid)
            {
                var result = await _bookingService.UpdateBookingAsync(booking);
                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật booking thành công!";
                    return RedirectToAction("Details", new { id = booking.BookingId });
                }
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật booking.";
            }
            return View(booking);
        }

        [HttpGet]
        public async Task<IActionResult> Calendar()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return View(bookings);
        }

        // Cập nhật trạng thái
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int bookingId, string status, string? returnUrl = null)
        {
            var result = await _bookingService.UpdateBookingStatusAsync(bookingId, status);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }

        // Hủy booking
        [HttpPost]
        public async Task<IActionResult> Cancel(int bookingId, string? returnUrl = null)
        {
            var result = await _bookingService.CancelBookingAsync(bookingId);

            if (result)
            {
                TempData["SuccessMessage"] = "Hủy booking thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy booking.";
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }

        // API endpoints cho AJAX
        [HttpGet]
        public async Task<JsonResult> GetBookingsJson(
            int page = 1,
            int pageSize = 10,
            string? status = null,
            string? fieldName = null,
            string? userName = null)
        {
            var result = await _bookingService.GetBookingsAsync(page, pageSize, status, fieldName, userName);
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> GetBookingJson(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            return Json(booking);
        }

        [HttpGet]
        public async Task<JsonResult> GetStatisticsJson()
        {
            var stats = await _bookingService.GetBookingStatisticsAsync();
            return Json(stats);
        }

        // Bulk operations
        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus(List<int> bookingIds, string status)
        {
            int successCount = 0;

            foreach (var id in bookingIds)
            {
                var result = await _bookingService.UpdateBookingStatusAsync(id, status);
                if (result) successCount++;
            }

            TempData["SuccessMessage"] = $"Đã cập nhật {successCount}/{bookingIds.Count} booking thành công.";
            return RedirectToAction("Index");
        }
    }
}
