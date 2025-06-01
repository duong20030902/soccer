using Microsoft.AspNetCore.Mvc;
using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.ViewModels;

namespace Soccer.Font_end.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ReportService _reportService;

        public HomeController(ReportService reportService)
        {
            _reportService = reportService;
        }

        // Dashboard page
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboard = await _reportService.GetDashboardAsync();

                if (dashboard == null)
                {
                    ViewBag.ErrorMessage = "Unable to load dashboard data";
                    dashboard = new DashboardDto(); // Empty dashboard to prevent null reference
                }

                return View(dashboard);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while loading the dashboard";
                return View(new DashboardDto());
            }
        }

        // Sales Report page
        [HttpGet]
        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);

                if (salesReport == null)
                {
                    ViewBag.ErrorMessage = "Unable to load sales report";
                    salesReport = new SalesReportDto
                    {
                        StartDate = startDate ?? DateTime.Today.AddDays(-30),
                        EndDate = endDate ?? DateTime.Today
                    };
                }

                return View(salesReport);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while loading the sales report";
                return View(new SalesReportDto());
            }
        }

        // Inventory Report page
        public async Task<IActionResult> InventoryReport()
        {
            try
            {
                var inventoryReport = await _reportService.GetInventoryReportAsync();

                if (inventoryReport == null || !inventoryReport.Any())
                {
                    ViewBag.ErrorMessage = "No inventory data available";
                    inventoryReport = new List<InventoryReportDto>();
                }

                return View(inventoryReport);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while loading the inventory report";
                return View(new List<InventoryReportDto>());
            }
        }

        // API endpoint for AJAX calls (optional)
        [HttpGet]
        public async Task<JsonResult> GetDashboardData()
        {
            try
            {
                var dashboard = await _reportService.GetDashboardAsync();
                return Json(new { success = true, data = dashboard });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading dashboard data" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetSalesData(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var salesReport = await _reportService.GetSalesReportAsync(startDate, endDate);
                return Json(new { success = true, data = salesReport });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading sales data" });
            }
        }
    }
}
