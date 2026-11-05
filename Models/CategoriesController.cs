using System;
using System.Collections.Generic;
using System.Web.Mvc;
namespace SmartBook.Models
{
    

    public class CategoriesController : Controller
    {
        public int category_id { get; set; }

        public string category_name { get; set; }

        public string description { get; set; }

        public Nullable<int> parent_category_id { get; set; }

        public bool is_active { get; set; }

        public System.DateTime created_at { get; set; }

        public virtual ICollection<book> books { get; set; }

        public virtual ICollection<category> categories1 { get; set; }

        public virtual category category1 { get; set; }
    }
}