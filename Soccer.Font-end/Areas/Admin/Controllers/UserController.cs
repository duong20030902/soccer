using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _userService.CreateUserAsync(request);
            if (result != null)
            {
                TempData["SuccessMessage"] = "Người dùng đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo người dùng.";
            return View(request);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var updateRequest = new UpdateUserRequest
            {
                RoleID = user.RoleID,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive
            };
            ViewBag.UserId = id;
            return View(updateRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _userService.UpdateUserAsync(id, request);
            if (result)
            {
                TempData["SuccessMessage"] = "Thông tin người dùng đã được cập nhật!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin.";
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBalance(int id, decimal amount)
        {
            var result = await _userService.UpdateUserBalanceAsync(id, amount);
            return Json(new { success = result });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Json(new { success = result });
        }

        // API endpoints cho AJAX calls
        [HttpGet]
        public async Task<IActionResult> GetUsersData()
        {
            var users = await _userService.GetAllUsersAsync();
            return Json(users);
        }
    }
}
