using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Soccer.Font_end.ViewModels
{
    public class ProductDetailViewModel
    {
        public ProductInfo ProductInfo { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<ProductReview> Reviews { get; set; }
        public RatingInfo RatingInfo { get; set; }
        public List<RelatedProduct> RelatedProducts { get; set; }
    }
}
