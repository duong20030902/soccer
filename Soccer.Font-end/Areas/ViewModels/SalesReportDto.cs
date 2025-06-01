namespace Soccer.Font_end.ViewModels
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
        public decimal BookingRevenue { get; set; }
        public decimal Commission { get; set; }
    }
}
