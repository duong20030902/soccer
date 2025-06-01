using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentMethodController : Controller
    {
        private readonly PaymentMethodService _paymentMethodService;

        public PaymentMethodController(PaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService ?? throw new ArgumentNullException(nameof(paymentMethodService));
        }

        public async Task<IActionResult> Index()
        {
            var paymentMethods = await _paymentMethodService.GetPaymentMethodsAsync();
            return View(paymentMethods);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                TempData["Error"] = "Method name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _paymentMethodService.CreatePaymentMethodAsync(methodName);
                TempData["Success"] = "Payment method created successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to create payment method: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                TempData["Error"] = "Method name is required.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _paymentMethodService.UpdatePaymentMethodAsync(id, methodName);
                TempData["Success"] = "Payment method updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update payment method: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _paymentMethodService.DeletePaymentMethodAsync(id);
                TempData["Success"] = "Payment method deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to delete payment method: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
