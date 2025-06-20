namespace Soccer.Font_end.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public string RoleName { get; set; }
        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public int AddressId { get; set; }
        public string RecipientName { get; set; }
        public string StreetAddress { get; set; }
        public string CityProvince { get; set; }
        public string PostalCode { get; set; }
    }
}