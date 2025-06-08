using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly SoccerContext _context;

        public ProductController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts(string sort = null, int? colorId = null, int? sizeId = null, int? categoryId = null, int? brandId = null, decimal? minPrice = null, decimal? maxPrice = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductReviews)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .AsSplitQuery() //mới thêm để tránh quá nhiều include gây duplicate
                .AsQueryable();

            // Lọc theo danh mục
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            // Lọc theo thương hiệu
            if (brandId.HasValue)
                query = query.Where(p => p.BrandId == brandId);

            // Lọc theo màu sắc
            if (colorId.HasValue)
                query = query.Where(p => p.ProductColors.Any(pc => pc.ColorId == colorId));

            // Lọc theo kích thước
            if (sizeId.HasValue)
                query = query.Where(p => p.ProductSizes.Any(ps => ps.SizeId == sizeId));

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
                query = query.Where(p => p.SalePrice >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.SalePrice <= maxPrice);

            // Xử lý sắp xếp
            switch (sort?.ToLower())
            {
                case "price_asc":
                    query = query.OrderBy(p => p.SalePrice);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.SalePrice);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    query = query.OrderBy(p => p.ProductId);
                    break;
            }

            // Phân trang
            var totalItems = await query.CountAsync();
            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    ProductID = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description ?? string.Empty,
                    SalePrice = p.SalePrice,
                    CategoryName = p.Category.CategoryName,
                    BrandName = p.Brand.BrandName,
                    ImageURL = p.ProductImages.FirstOrDefault().ImageUrl ?? string.Empty,
                    AverageRating = p.ProductReviews.Any() ? p.ProductReviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.ProductReviews.Count,
                    StockQuantity = p.StockQuantity,
                    AvailableColors = p.ProductColors.Select(pc => new ColorViewModel
                    {
                        ColorId = pc.Color.ColorId,
                        ColorName = pc.Color.ColorName,
                        ColorCode = pc.Color.ColorCode
                    }).ToList(),
                    AvailableSizes = p.ProductSizes.Select(ps => new SizeViewModel
                    {
                        SizeId = ps.Size.SizeId,
                        SizeName = ps.Size.SizeName,
                        SizeOrder = ps.Size.SizeOrder
                    }).ToList(),
                    Images = p.ProductImages.Select(i => new ProductImage
                    {
                        ImageId = i.ImageId,
                        ImageUrl = i.ImageUrl
                    }).ToList()
                }).ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Data = result
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductReviews)
                    .ThenInclude(r => r.User)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    p.CostPrice,
                    p.SalePrice,
                    p.StockQuantity,
                    Category = new
                    {
                        p.Category.CategoryId,
                        p.Category.CategoryName
                    },
                    Brand = new
                    {
                        p.Brand.BrandId,
                        p.Brand.BrandName
                    },
                    Supplier = new
                    {
                        p.Supplier.SupplierId,
                        p.Supplier.SupplierName,
                        p.Supplier.ContactInfo
                    },
                    Images = p.ProductImages.Select(i => new
                    {
                        i.ImageId,
                        i.ImageUrl
                    }),
                    Reviews = p.ProductReviews.Select(r => new
                    {
                        r.ReviewId,
                        r.Rating,
                        r.Comment,
                        r.ReviewDate,
                        User = new
                        {
                            r.User.UserId,
                            r.User.FullName
                        }
                    }),
                    AverageRating = p.ProductReviews.Any() ? p.ProductReviews.Average(r => r.Rating) : 0,
                    ReviewCount = p.ProductReviews.Count,
                    AvailableColors = p.ProductColors.Select(pc => new
                    {
                        pc.Color.ColorId,
                        pc.Color.ColorCode,
                        pc.Color.ColorName
                    }),
                    AvailableSizes = p.ProductSizes.Select(ps => new
                    {
                        ps.Size.SizeId,
                        ps.Size.SizeName
                    })
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return BadRequest("Không tìm thấy sản phẩm.");
            }

            return Ok(product);
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return await GetAllProducts();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Where(p => p.ProductName.Contains(query) ||
                           p.Description.Contains(query) ||
                           p.Category.CategoryName.Contains(query) ||
                           p.Brand.BrandName.Contains(query))
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    p.SalePrice,
                    CategoryName = p.Category.CategoryName,
                    BrandName = p.Brand.BrandName,
                    ImageURL = p.ProductImages.FirstOrDefault().ImageUrl
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            var brands = await _context.Brands
                .Select(b => new { b.BrandId, b.BrandName })
                .ToListAsync();
            return Ok(brands);
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var brands = await _context.Categories
                .Select(b => new { b.CategoryId, b.CategoryName })
                .ToListAsync();
            return Ok(brands);
        }

        [HttpGet("colors")]
        public async Task<ActionResult<IEnumerable<Color>>> GetColors()
        {
            var brands = await _context.Colors
                .Select(b => new { b.ColorId, b.ColorName, b.ColorCode })
                .ToListAsync();
            return Ok(brands);
        }

        [HttpGet("sizes")]
        public async Task<ActionResult<IEnumerable<Size>>> GetSizes()
        {
            var brands = await _context.Sizes
                .Select(b => new { b.SizeId, b.SizeName, b.SizeOrder })
                .ToListAsync();
            return Ok(brands);
        }
    }

    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int StockQuantity { get; set; }
        public List<ColorViewModel> AvailableColors { get; set; } = new List<ColorViewModel>();
        public List<SizeViewModel> AvailableSizes { get; set; } = new List<SizeViewModel>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
    }

    public class ColorViewModel
    {
        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty;
        public string ColorCode { get; set; } = string.Empty;
    }

    public class SizeViewModel
    {
        public int SizeId { get; set; }
        public string SizeName { get; set; } = string.Empty;
        public int SizeOrder { get; set; }
    }
}
