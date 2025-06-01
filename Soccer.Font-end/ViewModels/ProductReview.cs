namespace Soccer.Font_end.ViewModels
{
    public class ProductReview
    {
        public int ReviewID { get; set; }
        public byte Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public ReviewUser User { get; set; }
    }
}
