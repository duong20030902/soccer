namespace Soccer.Font_end.ViewModels
{
    public class InventoryReportDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public int StockQuantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal StockValue { get; set; }
        public bool IsLowStock { get; set; }
    }
}
