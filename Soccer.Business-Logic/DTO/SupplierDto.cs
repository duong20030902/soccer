using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
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
}
