using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        // GET: Hiển thị danh sách nhà cung cấp
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            try
            {
                List<SupplierDto> suppliers;

                if (string.IsNullOrEmpty(searchTerm))
                {
                    suppliers = await _supplierService.GetAllSuppliersAsync();
                }
                else
                {
                    suppliers = await _supplierService.SearchSuppliersAsync(searchTerm);
                    ViewBag.SearchTerm = searchTerm;
                }

                return View(suppliers);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<SupplierDto>());
            }
        }

        // GET: Hiển thị chi tiết nhà cung cấp
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound();
                }
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Hiển thị form tạo nhà cung cấp
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý tạo nhà cung cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSupplierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra nhà cung cấp đã tồn tại
                var supplierExists = await _supplierService.SupplierExistsAsync(model.SupplierName);
                if (supplierExists)
                {
                    ModelState.AddModelError("SupplierName", "Nhà cung cấp này đã tồn tại");
                    return View(model);
                }

                var request = new CreateSupplierRequest
                {
                    SupplierName = model.SupplierName,
                    ContactInfo = model.ContactInfo
                };

                var result = await _supplierService.CreateSupplierAsync(request);
                if (result != null)
                {
                    TempData["Success"] = "Tạo nhà cung cấp thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể tạo nhà cung cấp";
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
                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound();
                }

                var editModel = new EditSupplierViewModel
                {
                    SupplierID = supplier.SupplierID,
                    SupplierName = supplier.SupplierName,
                    ContactInfo = supplier.ContactInfo
                };

                return View(editModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Xử lý cập nhật nhà cung cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditSupplierViewModel model)
        {
            if (id != model.SupplierID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra tên nhà cung cấp mới có trùng không (trừ chính nó)
                var existingSuppliers = await _supplierService.GetAllSuppliersAsync();
                var duplicateSupplier = existingSuppliers.FirstOrDefault(s =>
                    s.SupplierName.Equals(model.SupplierName, StringComparison.OrdinalIgnoreCase) &&
                    s.SupplierID != model.SupplierID);

                if (duplicateSupplier != null)
                {
                    ModelState.AddModelError("SupplierName", "Tên nhà cung cấp này đã tồn tại");
                    return View(model);
                }

                var request = new UpdateSupplierRequest
                {
                    SupplierName = model.SupplierName,
                    ContactInfo = model.ContactInfo
                };

                var result = await _supplierService.UpdateSupplierAsync(id, request);
                if (result)
                {
                    TempData["Success"] = "Cập nhật nhà cung cấp thành công!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Không thể cập nhật nhà cung cấp";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        // POST: Xóa nhà cung cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _supplierService.DeleteSupplierAsync(id);
                if (result)
                {
                    TempData["Success"] = "Xóa nhà cung cấp thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa nhà cung cấp";
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
