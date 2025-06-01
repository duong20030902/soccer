using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Controllers
{
    public class ForgotPWController : Controller
    {
        private readonly ForgotPasswordService _forgotPasswordService;

        public ForgotPWController(ForgotPasswordService forgotPasswordService)
        {
            _forgotPasswordService = forgotPasswordService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendResetLink(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var (message, token) = await _forgotPasswordService.SendResetLinkAsync(model.Email);
            if (message.Contains("Success") || message.Contains("hướng dẫn đặt lại mật khẩu"))
            {
                TempData["SuccessMessage"] = "Nếu email tồn tại, bạn sẽ nhận được hướng dẫn đặt lại mật khẩu.";
                if (!string.IsNullOrEmpty(token))
                {
                    TempData["ResetToken"] = token;
                }
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, message);
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                ViewData["ErrorMessage"] = "Token không hợp lệ.";
                return View("Index", new ForgotPasswordViewModel());
            }

            var isValidToken = await _forgotPasswordService.ValidateResetTokenAsync(token);
            if (!isValidToken)
            {
                ViewData["ErrorMessage"] = "Token không hợp lệ hoặc đã hết hạn.";
                return View("Index", new ForgotPasswordViewModel());
            }

            var model = new ResetPasswordViewModel { Token = token };
            return View("ResetPassword", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }

            var result = await _forgotPasswordService.ResetPasswordAsync(model);
            if (result.Contains("thành công"))
            {
                TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập với mật khẩu mới.";
                return RedirectToAction("Index", "Auth");
            }

            ModelState.AddModelError(string.Empty, result);
            return View("ResetPassword", model);
        }
    }
}