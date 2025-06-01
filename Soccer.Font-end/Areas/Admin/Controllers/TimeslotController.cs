using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TimeslotController : Controller
    {
        private readonly TimeslotService _timeslotService;

        public TimeslotController(TimeslotService timeslotService)
        {
            _timeslotService = timeslotService;
        }

        // GET: Admin/Timeslot
        public async Task<IActionResult> Index()
        {
            try
            {
                var timeslots = await _timeslotService.GetAllTimeslotsAsync();
                return View(timeslots);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải danh sách timeslot: " + ex.Message;
                return View(new List<TimeslotDto>());
            }
        }

        [HttpGet]
        // GET: Admin/Timeslot/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var timeslot = await _timeslotService.GetTimeslotByIdAsync(id);
                if (timeslot == null)
                {
                    return NotFound();
                }
                return View(timeslot);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải thông tin timeslot: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/Timeslot/Create
        public IActionResult Create()
        {
            return View(new CreateTimeslotRequest());
        }

        // POST: Admin/Timeslot/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTimeslotRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            // Validation logic
            if (request.StartTime >= request.EndTime)
            {
                ModelState.AddModelError("", "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                return View(request);
            }

            try
            {
                var result = await _timeslotService.CreateTimeslotAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo timeslot thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể tạo timeslot. Có thể bị trùng thời gian.";
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tạo timeslot: " + ex.Message;
                return View(request);
            }
        }

        // GET: Admin/Timeslot/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var timeslot = await _timeslotService.GetTimeslotByIdAsync(id);
                if (timeslot == null)
                {
                    return NotFound();
                }

                var updateRequest = new UpdateTimeslotRequest
                {
                    StartTime = timeslot.StartTime,
                    EndTime = timeslot.EndTime
                };

                ViewBag.TimeslotId = id;
                return View(updateRequest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải thông tin timeslot: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Timeslot/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTimeslotRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TimeslotId = id;
                return View(request);
            }

            // Validation logic
            if (request.StartTime >= request.EndTime)
            {
                ModelState.AddModelError("", "Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc");
                ViewBag.TimeslotId = id;
                return View(request);
            }

            try
            {
                var result = await _timeslotService.UpdateTimeslotAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật timeslot thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật timeslot.";
                    ViewBag.TimeslotId = id;
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật timeslot: " + ex.Message;
                ViewBag.TimeslotId = id;
                return View(request);
            }
        }

        // GET: Admin/Timeslot/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var timeslot = await _timeslotService.GetTimeslotByIdAsync(id);
                if (timeslot == null)
                {
                    return NotFound();
                }
                return View(timeslot);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể tải thông tin timeslot: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Admin/Timeslot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _timeslotService.DeleteTimeslotAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa timeslot thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa timeslot.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa timeslot: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
