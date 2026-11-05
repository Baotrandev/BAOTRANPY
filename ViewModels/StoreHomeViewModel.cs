
using System.Collections.Generic;
using SmartBook.Models;

namespace SmartBook.ViewModels
{
    public class StoreHomeViewModel
    {
        public IEnumerable<category> Categories { get; set; }

        public List<ProductCardViewModel> Products { get; set; }

        public int? SelectedCategoryId { get; set; }

        public string SearchTerm { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public string SortOrder { get; set; }
    }

    public class ProductCardViewModel
    {
        public int ProductID { get; set; }

        public string NamePro { get; set; }

        public string CategoryName { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal Price { get; set; }

        public string ImagePro { get; set; }

        public string ShortDescription { get; set; }

        public int StockQuantity { get; set; }
    }
}
