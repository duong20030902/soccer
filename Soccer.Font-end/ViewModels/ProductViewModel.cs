namespace Soccer.Font_end.ViewModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal SalePrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int? BrandID { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int StockQuantity { get; set; }
        public List<ColorViewModel> AvailableColors { get; set; } = new List<ColorViewModel>();
        public List<SizeViewModel> AvailableSizes { get; set; } = new List<SizeViewModel>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}
