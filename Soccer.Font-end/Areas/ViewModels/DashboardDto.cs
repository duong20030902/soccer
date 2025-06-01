namespace Soccer.Font_end.ViewModels
{
    public class DashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBookings { get; set; }
        public int TodayOrders { get; set; }
        public int TodayBookings { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal MonthlyBookingRevenue { get; set; }
        public List<TopProductDto> TopSellingProducts { get; set; }
        public List<RecentOrderDto> RecentOrders { get; set; }
    }

    public class TopProductDto
    {
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
    }

    public class RecentOrderDto
    {
        public int OrderID { get; set; }
        public string UserName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
