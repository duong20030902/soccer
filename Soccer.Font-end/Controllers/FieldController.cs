using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Controllers
{
    public class FieldController : Controller
    {
        private readonly FieldService _fieldService;

        public FieldController(FieldService fieldService)
        {
            _fieldService = fieldService;
        }

        // GET: /Field/Booking
        public async Task<IActionResult> Booking(DateOnly? date = null, int? timeslotId = null, string? fieldName = null)
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
        }

        // API endpoint cho AJAX search
        [HttpGet]
        public async Task<JsonResult> SearchFields(DateOnly date, int timeslotId, string? fieldName = null)
        {
            try
            {
                List<FieldSearchResultViewModel>? fields;
                string? error;

                if (!string.IsNullOrEmpty(fieldName))
                {
                    (fields, error) = await _fieldService.SearchFieldsByNameAsync(fieldName, date, timeslotId);
                }
                else
                {
                    (fields, error) = await _fieldService.GetAvailableFieldsAsync(date, timeslotId);
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
    }
}
