namespace Soccer.Font_end.ViewModels
{
    public class ShopIndexViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<ColorViewModel> Colors { get; set; } = new List<ColorViewModel>();
        public List<SizeViewModel> Sizes { get; set; } = new List<SizeViewModel>();

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }

        // Thêm các thuộc tính để giữ trạng thái filter
        public string CurrentSort { get; set; }
        public int? CurrentColorId { get; set; }
        public int? CurrentSizeId { get; set; }
        public int? CurrentCategoryId { get; set; }
        public int? CurrentBrandId { get; set; }
        public decimal? CurrentMinPrice { get; set; }
        public decimal? CurrentMaxPrice { get; set; }
        public string CurrentSearch { get; set; }

        // Computed properties
        public int TotalPages => TotalItems > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 1;
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public List<int> PageSizeOptions { get; set; } = new List<int> { 6, 12, 24, 48 };
    }
}
