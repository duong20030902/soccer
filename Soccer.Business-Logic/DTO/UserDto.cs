using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class UserDto
    {
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserRequest
    {
        [Required]
        public int RoleID { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UpdateUserRequest
    {
        [Required]
        public int RoleID { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
    }
}
