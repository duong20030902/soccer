namespace Soccer.Business_Logic.DTO
{
    public class OrderDto
    {
        public int OrderID { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingMethod { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
