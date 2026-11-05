
using SmartBook.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace SmartBook.Controllers
{
    public class BooksController : Controller
    {
        private bookstore_dbEntities db = new bookstore_dbEntities();

        // DANH SACH SACH + TIM KIEM + LOC
        public ActionResult Index(
            string search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = db.books
                .Include(b => b.category)
                .AsQueryable();

            // Tim theo ten sach hoac ten tac gia
            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();

                var bookIds = db.book_authors
                    .Where(ba => db.authors.Any(a =>
                        a.author_id == ba.author_id &&
                        a.author_name.Contains(keyword)))
                    .Select(ba => ba.book_id);

                query = query.Where(b =>
                    b.book_title.Contains(keyword) ||
                    bookIds.Contains(b.book_id));
            }

            // Loc theo danh muc
            if (categoryId.HasValue)
            {
                query = query.Where(b =>
                    b.category_id == categoryId.Value);
            }

            // Loc gia tu
            if (minPrice.HasValue)
            {
                query = query.Where(b =>
                    b.sale_price >= minPrice.Value);
            }

            // Loc gia den
            if (maxPrice.HasValue)
            {
                query = query.Where(b =>
                    b.sale_price <= maxPrice.Value);
            }

            ViewBag.Search = search;
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", categoryId);

            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(query.OrderBy(b => b.book_title).ToList());
        }

        // CHI TIET SACH
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            book product = db.books
                .Include("category")
                .Include("publisher")
                .FirstOrDefault(x => x.book_id == id.Value);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // HIEN FORM THEM SACH
        public ActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // LUU SACH MOI
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(book book)
        {
            if (ModelState.IsValid)
            {
                book.created_at = DateTime.Now;
                book.updated_at = null;

                db.books.Add(book);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadDropdowns(book.category_id, book.publisher_id);
            return View(book);
        }

        // HIEN FORM SUA SACH
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            book book = db.books.Find(id);

            if (book == null)
                return HttpNotFound();

            LoadDropdowns(book.category_id, book.publisher_id);
            return View(book);
        }

        // CAP NHAT SACH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(book model)
        {
            if (ModelState.IsValid)
            {
                var book = db.books.Find(model.book_id);

                if (book == null)
                    return HttpNotFound();

                book.category_id = model.category_id;
                book.publisher_id = model.publisher_id;
                book.ISBN = model.ISBN;
                book.book_title = model.book_title;
                book.description = model.description;
                book.publication_date = model.publication_date;
                book.page_count = model.page_count;
                book.language = model.language;
                book.book_format = model.book_format;
                book.cover_image_url = model.cover_image_url;
                book.original_price = model.original_price;
                book.sale_price = model.sale_price;
                book.status = model.status;
                book.updated_at = DateTime.Now;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            LoadDropdowns(model.category_id, model.publisher_id);
            return View(model);
        }

        // XEM THONG TIN TRUOC KHI XOA
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var book = db.books
                .Include(b => b.category)
                .FirstOrDefault(b => b.book_id == id);

            if (book == null)
                return HttpNotFound();

            return View(book);
        }

        // XOA SACH
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var book = db.books.Find(id);

            if (book == null)
                return HttpNotFound();

            // Khong xoa sach da phat sinh giao dich
            bool hasOrder = db.order_items.Any(x => x.book_id == id);
            bool hasCart = db.cart_items.Any(x => x.book_id == id);
            bool hasReview = db.reviews.Any(x => x.book_id == id);

            if (hasOrder || hasCart || hasReview)
            {
                TempData["Error"] =
                    "Không thể xóa sách đã có trong giỏ hàng, đơn hàng hoặc đánh giá.";

                return RedirectToAction("Index");
            }

            // Xoa du lieu lien ket truoc
            var links = db.book_authors.Where(x => x.book_id == id);
            db.book_authors.RemoveRange(links);

            // Xóa dữ liệu tồn kho liên kết với sách
            var inventoryItem = db.inventories
                .FirstOrDefault(x => x.book_id == id);

            if (inventoryItem != null)
            {
                db.inventories.Remove(inventoryItem);
            }

            db.books.Remove(book);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // NAP DANH MUC VA NHA XUAT BAN
        private void LoadDropdowns(
            int? categoryId = null,
            int? publisherId = null)
        {
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", categoryId);

            ViewBag.publisher_id = new SelectList(
                db.publishers,
                "publisher_id",
                "publisher_name",
                publisherId
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
