using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShippingMethodController : Controller
    {
        private readonly ShippingMethodService _shippingMethodService;

        public ShippingMethodController(ShippingMethodService shippingMethodService)
        {
            _shippingMethodService = shippingMethodService ?? throw new ArgumentNullException(nameof(shippingMethodService));
        }

        public async Task<IActionResult> Index()
        {
            var shippingMethods = await _shippingMethodService.GetShippingMethodsAsync();
            return View(shippingMethods);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShippingMethodRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Method name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _shippingMethodService.CreateShippingMethodAsync(request);
                TempData["Success"] = "Shipping method created successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create shipping method: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateShippingMethodRequest request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Method name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _shippingMethodService.UpdateShippingMethodAsync(id, request);
                TempData["Success"] = "Shipping method updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update shipping method: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _shippingMethodService.DeleteShippingMethodAsync(id);
                TempData["Success"] = "Shipping method deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete shipping method: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
