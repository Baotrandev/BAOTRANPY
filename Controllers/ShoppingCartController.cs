
using System.Linq;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class ShoppingCartController : Controller
    {
        // Kết nối database SmartBook
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        // Lấy giỏ hàng từ Session
        private Cart GetCart()
        {
            var cart = Session["Cart"] as Cart;

            if (cart == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }

            return cart;
        }

        // Hiển thị giỏ hàng
        public ActionResult ShowCart()
        {
            return View(GetCart());
        }

        // Thêm sách vào giỏ hàng
        public ActionResult AddToCart(int id)
        {
            // Lấy sách trực tiếp từ database
            var book = db.books.FirstOrDefault(
                b => b.book_id == id);

            if (book == null)
            {
                TempData["CartError"] =
                    "Sản phẩm không tồn tại.";

                return RedirectToAction("Index", "Product");
            }

            // Thêm sách vào giỏ
            GetCart().Add(book);

            TempData["CartMessage"] =
                "Đã thêm sản phẩm vào giỏ hàng.";

            return RedirectToAction("ShowCart");
        }

        // Cập nhật số lượng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(
            int productId, int quantity)
        {
            var book = db.books.Find(productId);

            if (book == null)
            {
                TempData["CartError"] =
                    "Không tìm thấy sản phẩm.";

                return RedirectToAction("ShowCart");
            }

            bool updated = GetCart().Update(
                productId, quantity);

            if (!updated)
            {
                TempData["CartError"] =
                    "Số lượng phải lớn hơn 0.";
            }

            return RedirectToAction("ShowCart");
        }

        // Xóa một sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int productId)
        {
            GetCart().Remove(productId);

            return RedirectToAction("ShowCart");
        }

        // Xóa toàn bộ giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart()
        {
            GetCart().Clear();

            return RedirectToAction("ShowCart");
        }

        // Hiển thị số lượng giỏ hàng trên menu
        [ChildActionOnly]
        public PartialViewResult BagCart()
        {
            var cart = Session["Cart"] as Cart;

            ViewBag.CartQty = cart == null
                ? 0 : cart.TotalQuantity();

            ViewBag.CartTotal = cart == null
                ? 0 : cart.TotalMoney();

            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
