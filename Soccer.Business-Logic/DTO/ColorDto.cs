using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class ColorDto
    {
        public int ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }

    public class CreateColorRequest
    {
        [Required]
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }

    public class UpdateColorRequest
    {
        [Required]
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
    }
}
