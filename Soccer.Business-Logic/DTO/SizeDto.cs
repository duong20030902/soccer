using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class SizeDto
    {
        public int SizeID { get; set; }
        public string SizeName { get; set; }
        public int SizeOrder { get; set; }
    }

    public class CreateSizeRequest
    {
        [Required]
        public string SizeName { get; set; }
        [Required]
        public int SizeOrder { get; set; }
    }

    public class UpdateSizeRequest
    {
        [Required]
        public string SizeName { get; set; }
        [Required]
        public int SizeOrder { get; set; }
    }
}
