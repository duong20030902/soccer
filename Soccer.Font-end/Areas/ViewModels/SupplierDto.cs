using System.ComponentModel.DataAnnotations;

namespace Soccer.Font_end.ViewModels
{
    public class SupplierDto
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string ContactInfo { get; set; }
    }

    public class CreateSupplierRequest
    {
        [Required]
        public string SupplierName { get; set; }
        public string ContactInfo { get; set; }
    }

    public class UpdateSupplierRequest
    {
        [Required]
        public string SupplierName { get; set; }
        public string ContactInfo { get; set; }
    }

    public class CreateSupplierViewModel
    {
        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
        [Display(Name = "Tên nhà cung cấp")]
        [StringLength(200, ErrorMessage = "Tên nhà cung cấp không được vượt quá 200 ký tự")]
        public string SupplierName { get; set; }

        [Display(Name = "Thông tin liên hệ")]
        [StringLength(500, ErrorMessage = "Thông tin liên hệ không được vượt quá 500 ký tự")]
        public string? ContactInfo { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")]
        public string? Address { get; set; }
    }

    public class EditSupplierViewModel
    {
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
        [Display(Name = "Tên nhà cung cấp")]
        [StringLength(200, ErrorMessage = "Tên nhà cung cấp không được vượt quá 200 ký tự")]
        public string SupplierName { get; set; }

        [Display(Name = "Thông tin liên hệ")]
        [StringLength(500, ErrorMessage = "Thông tin liên hệ không được vượt quá 500 ký tự")]
        public string? ContactInfo { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")]
        public string? Address { get; set; }
    }
}
