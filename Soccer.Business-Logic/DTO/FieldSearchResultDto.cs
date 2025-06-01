using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class FieldSearchResultDto
    {
        public int ScheduleId { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = null!;
        public List<string> ImageUrls { get; set; } = new();
    }

    public class FieldSearchRequest
    {
        [Required(ErrorMessage = "Ngày là bắt buộc")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Khung giờ là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Khung giờ phải lớn hơn 0")]
        public int TimeslotId { get; set; }

        public string? FieldName { get; set; }
    }
}
