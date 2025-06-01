using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public ReportsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);

            var dashboard = new DashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalBookings = await _context.Bookings.CountAsync(),
                TodayOrders = await _context.Orders.CountAsync(o => o.OrderDate.Date == today),
                TodayBookings = await _context.Bookings.CountAsync(b => b.BookingTime.Date == today),
                MonthlyRevenue = await _context.Orders
                    .Where(o => o.OrderDate >= thisMonth)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m,
                MonthlyBookingRevenue = await _context.Bookings
                    .Where(b => b.BookingTime >= thisMonth)
                    .SumAsync(b => (decimal?)b.Price) ?? 0m,
                TopSellingProducts = await _context.OrderItems
                    .GroupBy(oi => new { oi.ProductId, oi.Product.ProductName })
                    .Select(g => new TopProductDto
                    {
                        ProductName = g.Key.ProductName,
                        TotalSold = g.Sum(oi => oi.Quantity)
                    })
                    .OrderByDescending(tp => tp.TotalSold)
                    .Take(5)
                    .ToListAsync(),
                RecentOrders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(10)
                    .Select(o => new RecentOrderDto
                    {
                        OrderID = o.OrderId,
                        UserName = o.User.FullName,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status.StatusName,
                        OrderDate = o.OrderDate
                    })
                    .ToListAsync()
            };

            return Ok(dashboard);
        }

        [HttpGet("sales-report")]
        public async Task<ActionResult<SalesReportDto>> GetSalesReport(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today.AddDays(1);

            var report = new SalesReportDto
            {
                StartDate = start,
                EndDate = end,
                TotalOrders = await _context.Orders
                    .CountAsync(o => o.OrderDate >= start && o.OrderDate < end),
                TotalRevenue = await _context.Orders
                    .Where(o => o.OrderDate >= start && o.OrderDate < end)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m,
                TotalBookings = await _context.Bookings
                    .CountAsync(b => b.BookingTime >= start && b.BookingTime < end),
                BookingRevenue = await _context.Bookings
                    .Where(b => b.BookingTime >= start && b.BookingTime < end)
                    .SumAsync(b => (decimal?)b.Price) ?? 0m,
                Commission = await _context.Bookings
                    .Where(b => b.BookingTime >= start && b.BookingTime < end)
                    .SumAsync(b => (decimal?)b.Commission) ?? 0m
            };

            return Ok(report);
        }

        [HttpGet("inventory-report")]
        public async Task<ActionResult<IEnumerable<InventoryReportDto>>> GetInventoryReport()
        {
            var inventory = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Select(p => new InventoryReportDto
                {
                    ProductID = p.ProductId,
                    ProductName = p.ProductName,
                    CategoryName = p.Category.CategoryName,
                    BrandName = p.Brand.BrandName,
                    StockQuantity = p.StockQuantity,
                    CostPrice = p.CostPrice,
                    SalePrice = p.SalePrice,
                    StockValue = p.StockQuantity * p.CostPrice,
                    IsLowStock = p.StockQuantity < 10
                })
                .OrderBy(i => i.StockQuantity)
                .ToListAsync();

            return Ok(inventory);
        }
    }
}
