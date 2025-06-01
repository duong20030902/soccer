using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Services;

namespace Soccer.Font_end.Controllers
{
    public class BookController : Controller
    {
        private readonly BookingService _bookingService;

        public BookController(BookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: /Book
        public async Task<IActionResult> Index(int page = 1, string? status = null)
        {
            try
            {
                var result = await _bookingService.GetMyBookingsAsync(page, 10, status);

                if (result.Success && result.Data != null)
                {
                    ViewBag.CurrentPage = page;
                    ViewBag.CurrentStatus = status;
                    return View(result.Data);
                }

                ViewBag.ErrorMessage = result.Message;
                return View(new PagedResult<BookingResponseDto>());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải trang";
                return View(new PagedResult<BookingResponseDto>());
            }
        }

        // GET: /Book/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var result = await _bookingService.GetBookingAsync(id);

                if (result.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                ViewBag.ErrorMessage = result.Message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải chi tiết booking";
                return View();
            }
        }

        // GET: /Book/Create
        [Route("Create")]
        public IActionResult Create(int? scheduleId)
        {
            var model = new CreateBookingRequest();
            if (scheduleId.HasValue)
            {
                model.ScheduleId = scheduleId.Value;
            }
            return View(model);
        }

        // POST: /Book/Create
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await _bookingService.CreateBookingAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = result.Data?.BookingId });
                }

                ModelState.AddModelError("", result.Message ?? "Có lỗi xảy ra");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        // POST: /Book/Cancel/5
        [HttpPost]
        [Route("Cancel/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy booking";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: /Book/UpdateStatus/5
        [HttpPost]
        [Route("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var result = await _bookingService.UpdateBookingStatusAsync(id, status);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message;
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // API endpoint for AJAX calls
        [HttpPost]
        [Route("api/cancel/{id}")]
        public async Task<JsonResult> CancelBookingApi(int id)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Có lỗi xảy ra"
                });
            }
        }
    }
}
