namespace Soccer.Font_end.ViewModels
{
    public class FieldBookingViewModel
    {
        public List<FieldSearchResultViewModel> Fields { get; set; } = new();
        public List<TimeslotViewModel> Timeslots { get; set; } = new();
        public DateOnly SelectedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int? SelectedTimeslotId { get; set; }
        public string? SearchFieldName { get; set; }
        public string? ErrorMessage { get; set; }
        public int TotalCount => Fields.Count;
    }
}
