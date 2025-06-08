namespace Soccer.Font_end.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public int SizeId { get; set; }
        public string ImageUrl { get; set; }
    }
}
