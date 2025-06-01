namespace Soccer.Font_end.ViewModels
{
    public class ProductInfo
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public string StockStatus { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public Supplier Supplier { get; set; }
    }
}
