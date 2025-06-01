using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderStatusController : Controller
    {
        private readonly OrderStatusService _orderStatusService;

        public OrderStatusController(OrderStatusService orderStatusService)
        {
            _orderStatusService = orderStatusService;
        }

        // GET: Hiển thị danh sách OrderStatus
        public async Task<IActionResult> Index()
        {
            var orderStatuses = await _orderStatusService.GetAllOrderStatusesAsync();
            return View(orderStatuses);
        }

        // GET: Hiển thị form tạo mới
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý tạo OrderStatus mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
            {
                ModelState.AddModelError("", "Tên trạng thái không được để trống");
                return View();
            }

            var result = await _orderStatusService.CreateOrderStatusAsync(statusName);

            if (result)
            {
                TempData["SuccessMessage"] = "Tạo trạng thái thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi tạo trạng thái");
            return View();
        }

        // GET: Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int id)
        {
            var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);

            if (orderStatus == null)
            {
                return NotFound();
            }

            return View(orderStatus);
        }

        // POST: Xử lý cập nhật OrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
            {
                ModelState.AddModelError("", "Tên trạng thái không được để trống");
                var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);
                return View(orderStatus);
            }

            var result = await _orderStatusService.UpdateOrderStatusAsync(id, statusName);

            if (result)
            {
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật trạng thái");
            var status = await _orderStatusService.GetOrderStatusByIdAsync(id);
            return View(status);
        }

        // POST: Xử lý xóa OrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderStatusService.DeleteOrderStatusAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Xóa trạng thái thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa trạng thái";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Hiển thị chi tiết OrderStatus
        public async Task<IActionResult> Details(int id)
        {
            var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);

            if (orderStatus == null)
            {
                return NotFound();
            }

            return View(orderStatus);
        }
    }
}
