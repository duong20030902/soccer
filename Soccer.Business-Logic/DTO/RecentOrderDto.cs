namespace Soccer.Business_Logic.DTO
{
    public class RecentOrderDto
    {
        public int OrderID { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
