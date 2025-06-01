using System.ComponentModel.DataAnnotations;

namespace Soccer.Font_end.Areas.ViewModels
{
    public class FieldDto
    {
        public int FieldID { get; set; }
        public string FieldName { get; set; }
        public string Location { get; set; }
        public decimal PricePerHour { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }

    public class CreateFieldRequest
    {
        [Required]
        public string FieldName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public decimal PricePerHour { get; set; }
        public List<string> ImageUrls { get; set; }
    }

    public class UpdateFieldRequest
    {
        [Required]
        public string FieldName { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public decimal PricePerHour { get; set; }
    }
}
