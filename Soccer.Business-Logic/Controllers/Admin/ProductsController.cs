using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soccer.Business_Logic.DTO;
using Soccer.Data_Access.Models;

namespace Soccer.Business_Logic.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly SoccerContext _context;

        public ProductsController(SoccerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Select(p => new ProductDto
                {
                    ProductID = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    CategoryID = p.Category.CategoryId,
                    BrandID = p.Brand.BrandId,
                    SupplierID = p.Supplier.SupplierId,
                    CategoryName = p.Category.CategoryName,
                    BrandName = p.Brand.BrandName,
                    SupplierName = p.Supplier.SupplierName,
                    CostPrice = p.CostPrice,
                    SalePrice = p.SalePrice,
                    StockQuantity = p.StockQuantity,
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).ToList(),
                    Colors = p.ProductColors.Select(pc => pc.Color.ColorName).ToList(),
                    Sizes = p.ProductSizes.Select(ps => ps.Size.SizeName).ToList()
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Supplier)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            return Ok(new ProductDto
            {
                ProductID = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryID = product.CategoryId,
                BrandID = product.BrandId,
                SupplierID = product.SupplierId,
                CategoryName = product.Category.CategoryName,
                BrandName = product.Brand.BrandName,
                SupplierName = product.Supplier.SupplierName,
                CostPrice = product.CostPrice,
                SalePrice = product.SalePrice,
                StockQuantity = product.StockQuantity,
                ImageUrls = product.ProductImages.Select(i => i.ImageUrl).ToList(),
                Colors = product.ProductColors.Select(pc => pc.Color.ColorName).ToList(),
                Sizes = product.ProductSizes.Select(ps => ps.Size.SizeName).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductRequest request)
        {
            var product = new Product
            {
                CategoryId = request.CategoryID,
                BrandId = request.BrandID,
                SupplierId = request.SupplierID,
                ProductName = request.ProductName,
                Description = request.Description,
                CostPrice = request.CostPrice,
                SalePrice = request.SalePrice,
                StockQuantity = request.StockQuantity
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Add images
            if (request.ImageUrls?.Any() == true)
            {
                var images = request.ImageUrls.Select(url => new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = url
                }).ToList();
                _context.ProductImages.AddRange(images);
            }

            // Add colors
            if (request.ColorIds?.Any() == true)
            {
                var productColors = request.ColorIds.Select(colorId => new ProductColor
                {
                    ProductId = product.ProductId,
                    ColorId = colorId
                }).ToList();
                _context.ProductColors.AddRange(productColors);
            }

            // Add sizes
            if (request.SizeIds?.Any() == true)
            {
                var productSizes = request.SizeIds.Select(sizeId => new ProductSize
                {
                    ProductId = product.ProductId,
                    SizeId = sizeId
                }).ToList();
                _context.ProductSizes.AddRange(productSizes);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId },
                await GetProduct(product.ProductId));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.CategoryId = request.CategoryID;
            product.BrandId = request.BrandID;
            product.SupplierId = request.SupplierID;
            product.ProductName = request.ProductName;
            product.Description = request.Description;
            product.CostPrice = request.CostPrice;
            product.SalePrice = request.SalePrice;
            product.StockQuantity = request.StockQuantity;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
