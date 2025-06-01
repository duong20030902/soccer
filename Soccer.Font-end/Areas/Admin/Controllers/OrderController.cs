using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders == null)
            {
                // Có thể show error message hoặc empty list
                orders = new List<OrderDto>();
                ViewBag.ErrorMessage = "Unable to load orders. Please try again later.";
            }

            return View(orders);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Order/UpdateStatus
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, int statusId)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, statusId);

            if (result)
            {
                TempData["SuccessMessage"] = "Order status updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update order status.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Order/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderService.DeleteOrderAsync(id);

            if (result)
            {
                TempData["SuccessMessage"] = "Order deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete order.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Order/Edit/5 - For status update form
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // You might want to load available statuses here
            // ViewBag.Statuses = await GetOrderStatuses();

            return View(order);
        }
    }
}
