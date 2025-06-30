namespace Soccer.Business_Logic.DTO
{
    public class CreateQuickBookingRequest
    {
        public int FieldId { get; set; }
        public int TimeslotId { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public string? Notes { get; set; }
    }

}
