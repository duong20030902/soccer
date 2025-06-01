using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.Areas.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Hiển thị danh sách categories
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<CategoryDto>());
            }
        }

        // Hiển thị form tạo category
        public async Task<IActionResult> Create()
        {
            try
            {
                // Lấy danh sách categories cha để làm dropdown
                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Xử lý tạo category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories;
                return View(request);
            }

            try
            {
                var result = await _categoryService.CreateCategoryAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo danh mục thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Không thể tạo danh mục!";
                    var parentCategories = await _categoryService.GetParentCategoriesAsync();
                    ViewBag.ParentCategories = parentCategories;
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories;
                return View(request);
            }
        }

        // Hiển thị form chỉnh sửa
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    TempData["Error"] = "Không tìm thấy danh mục!";
                    return RedirectToAction("Index");
                }

                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories.Where(c => c.CategoryID != id).ToList();

                var updateRequest = new UpdateCategoryRequest
                {
                    CategoryName = category.CategoryName,
                    ParentID = category.ParentID
                };

                return View(updateRequest);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Xử lý cập nhật category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories.Where(c => c.CategoryID != id).ToList();
                return View(request);
            }

            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật danh mục!";
                    var parentCategories = await _categoryService.GetParentCategoriesAsync();
                    ViewBag.ParentCategories = parentCategories.Where(c => c.CategoryID != id).ToList();
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                var parentCategories = await _categoryService.GetParentCategoriesAsync();
                ViewBag.ParentCategories = parentCategories.Where(c => c.CategoryID != id).ToList();
                return View(request);
            }
        }

        // Xóa category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa danh mục thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa danh mục!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        // API để lấy categories con (dùng cho AJAX)
        [HttpGet]
        public async Task<JsonResult> GetChildCategories(int parentId)
        {
            try
            {
                var childCategories = await _categoryService.GetChildCategoriesAsync(parentId);
                return Json(childCategories);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
