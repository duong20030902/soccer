using System.ComponentModel.DataAnnotations;

namespace Soccer.Font_end.Areas.ViewModels
{
    public class RoleDto
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
        [Display(Name = "Tên vai trò")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
        public string RoleName { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
    }

    public class EditRoleViewModel
    {
        public int RoleID { get; set; }

        [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
        [Display(Name = "Tên vai trò")]
        [StringLength(100, ErrorMessage = "Tên vai trò không được vượt quá 100 ký tự")]
        public string RoleName { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }
    }
}
