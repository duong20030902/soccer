using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: Hiển thị danh sách vai trò
        public async Task<IActionResult> Index()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return View(roles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<RoleDto>());
            }
        }

        // GET: Hiển thị chi tiết vai trò
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
                return View(role);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Hiển thị form tạo vai trò
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý tạo vai trò
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra vai trò đã tồn tại
                var roleExists = await _roleService.RoleExistsAsync(model.RoleName);
                if (roleExists)
                {
                    ModelState.AddModelError("RoleName", "Vai trò này đã tồn tại");
                    return View(model);
                }

                var result = await _roleService.CreateRoleAsync(model.RoleName);
                if (result != null)
                {
                    TempData["Success"] = "Tạo vai trò thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể tạo vai trò";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        // GET: Hiển thị form chỉnh sửa
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                var editModel = new EditRoleViewModel
                {
                    RoleID = role.RoleID,
                    RoleName = role.RoleName
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Xử lý cập nhật vai trò
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRoleViewModel model)
        {
            if (id != model.RoleID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra tên vai trò mới có trùng không (trừ chính nó)
                var existingRoles = await _roleService.GetAllRolesAsync();
                var duplicateRole = existingRoles.FirstOrDefault(r =>
                    r.RoleName.Equals(model.RoleName, StringComparison.OrdinalIgnoreCase) &&
                    r.RoleID != model.RoleID);

                if (duplicateRole != null)
                {
                    ModelState.AddModelError("RoleName", "Tên vai trò này đã tồn tại");
                    return View(model);
                }

                var result = await _roleService.UpdateRoleAsync(id, model.RoleName);
                if (result)
                {
                    TempData["Success"] = "Cập nhật vai trò thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật vai trò";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        // POST: Xóa vai trò
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa vai trò thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa vai trò";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
