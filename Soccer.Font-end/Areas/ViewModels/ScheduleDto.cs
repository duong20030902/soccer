namespace Soccer.Font_end.Areas.ViewModels
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int TimeslotId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
