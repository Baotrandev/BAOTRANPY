
using System;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using SmartBook.Models;
using SmartBook.ViewModels;

namespace SmartBook.Controllers
{
    public class StoreController : Controller
    {
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        public ActionResult Index(
            int? categoryId,
            string keyword,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1,
            string sortOrder = "")
        {
            const int pageSize = 6;

            if (page < 1)
                page = 1;

            // Lấy sách từ database.
            var products = db.books
                .Include(b => b.category)
                .AsQueryable();

            // Lọc theo thể loại.
            if (categoryId.HasValue)
            {
                products = products.Where(
                    b => b.category_id == categoryId.Value);
            }

            // Tìm kiếm tên sách và mô tả.
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                products = products.Where(b =>
                    b.book_title.Contains(keyword) ||
                    (b.description != null &&
                     b.description.Contains(keyword)));
            }

            // Lọc giá bán.
            if (minPrice.HasValue)
            {
                products = products.Where(
                    b => b.sale_price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(
                    b => b.sale_price <= maxPrice.Value);
            }

            // Tổng số sản phẩm.
            int totalItems = products.Count();

            int totalPages = (int)Math.Ceiling(
                (double)totalItems / pageSize);

            if (totalPages < 1)
                totalPages = 1;

            if (page > totalPages)
                page = totalPages;

            // Sắp xếp.
            switch ((sortOrder ?? "").ToLower())
            {
                case "price_asc":
                    products = products
                        .OrderBy(b => b.sale_price);
                    break;

                case "price_desc":
                    products = products
                        .OrderByDescending(b => b.sale_price);
                    break;

                default:
                    products = products
                        .OrderByDescending(b => b.book_id);
                    sortOrder = "";
                    break;
            }

            // Tạo dữ liệu hiển thị.
            var model = new StoreHomeViewModel
            {
                Categories = db.categories
                    .OrderBy(c => c.category_name)
                    .ToList(),

                Products = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new ProductCardViewModel
                    {
                        ProductID = b.book_id,
                        NamePro = b.book_title,
                        CategoryName = b.category.category_name,

                        // Giá gốc và giá khuyến mãi.
                        OriginalPrice = b.original_price,
                        Price = b.sale_price,

                        // Ảnh có sẵn trong database.
                        ImagePro = b.cover_image_url,

                        ShortDescription = b.description

                    }).ToList(),

                SelectedCategoryId = categoryId,
                SearchTerm = keyword,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                SortOrder = sortOrder
            };

            ViewBag.Title = "Tất cả sách";

            return View(model);
        }

        public ActionResult AboutStore()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
