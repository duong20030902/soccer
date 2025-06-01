using System.ComponentModel.DataAnnotations;

namespace Soccer.Font_end.ViewModels
{
    public class TimeslotDto
    {
        public int TimeslotID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class CreateTimeslotRequest
    {
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
    }

    public class UpdateTimeslotRequest
    {
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
    }
}
