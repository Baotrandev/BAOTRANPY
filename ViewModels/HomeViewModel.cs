using System.Collections.Generic;

namespace SmartBook.Models
{
    public class HomeViewModel
    {
        public List<CategorySectionViewModel> Sections { get; set; }

        public HomeViewModel()
        {
            Sections = new List<CategorySectionViewModel>();
        }
    }

    public class CategorySectionViewModel
    {
        public category Category { get; set; }

        public List<book> Books { get; set; }

        public CategorySectionViewModel()
        {
            Books = new List<book>();
        }
    }
}