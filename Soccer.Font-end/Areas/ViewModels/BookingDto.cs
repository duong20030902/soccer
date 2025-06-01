using System.ComponentModel.DataAnnotations;

namespace Soccer.Font_end.Areas.ViewModels
{
    public class BookingDto
    {
        public int BookingId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingTime { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateBookingRequest
    {
        public int ScheduleId { get; set; }
        public int UserId { get; set; }
        public string? Notes { get; set; }
    }

    public class PaginatedBookingResponse
    {
        public List<BookingDto> Data { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
    }

    public class PaginationInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class BookingStatistics
    {
        public List<StatusStat> StatusStats { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class StatusStat
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class UpdateStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
