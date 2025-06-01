using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public int BrandID { get; set; }
        public int SupplierID { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string SupplierName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> Colors { get; set; } = new();
        public List<string> Sizes { get; set; } = new();
    }

    public class CreateProductRequest
    {
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public int BrandID { get; set; }
        [Required]
        public int SupplierID { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal CostPrice { get; set; }
        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<int> ColorIds { get; set; }
        public List<int> SizeIds { get; set; }
    }

    public class UpdateProductRequest
    {
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public int BrandID { get; set; }
        [Required]
        public int SupplierID { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string Description { get; set; }
        [Required]
        public decimal CostPrice { get; set; }
        [Required]
        public decimal SalePrice { get; set; }
        [Required]
        public int StockQuantity { get; set; }
    }
}
