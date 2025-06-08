using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Controllers
{
    public class BookController : Controller
    {
        private readonly BookingService _bookingService;
        private readonly FieldService _fieldService;

        public BookController(BookingService bookingService, FieldService fieldService)
        {
            _bookingService = bookingService;
            _fieldService = fieldService;
        }

        /*     // GET: /Book - Trang danh sách sân để đặt
             public async Task<IActionResult> Index(DateOnly? date = null, int? timeslotId = null, string? fieldName = null)
             {
                 try
                 {
                     // Nếu không có ngày, sử dụng ngày hôm nay
                     var searchDate = date ?? DateOnly.FromDateTime(DateTime.Now);

                     // Lấy danh sách khung giờ
                     var (timeslots, timeslotError) = await _fieldService.GetTimeslotsAsync();
                     if (timeslots == null)
                     {
                         ViewBag.ErrorMessage = timeslotError ?? "Không thể lấy danh sách khung giờ";
                         return View(new FieldBookingViewModel());
                     }

                     List<FieldSearchResultViewModel>? fields = null;
                     string? error = null;

                     // Nếu có timeslotId, tìm kiếm sân
                     if (timeslotId.HasValue)
                     {
                         if (!string.IsNullOrEmpty(fieldName))
                         {
                             // Tìm kiếm theo tên sân
                             (fields, error) = await _fieldService.SearchFieldsByNameAsync(fieldName, searchDate, timeslotId.Value);
                         }
                         else
                         {
                             // Lấy tất cả sân có sẵn
                             (fields, error) = await _fieldService.GetAvailableFieldsAsync(searchDate, timeslotId.Value);
                         }
                     }
                     else if (timeslots.Any())
                     {
                         // Nếu không có timeslotId, sử dụng khung giờ đầu tiên
                         var firstTimeslot = timeslots.First();
                         (fields, error) = await _fieldService.GetAvailableFieldsAsync(searchDate, firstTimeslot.TimeslotID);
                     }

                     var viewModel = new FieldBookingViewModel
                     {
                         Fields = fields ?? new List<FieldSearchResultViewModel>(),
                         Timeslots = timeslots,
                         SelectedDate = searchDate,
                         SelectedTimeslotId = timeslotId,
                         SearchFieldName = fieldName,
                         ErrorMessage = error
                     };

                     return View(viewModel);
                 }
                 catch (Exception ex)
                 {
                     ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải trang";
                     return View(new FieldBookingViewModel());
                 }
             }*/

        /*        public async Task<IActionResult> Index(DateOnly? date = null, int? timeslotId = null, string? fieldName = null)
                {
                    try
                    {
                        var searchDate = date ?? DateOnly.FromDateTime(DateTime.Now);

                        // Lấy danh sách khung giờ
                        var (timeslots, timeslotError) = await _fieldService.GetTimeslotsAsync();
                        if (timeslots == null)
                        {
                            ViewBag.ErrorMessage = timeslotError ?? "Không thể lấy danh sách khung giờ";
                            return View(new FieldBookingViewModel());
                        }

                        List<FieldSearchResultViewModel>? fields = null;
                        string? error = null;

                        // Nếu có timeslotId cụ thể, tìm kiếm theo khung giờ đó
                        if (timeslotId.HasValue)
                        {
                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                (fields, error) = await _fieldService.SearchFieldsByNameAsync(fieldName, searchDate, timeslotId.Value);
                            }
                            else
                            {
                                (fields, error) = await _fieldService.GetAvailableFieldsAsync(searchDate, timeslotId.Value);
                            }
                        }
                        else
                        {
                            // Nếu không có timeslotId, lấy tất cả sân cho tất cả khung giờ
                            if (!string.IsNullOrEmpty(fieldName))
                            {
                                // Tìm kiếm theo tên trong tất cả khung giờ
                                var allFields = new List<FieldSearchResultViewModel>();
                                foreach (var timeslot in timeslots)
                                {
                                    var (searchFields, searchError) = await _fieldService.SearchFieldsByNameAsync(fieldName, searchDate, timeslot.TimeslotID);
                                    if (searchFields != null)
                                    {
                                        allFields.AddRange(searchFields);
                                    }
                                }
                                fields = allFields.GroupBy(f => new { f.FieldId, f.ScheduleId }).Select(g => g.First()).ToList();
                            }
                            else
                            {
                                // Lấy tất cả sân cho tất cả khung giờ
                                (fields, error) = await _fieldService.GetAllAvailableFieldsAsync(searchDate);
                            }
                        }

                        var viewModel = new FieldBookingViewModel
                        {
                            Fields = fields ?? new List<FieldSearchResultViewModel>(),
                            Timeslots = timeslots,
                            SelectedDate = searchDate,
                            SelectedTimeslotId = timeslotId,
                            SearchFieldName = fieldName,
                            ErrorMessage = error
                        };

                        return View(viewModel);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải trang";
                        return View(new FieldBookingViewModel());
                    }
                }*/

        public async Task<IActionResult> Index(DateOnly? date = null, int? timeslotId = null, string? fieldName = null)
        {
            try
            {
                var searchDate = date ?? DateOnly.FromDateTime(DateTime.Now);

                // Lấy danh sách khung giờ
                var (timeslots, timeslotError) = await _fieldService.GetTimeslotsAsync();
                if (timeslots == null)
                {
                    ViewBag.ErrorMessage = timeslotError ?? "Không thể lấy danh sách khung giờ";
                    return View(new FieldBookingViewModel());
                }

                List<FieldSearchResultViewModel>? fields = null;
                string? error = null;

                // Nếu có timeslotId cụ thể
                if (timeslotId.HasValue)
                {
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        (fields, error) = await _fieldService.SearchFieldsByNameAsync(fieldName, searchDate, timeslotId.Value);
                    }
                    else
                    {
                        (fields, error) = await _fieldService.GetAvailableFieldsAsync(searchDate, timeslotId.Value);
                    }
                }
                else
                {
                    // Search trong tất cả khung giờ
                    (fields, error) = await _fieldService.GetAllAvailableFieldsAsync(searchDate, fieldName);
                }

                var viewModel = new FieldBookingViewModel
                {
                    Fields = fields ?? new List<FieldSearchResultViewModel>(),
                    Timeslots = timeslots,
                    SelectedDate = searchDate,
                    SelectedTimeslotId = timeslotId,
                    SearchFieldName = fieldName,
                    ErrorMessage = error
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải trang";
                return View(new FieldBookingViewModel());
            }
        }


        // GET: /Book/MyBookings - Trang quản lý booking của user
        public async Task<IActionResult> MyBookings(int page = 1, string? status = null)
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

        // Cập nhật API endpoint cho AJAX search
        [HttpGet]
        public async Task<JsonResult> SearchFields(DateOnly date, int? timeslotId = null, string? fieldName = null)
        {
            try
            {
                List<FieldSearchResultViewModel>? fields;
                string? error;

                if (timeslotId.HasValue)
                {
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        (fields, error) = await _fieldService.SearchFieldsByNameAsync(fieldName, date, timeslotId.Value);
                    }
                    else
                    {
                        (fields, error) = await _fieldService.GetAvailableFieldsAsync(date, timeslotId.Value);
                    }
                }
                else
                {
                    (fields, error) = await _fieldService.GetAllAvailableFieldsAsync(date, fieldName);
                }

                if (error != null)
                {
                    return Json(new { success = false, message = error });
                }

                return Json(new { success = true, data = fields });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi tìm kiếm" });
            }
        }

        // GET: /Book/Details/5
        [Route("BookingDetails/{id}")]
        public async Task<IActionResult> BookingDetails(int id)
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

        [HttpGet]
        public async Task<IActionResult> FieldDetails(int id)
        {
            try
            {
                var result = await _fieldService.GetFieldDetailsAsync(id);

                if (result.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                ViewBag.ErrorMessage = result.Message ?? "Không tìm thấy thông tin sân";
                return NotFound();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải chi tiết sân";
                return View();
            }
        }

        // GET: /Book/Create
        [Route("Create")]
        public async Task<IActionResult> Create(int? fieldId, DateOnly? date, int? timeslotId)
        {
            try
            {
                var model = new CreateBookingRequest();

                if (fieldId.HasValue)
                {
                    // Lấy thông tin sân để hiển thị
                    var fieldResult = await _fieldService.GetFieldDetailsAsync(fieldId.Value);
                    if (fieldResult.Success && fieldResult.Data != null)
                    {
                        ViewBag.FieldInfo = fieldResult.Data;
                    }
                }

                if (date.HasValue)
                {
                    model.BookingDate = date.Value;
                }

                if (timeslotId.HasValue)
                {
                    model.TimeslotId = timeslotId.Value;
                }

                // Lấy danh sách timeslots cho dropdown
                var (timeslots, _) = await _fieldService.GetTimeslotsAsync();
                ViewBag.Timeslots = timeslots ?? new List<TimeslotViewModel>();

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải trang đặt sân";
                return View(new CreateBookingRequest());
            }
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
                    // Reload dữ liệu cần thiết cho view
                    var (timeslots, _) = await _fieldService.GetTimeslotsAsync();
                    ViewBag.Timeslots = timeslots ?? new List<TimeslotViewModel>();

                    if (model.FieldId.HasValue)
                    {
                        var fieldResult = await _fieldService.GetFieldDetailsAsync(model.FieldId.Value);
                        if (fieldResult.Success && fieldResult.Data != null)
                        {
                            ViewBag.FieldInfo = fieldResult.Data;
                        }
                    }

                    return View(model);
                }

                var result = await _bookingService.CreateBookingAsync(model);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction(nameof(BookingDetails), new { id = result.Data?.BookingId });
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

                return RedirectToAction(nameof(BookingDetails), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi hủy booking";
                return RedirectToAction(nameof(BookingDetails), new { id });
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

                return RedirectToAction(nameof(BookingDetails), new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái";
                return RedirectToAction(nameof(BookingDetails), new { id });
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
