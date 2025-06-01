using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    // Base DTO - thống nhất tên thuộc tính
    public class BookingDto
    {
        public int BookingId { get; set; } // Thống nhất với BookingResponseDto
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

    // Request model cho tạo booking
    public class CreateBookingRequest
    {
        [Required(ErrorMessage = "ScheduleId là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ScheduleId phải lớn hơn 0")]
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "UserId là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId phải lớn hơn 0")]
        public int UserId { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }
    }

    // Response model - kế thừa từ BookingDto để tránh trùng lặp
    public class BookingResponseDto : BookingDto
    {
        // Có thể thêm các thuộc tính khác nếu cần
    }

    // Request model cho update status
    public class UpdateStatusRequest
    {
        [Required(ErrorMessage = "Status là bắt buộc")]
        public string Status { get; set; } = string.Empty;
    }

    // Thêm enum cho Status để type-safe hơn
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
