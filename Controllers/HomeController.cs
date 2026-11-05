
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class HomeController : Controller
    {
        // Trang liên hệ
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        // Xử lý biểu mẫu liên hệ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(
            string fullName,
            string email,
            string subject,
            string message)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(message))
            {
                ViewBag.ContactError = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            if (!email.Contains("@"))
            {
                ViewBag.ContactError = "Địa chỉ email chưa hợp lệ.";
                return View();
            }

            TempData["ContactSuccess"] =
                "Biểu mẫu đã được tiếp nhận thành công!";

            return RedirectToAction("Contact");
        }
    
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        public ActionResult Index()
        {
            var model = new HomeViewModel();

            var categories = db.categories
                .Where(c => c.is_active)
                .OrderBy(c => c.category_id)
                .ToList();

            var books = db.books
                .Include("category")
                .Include("publisher")
                .Where(b => b.status == "Đang bán")
                .ToList();

            foreach (var category in categories)
            {
                model.Sections.Add(
                    new CategorySectionViewModel
                    {
                        Category = category,
                        Books = books
                            .Where(b => b.category_id == category.category_id)
                            .OrderBy(b => b.book_title)
                            .ToList()
                    });
            }

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
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
