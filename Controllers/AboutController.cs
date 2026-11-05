using System.Linq;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class AboutController : Controller
    {
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        public ActionResult Index()
        {
            // Ưu tiên hiển thị những cuốn sách đặc sắc
            string[] featuredTitles =
            {
                "Mắt biếc",
                "Đắc Nhân Tâm",
                "Hoàng Tử Bé",
                "Big Data - Dữ Liệu Lớn"
            };

            var featuredBooks = db.books
                .Where(b => featuredTitles.Contains(b.book_title)
                         && b.status == "Đang bán")
                .Take(4)
                .ToList();

            // Nếu tên sách không khớp dữ liệu thì lấy 4 sách đang bán
            if (!featuredBooks.Any())
            {
                featuredBooks = db.books
                    .Where(b => b.status == "Đang bán")
                    .OrderBy(b => b.book_id)
                    .Take(4)
                    .ToList();
            }

            return View(featuredBooks);
        }
    }
}