namespace Soccer.Business_Logic.DTO
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
}
