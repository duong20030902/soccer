namespace Soccer.Font_end.ViewModels
{
    public class FieldSearchResultViewModel
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
        public bool IsAvailable => Status.Equals("Available", StringComparison.OrdinalIgnoreCase);
        // Properties for display
        public string PriceDisplay => $"{PricePerHour:N0} VNĐ/giờ";
        public string TimeDisplay => $"{StartTime:HH:mm} - {EndTime:HH:mm}";
        public string FirstImageUrl => ImageUrls.FirstOrDefault() ?? "/images/default-field.jpg";
        public int TimeslotId { get; set; }
    }

    public class TimeslotViewModel
    {
        public int TimeslotID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string TimeDisplay => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
    }

    public class FieldSearchRequest
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int TimeslotId { get; set; }
        public string? FieldName { get; set; }
    }
}