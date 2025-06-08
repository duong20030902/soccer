namespace Soccer.Font_end.ViewModels
{
    public class BookingDetailViewModel
    {
        public int BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string TotalAmountDisplay => $"{TotalAmount:N0} VNĐ";

        // Field Information
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string FieldLocation { get; set; } = string.Empty;

        // Booking Information
        public DateOnly BookingDate { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int Duration { get; set; }

        // Customer Information
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string? Note { get; set; }

        // Payment Information
        public decimal FieldPrice { get; set; }
        public string FieldPriceDisplay => $"{FieldPrice:N0} VNĐ";
        public decimal DiscountAmount { get; set; }
        public string DiscountAmountDisplay => $"{DiscountAmount:N0} VNĐ";

        // Action Permissions
        public bool CanCancel { get; set; }
        public bool CanModify { get; set; }

        // Timeline
        public List<BookingTimelineItem> Timeline { get; set; } = new();
    }

    public class BookingTimelineItem
    {
        public DateTime Time { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
