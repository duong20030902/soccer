using System.ComponentModel.DataAnnotations;

namespace Soccer.Font_end.ViewModels
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

    public class CreateSizeViewModel
    {
        [Required(ErrorMessage = "Tên kích thước là bắt buộc")]
        [Display(Name = "Tên kích thước")]
        [StringLength(50, ErrorMessage = "Tên kích thước không được vượt quá 50 ký tự")]
        public string SizeName { get; set; }

        [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc")]
        [Display(Name = "Thứ tự hiển thị")]
        [Range(1, 999, ErrorMessage = "Thứ tự phải từ 1 đến 999")]
        public int SizeOrder { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Description { get; set; }
    }

    public class EditSizeViewModel
    {
        public int SizeID { get; set; }

        [Required(ErrorMessage = "Tên kích thước là bắt buộc")]
        [Display(Name = "Tên kích thước")]
        [StringLength(50, ErrorMessage = "Tên kích thước không được vượt quá 50 ký tự")]
        public string SizeName { get; set; }

        [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc")]
        [Display(Name = "Thứ tự hiển thị")]
        [Range(1, 999, ErrorMessage = "Thứ tự phải từ 1 đến 999")]
        public int SizeOrder { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(200, ErrorMessage = "Mô tả không được vượt quá 200 ký tự")]
        public string? Description { get; set; }
    }
}
