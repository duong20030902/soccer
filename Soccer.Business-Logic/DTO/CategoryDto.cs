using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class CategoryDto
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ParentID { get; set; }
        public string ParentName { get; set; }
    }

    public class CreateCategoryRequest
    {
        [Required]
        public string CategoryName { get; set; }
        public int? ParentID { get; set; }
    }

    public class UpdateCategoryRequest
    {
        [Required]
        public string CategoryName { get; set; }
        public int? ParentID { get; set; }
    }
}
