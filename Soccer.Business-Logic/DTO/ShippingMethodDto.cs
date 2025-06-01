using System.ComponentModel.DataAnnotations;

namespace Soccer.Business_Logic.DTO
{
    public class ShippingMethodDto
    {
        public int ShippingMethodID { get; set; }
        public string MethodName { get; set; }
        public string DeliveryTime { get; set; }
    }

    public class CreateShippingMethodRequest
    {
        [Required]
        public string MethodName { get; set; }
        public string DeliveryTime { get; set; }
    }

    public class UpdateShippingMethodRequest
    {
        [Required]
        public string MethodName { get; set; }
        public string DeliveryTime { get; set; }
    }
}
