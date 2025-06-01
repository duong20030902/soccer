using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Soccer.Font_end.Services;
using Soccer.Font_end.ViewModels;
using System.Security.Claims;

namespace Soccer.Font_end.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Authentication()
        {
            // Khởi tạo model mới, tránh null
            return View(new AuthViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            Console.WriteLine($"Dữ liệu register nhận được: {JsonConvert.SerializeObject(registerModel)}");

            ModelState.Clear();
            TryValidateModel(registerModel);

            if (!ModelState.IsValid)
            {
                Console.WriteLine($"Lỗi Register ModelState: {string.Join(" | ", ModelState)}");

                var authModel = new AuthViewModel
                {
                    Login = new LoginViewModel(),
                    Register = registerModel
                };

                ViewData["ActiveTab"] = "register";
                return View("Authentication", authModel);
            }

            var result = await _authService.RegisterAsync(registerModel);
            Console.WriteLine("Register result: " + result.Message);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Authentication");
            }

            ViewData["RegisterError"] = result.Message;
            ViewData["ActiveTab"] = "register";
            return View("Authentication", registerModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            Console.WriteLine($"Dữ liệu nhận được - Email: {model.Email}, Password: {model.Password}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Lỗi ModelState: " +
                    string.Join(" | ", ModelState.SelectMany(x => x.Value.Errors)));

                // Truyền lại AuthViewModel với dữ liệu đã nhập
                var authModel = new AuthViewModel
                {
                    Login = model,
                    Register = new RegisterViewModel()
                };

                ViewData["ActiveTab"] = "login";
                return View("Authentication", authModel);
            }

            var result = await _authService.LoginAsync(model);
            Console.WriteLine("Login result: " + result.Message);

            if (result.Success)
            {
                // Xử lý session/cookie như cũ
                return RedirectToAction("Index", "Home");
            }

            ViewData["LoginError"] = result.Message;
            ViewData["ActiveTab"] = "login";
            return View("Authentication", model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Authentication");
        }

        // Nếu bạn muốn có trang thông báo đăng ký thành công riêng
        public IActionResult RegistrationSuccess()
        {
            return View();
        }

        // Xử lý đăng nhập bằng Google (nếu cần)
        public IActionResult GoogleLogin()
        {
            // Xử lý đăng nhập Google
            return RedirectToAction("Authentication");
        }

        // Xử lý đăng nhập bằng Facebook (nếu cần)
        public IActionResult FacebookLogin()
        {
            // Xử lý đăng nhập Facebook
            return RedirectToAction("Authentication");
        }
    }
}