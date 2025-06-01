namespace Soccer.Font_end.ViewModels
{
    public class HomeViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public List<FieldSearchResultViewModel> FeaturedFields { get; set; } = new();
    }
}
