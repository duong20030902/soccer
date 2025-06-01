using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly SoccerContext _context;

        public HomeController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GlobalSearch([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Từ khóa tìm kiếm không được để trống");
            }

            if (query.Length < 2)
            {
                return BadRequest("Từ khóa tìm kiếm phải có ít nhất 2 ký tự");
            }

            try
            {
                var searchTerm = query.ToLower().Trim();
                var results = new GlobalSearchResult();

                // Tìm kiếm Users
                var users = await _context.Users
                    .Where(u => u.FullName.ToLower().Contains(searchTerm) ||
                               u.Email.ToLower().Contains(searchTerm) ||
                               u.Phone.Contains(searchTerm))
                    .Include(u => u.Role)
                    .Select(u => new SearchResultItem
                    {
                        Id = u.UserId,
                        Title = u.FullName,
                        Description = $"Email: {u.Email} | Phone: {u.Phone} | Role: {u.Role.RoleName}",
                        Type = "User",
                        Status = u.IsActive ? "Active" : "Inactive"
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Products
                var products = await _context.Products
                    .Where(p => p.ProductName.ToLower().Contains(searchTerm) ||
                               p.Description.ToLower().Contains(searchTerm))
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Select(p => new SearchResultItem
                    {
                        Id = p.ProductId,
                        Title = p.ProductName,
                        Description = $"Brand: {p.Brand.BrandName} | Category: {p.Category.CategoryName} | Price: {p.SalePrice:C}",
                        Type = "Product",
                        Status = p.StockQuantity > 0 ? "In Stock" : "Out of Stock"
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Orders
                var orders = await _context.Orders
                    .Where(o => o.OrderId.ToString().Contains(searchTerm))
                    .Include(o => o.User)
                    .Include(o => o.Status)
                    .Select(o => new SearchResultItem
                    {
                        Id = o.OrderId,
                        Title = $"Order #{o.OrderId}",
                        Description = $"Customer: {o.User.FullName} | Total: {o.TotalAmount:C} | Date: {o.OrderDate:dd/MM/yyyy}",
                        Type = "Order",
                        Status = o.Status.StatusName
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Fields
                var fields = await _context.Fields
                    .Where(f => f.FieldName.ToLower().Contains(searchTerm) ||
                               f.Location.ToLower().Contains(searchTerm))
                    .Select(f => new SearchResultItem
                    {
                        Id = f.FieldId,
                        Title = f.FieldName,
                        Description = $"Location: {f.Location} | Price: {f.PricePerHour:C}/hour",
                        Type = "Field",
                        Status = "Available"
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Bookings
                var bookings = await _context.Bookings
                    .Where(b => b.BookingId.ToString().Contains(searchTerm))
                    .Include(b => b.User)
                    .Include(b => b.Schedule)
                    .ThenInclude(s => s.Field)
                    .Select(b => new SearchResultItem
                    {
                        Id = b.BookingId,
                        Title = $"Booking #{b.BookingId}",
                        Description = $"Customer: {b.User.FullName} | Field: {b.Schedule.Field.FieldName} | Price: {b.Price:C}",
                        Type = "Booking",
                        Status = b.Status
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Brands
                var brands = await _context.Brands
                    .Where(b => b.BrandName.ToLower().Contains(searchTerm))
                    .Select(b => new SearchResultItem
                    {
                        Id = b.BrandId,
                        Title = b.BrandName,
                        Description = "Brand",
                        Type = "Brand",
                        Status = "Active"
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Categories
                var categories = await _context.Categories
                    .Where(c => c.CategoryName.ToLower().Contains(searchTerm))
                    .Select(c => new SearchResultItem
                    {
                        Id = c.CategoryId,
                        Title = c.CategoryName,
                        Description = c.ParentId.HasValue ? "Sub Category" : "Main Category",
                        Type = "Category",
                        Status = "Active"
                    })
                    .Take(5)
                    .ToListAsync();

                // Tìm kiếm Suppliers
                var suppliers = await _context.Suppliers
                    .Where(s => s.SupplierName.ToLower().Contains(searchTerm) ||
                               s.ContactInfo.ToLower().Contains(searchTerm))
                    .Select(s => new SearchResultItem
                    {
                        Id = s.SupplierId,
                        Title = s.SupplierName,
                        Description = $"Contact: {s.ContactInfo}",
                        Type = "Supplier",
                        Status = "Active"
                    })
                    .Take(5)
                    .ToListAsync();

                // Gộp tất cả kết quả
                results.Users = users;
                results.Products = products;
                results.Orders = orders;
                results.Fields = fields;
                results.Bookings = bookings;
                results.Brands = brands;
                results.Categories = categories;
                results.Suppliers = suppliers;

                // Tính tổng số kết quả
                results.TotalResults = users.Count + products.Count + orders.Count +
                                     fields.Count + bookings.Count + brands.Count +
                                     categories.Count + suppliers.Count;

                results.SearchTerm = query;
                results.SearchTime = DateTime.Now;

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("search/suggestions")]
        public async Task<IActionResult> GetSearchSuggestions([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return Ok(new List<string>());
            }

            try
            {
                var searchTerm = query.ToLower().Trim();
                var suggestions = new List<string>();

                // Gợi ý từ tên sản phẩm
                var productSuggestions = await _context.Products
                    .Where(p => p.ProductName.ToLower().Contains(searchTerm))
                    .Select(p => p.ProductName)
                    .Take(3)
                    .ToListAsync();

                // Gợi ý từ tên người dùng
                var userSuggestions = await _context.Users
                    .Where(u => u.FullName.ToLower().Contains(searchTerm))
                    .Select(u => u.FullName)
                    .Take(3)
                    .ToListAsync();

                // Gợi ý từ tên thương hiệu
                var brandSuggestions = await _context.Brands
                    .Where(b => b.BrandName.ToLower().Contains(searchTerm))
                    .Select(b => b.BrandName)
                    .Take(2)
                    .ToListAsync();

                suggestions.AddRange(productSuggestions);
                suggestions.AddRange(userSuggestions);
                suggestions.AddRange(brandSuggestions);

                return Ok(suggestions.Distinct().Take(8));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("search/quick")]
        public async Task<IActionResult> QuickSearch([FromQuery] string query, [FromQuery] string type = "all")
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Từ khóa tìm kiếm không được để trống");
            }

            try
            {
                var searchTerm = query.ToLower().Trim();
                var results = new List<SearchResultItem>();

                switch (type.ToLower())
                {
                    case "users":
                        results = await SearchUsers(searchTerm);
                        break;
                    case "products":
                        results = await SearchProducts(searchTerm);
                        break;
                    case "orders":
                        results = await SearchOrders(searchTerm);
                        break;
                    case "fields":
                        results = await SearchFields(searchTerm);
                        break;
                    default:
                        // Tìm kiếm nhanh tất cả
                        var users = await SearchUsers(searchTerm, 3);
                        var products = await SearchProducts(searchTerm, 3);
                        var orders = await SearchOrders(searchTerm, 2);

                        results.AddRange(users);
                        results.AddRange(products);
                        results.AddRange(orders);
                        break;
                }

                return Ok(results.Take(10));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        // Helper methods
        private async Task<List<SearchResultItem>> SearchUsers(string searchTerm, int limit = 10)
        {
            return await _context.Users
                .Where(u => u.FullName.ToLower().Contains(searchTerm) ||
                           u.Email.ToLower().Contains(searchTerm))
                .Include(u => u.Role)
                .Select(u => new SearchResultItem
                {
                    Id = u.UserId,
                    Title = u.FullName,
                    Description = $"{u.Email} | {u.Role.RoleName}",
                    Type = "User",
                    Status = u.IsActive ? "Active" : "Inactive"
                })
                .Take(limit)
                .ToListAsync();
        }

        private async Task<List<SearchResultItem>> SearchProducts(string searchTerm, int limit = 10)
        {
            return await _context.Products
                .Where(p => p.ProductName.ToLower().Contains(searchTerm))
                .Include(p => p.Brand)
                .Select(p => new SearchResultItem
                {
                    Id = p.ProductId,
                    Title = p.ProductName,
                    Description = $"{p.Brand.BrandName} | {p.SalePrice:C}",
                    Type = "Product",
                    Status = p.StockQuantity > 0 ? "In Stock" : "Out of Stock"
                })
                .Take(limit)
                .ToListAsync();
        }

        private async Task<List<SearchResultItem>> SearchOrders(string searchTerm, int limit = 10)
        {
            return await _context.Orders
                .Where(o => o.OrderId.ToString().Contains(searchTerm) ||
                           o.User.FullName.ToLower().Contains(searchTerm))
                .Include(o => o.User)
                .Include(o => o.Status)
                .Select(o => new SearchResultItem
                {
                    Id = o.OrderId,
                    Title = $"Order #{o.OrderId}",
                    Description = $"{o.User.FullName} | {o.TotalAmount:C}",
                    Type = "Order",
                    Status = o.Status.StatusName
                })
                .Take(limit)
                .ToListAsync();
        }

        private async Task<List<SearchResultItem>> SearchFields(string searchTerm, int limit = 10)
        {
            return await _context.Fields
                .Where(f => f.FieldName.ToLower().Contains(searchTerm) ||
                           f.Location.ToLower().Contains(searchTerm))
                .Select(f => new SearchResultItem
                {
                    Id = f.FieldId,
                    Title = f.FieldName,
                    Description = $"{f.Location} | {f.PricePerHour:C}/hour",
                    Type = "Field",
                    Status = "Available"
                })
                .Take(limit)
                .ToListAsync();
        }
    }

    // Models for search results
    public class GlobalSearchResult
    {
        public string SearchTerm { get; set; }
        public DateTime SearchTime { get; set; }
        public int TotalResults { get; set; }
        public List<SearchResultItem> Users { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Products { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Orders { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Fields { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Bookings { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Brands { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Categories { get; set; } = new List<SearchResultItem>();
        public List<SearchResultItem> Suppliers { get; set; } = new List<SearchResultItem>();
    }

    public class SearchResultItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
