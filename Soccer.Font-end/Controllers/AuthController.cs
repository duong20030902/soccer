/*using Microsoft.AspNetCore.Authentication.Cookies;
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
}*/


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

        // Trang đăng nhập
        /*        [HttpGet]
                public IActionResult Login(string verified, string error)
                {
                    // Kiểm tra nếu user đã đăng nhập
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    // Xử lý thông báo từ email verification
                    if (!string.IsNullOrEmpty(verified) && verified == "true")
                    {
                        ViewData["SuccessMessage"] = "Xác nhận email thành công! Bạn có thể đăng nhập ngay bây giờ.";
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        switch (error)
                        {
                            case "invalid_token":
                                ViewData["ErrorMessage"] = "Link xác nhận không hợp lệ hoặc đã hết hạn.";
                                break;
                            default:
                                ViewData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình xác nhận email.";
                                break;
                        }
                    }

                    return View(new LoginViewModel());
                }*/


        // Trang đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.Success && result.Data != null)
            {
                try
                {
                    LoginResponse loginResponse;

                    // Kiểm tra xem Data đã là object hay vẫn là string
                    if (result.Data is string jsonString)
                    {
                        loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonString);
                    }
                    else
                    {
                        // Nếu Data đã là object, serialize rồi deserialize lại
                        var jsonStr = JsonConvert.SerializeObject(result.Data);
                        loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonStr);
                    }

                    if (loginResponse?.User != null)
                    {
                        // Lưu thông tin user vào session
                        HttpContext.Session.SetString("UserId", loginResponse.User.UserId.ToString());
                        HttpContext.Session.SetString("UserName", loginResponse.User.FullName ?? "");
                        HttpContext.Session.SetString("UserEmail", loginResponse.User.Email ?? "");
                        HttpContext.Session.SetString("RoleId", loginResponse.User.RoleId.ToString());

                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing login response: {ex.Message}");
                    ViewData["LoginError"] = "Lỗi xử lý thông tin đăng nhập";
                    return View(model);
                }
            }

            ViewData["LoginError"] = result.Message ?? "Đăng nhập thất bại";
            return View(model);
        }

        // Trang đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            // Kiểm tra nếu user đã đăng nhập
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            Console.WriteLine($"Dữ liệu register nhận được: {JsonConvert.SerializeObject(model)}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine($"Lỗi Register ModelState: {string.Join(" | ", ModelState)}");
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);
            Console.WriteLine("Register result: " + result.Message);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ViewData["RegisterError"] = result.Message;
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
        

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        // Xử lý đăng nhập bằng Google (nếu cần)
        public IActionResult GoogleLogin()
        {
            // Xử lý đăng nhập Google
            return RedirectToAction("Login");
        }

        // Xử lý đăng nhập bằng Facebook (nếu cần)
        public IActionResult FacebookLogin()
        {
            // Xử lý đăng nhập Facebook
            return RedirectToAction("Login");
        }
    }
}
