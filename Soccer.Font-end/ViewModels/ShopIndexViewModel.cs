namespace Soccer.Font_end.ViewModels
{
    public class ShopIndexViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<ColorViewModel> Colors { get; set; } = new List<ColorViewModel>();
        public List<SizeViewModel> Sizes { get; set; } = new List<SizeViewModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}
