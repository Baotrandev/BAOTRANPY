
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class CartController : Controller
    {
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        private const string CartKey = "SmartBookCart";

        private List<CartItemViewModel> GetCart()
        {
            var cart = Session[CartKey]
                as List<CartItemViewModel>;

            if (cart == null)
            {
                cart = new List<CartItemViewModel>();
                Session[CartKey] = cart;
            }

            return cart;
        }

        // HIEN THI GIO HANG
        // Duong dan: /Cart/Index
        public ActionResult Index()
        {
            return View("Index", GetCart());
        }

        // Duong dan: /Cart/giohang
        public ActionResult GioHang()
        {
            return View("Index", GetCart());
        }

        // THEM SACH VAO GIO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int id, int quantity = 1)
        {
            if (quantity < 1)
                quantity = 1;

            var book = db.books
                .FirstOrDefault(x => x.book_id == id);

            if (book == null)
                return HttpNotFound();

            // LAY TEN TAC GIA
            var authorName = (
                from ba in db.book_authors
                join a in db.authors
                    on ba.author_id equals a.author_id
                where ba.book_id == id
                select a.author_name
            ).ToList();

            string authors = string.Join(", ", authorName);

            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.BookId == id);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    BookId = book.book_id,
                    BookTitle = book.book_title,
                    AuthorName = authors,
                    ImageUrl = book.cover_image_url,
                    UnitPrice = book.sale_price,
                    Quantity = quantity
                });
            }

            Session[CartKey] = cart;

            TempData["CartMessage"] =
                "Đã thêm sách vào giỏ hàng!";

            return RedirectToAction("Index", "Cart");
        }
        // CAP NHAT SO LUONG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, int quantity)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.BookId == id);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    var book = db.books.FirstOrDefault(
                        x => x.book_id == id);

                    if (book == null)
                    {
                        cart.Remove(item);
                    }
                    else
                    {
                        item.Quantity = quantity;
                    }
                }
            }

            Session[CartKey] = cart;

            return RedirectToAction("Index");
        }

        // XOA SACH
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.BookId == id);

            if (item != null)
                cart.Remove(item);

            Session[CartKey] = cart;

            return RedirectToAction("Index");
        }
    }
}
