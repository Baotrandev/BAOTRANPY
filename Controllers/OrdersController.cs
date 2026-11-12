using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class OrdersController : Controller
    {
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        private const string CartKey = "SmartBookCart";

        // form đặt hàng
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = Session[CartKey]
                as List<CartItemViewModel>;

            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            var model = new CheckoutViewModel();

            // login rồi thì tự điền tên
            if (Session["CustomerID"] != null)
            {
                int customerId;

                if (int.TryParse(
                    Session["CustomerID"].ToString(),
                    out customerId))
                {
                    var customer = db.customers.FirstOrDefault(
                        x => x.customer_id == customerId);

                    if (customer != null)
                    {
                        model.FullName = customer.full_name;
                        model.Phone = customer.phone;
                    }
                }
            }

            return View(model);
        }

        // xử lý đặt hàng + lưu db
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var cart = Session[CartKey]
                as List<CartItemViewModel>;

            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
                return View(model);

            // check độ dài cột sql kẻo lỗi
            if (model.FullName.Trim().Length > 150)
            {
                ModelState.AddModelError(
                    "FullName",
                    "Họ tên người nhận không được vượt quá 150 ký tự.");

                return View(model);
            }

            if (model.Address.Trim().Length > 255)
            {
                ModelState.AddModelError(
                    "Address",
                    "Địa chỉ không được vượt quá 255 ký tự.");

                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Note)
                && model.Note.Trim().Length > 500)
            {
                ModelState.AddModelError(
                    "Note",
                    "Ghi chú không được vượt quá 500 ký tự.");

                return View(model);
            }

            using (var transaction =
                db.Database.BeginTransaction())
            {
                try
                {
                    // b1: xem ai đang đặt

                    int customerId = 0;

                    if (Session["CustomerID"] != null)
                    {
                        int.TryParse(
                            Session["CustomerID"].ToString(),
                            out customerId);
                    }

                    var customer = customerId > 0
                        ? db.customers.FirstOrDefault(
                            x => x.customer_id == customerId)
                        : null;

                    // session lỗi thì coi như khách vãng lai
                    if (customer == null)
                    {
                        string guestEmail =
                            "guest_" +
                            Guid.NewGuid().ToString("N") +
                            "@smartbook.local";

                        string guestPassword =
                            SmartBook.PasswordHelper.HashPassword(
                                Guid.NewGuid().ToString("N"));

                        customerId = db.Database.SqlQuery<int>(
                            @"
                            INSERT INTO dbo.customers
                                (full_name, email, password_hash)
                            VALUES
                                (@p0, @p1, @p2);

                            SELECT CAST(SCOPE_IDENTITY() AS int);
                            ",
                            model.FullName.Trim(),
                            guestEmail,
                            guestPassword
                        ).Single();
                    }

                    // b2: tạo địa chỉ giao

                    int addressId = db.Database.SqlQuery<int>(
                        @"
                        INSERT INTO dbo.customer_addresses
                            (
                                customer_id,
                                recipient_name,
                                phone,
                                province,
                                district,
                                street_address,
                                is_default
                            )
                        VALUES
                            (
                                @p0,
                                @p1,
                                @p2,
                                @p3,
                                @p4,
                                @p5,
                                0
                            );

                        SELECT CAST(SCOPE_IDENTITY() AS int);
                        ",
                        customerId,
                        model.FullName.Trim(),
                        model.Phone.Trim(),
                        "Chưa cung cấp",
                        "Chưa cung cấp",
                        model.Address.Trim()
                    ).Single();

                    // b3: check sách + tính tiền

                    decimal subtotal = 0;

                    foreach (var item in cart)
                    {
                        if (item.Quantity <= 0)
                        {
                            throw new Exception(
                                "Số lượng sách trong giỏ hàng không hợp lệ.");
                        }

                        var book = db.books.FirstOrDefault(
                            x => x.book_id == item.BookId);

                        if (book == null)
                        {
                            throw new Exception(
                                "Một cuốn sách trong giỏ hàng không còn tồn tại.");
                        }

                        // lấy giá mới nhất trong db
                        item.UnitPrice = book.sale_price;

                        subtotal +=
                            item.UnitPrice * item.Quantity;
                    }

                    decimal discount = 0;
                    decimal shipping = 0;
                    decimal total = subtotal - discount + shipping;

                    // b4: check tồn kho rồi trừ

                    foreach (var item in cart)
                    {
                        int affectedRows =
                            db.Database.ExecuteSqlCommand(
                                @"
                                UPDATE dbo.inventory
                                SET
                                    quantity_in_stock =
                                        quantity_in_stock - @p0,
                                    last_stock_update = SYSDATETIME()
                                WHERE
                                    book_id = @p1
                                    AND quantity_in_stock
                                        - quantity_reserved >= @p0;
                                ",
                                item.Quantity,
                                item.BookId
                            );

                        if (affectedRows == 0)
                        {
                            throw new Exception(
                                "Sách '" + item.BookTitle +
                                "' không đủ số lượng tồn kho. " +
                                "Vui lòng kiểm tra lại giỏ hàng.");
                        }
                    }

                    // b5: tạo mã đơn

                    string orderCode =
                        "SB" +
                        DateTime.Now.ToString("yyyyMMddHHmmss") +
                        Guid.NewGuid()
                            .ToString("N")
                            .Substring(0, 4)
                            .ToUpper();

                    // b6: lưu đơn

                    int orderId = db.Database.SqlQuery<int>(
                        @"
                        INSERT INTO dbo.orders
                            (
                                customer_id,
                                address_id,
                                order_code,
                                subtotal,
                                discount_amount,
                                shipping_fee,
                                total_amount,
                                customer_note
                            )
                        VALUES
                            (
                                @p0,
                                @p1,
                                @p2,
                                @p3,
                                @p4,
                                @p5,
                                @p6,
                                @p7
                            );

                        SELECT CAST(SCOPE_IDENTITY() AS int);
                        ",
                        customerId,
                        addressId,
                        orderCode,
                        subtotal,
                        discount,
                        shipping,
                        total,
                        string.IsNullOrWhiteSpace(model.Note)
                            ? (object)DBNull.Value
                            : model.Note.Trim()
                    ).Single();

                    // b7: lưu từng món trong đơn

                    foreach (var item in cart)
                    {
                        decimal lineTotal =
                            item.UnitPrice * item.Quantity;

                        db.Database.ExecuteSqlCommand(
                            @"
                            INSERT INTO dbo.order_items
                                (
                                    order_id,
                                    book_id,
                                    quantity,
                                    unit_price,
                                    discount_amount,
                                    line_total
                                )
                            VALUES
                                (
                                    @p0,
                                    @p1,
                                    @p2,
                                    @p3,
                                    @p4,
                                    @p5
                                );
                            ",
                            orderId,
                            item.BookId,
                            item.Quantity,
                            item.UnitPrice,
                            0,
                            lineTotal
                        );
                    }

                    // b8: commit xong

                    transaction.Commit();

                    // lưu ok mới xóa giỏ nha
                    Session.Remove(CartKey);

                    ViewBag.OrderCode = orderCode;
                    ViewBag.Total = total;

                    return View("Success");
                }
                catch (Exception)
                {
                    transaction.Rollback();

                    ModelState.AddModelError(
                        "",
                        "Không thể đặt hàng. Có thể sách đã hết hàng " +
                        "hoặc dữ liệu đơn hàng gặp lỗi. " +
                        "Vui lòng kiểm tra lại và thử lần nữa.");

                    return View(model);
                }
            }
        }


        // Danh sách đơn hàng của tài khoản đang đăng nhập
        public ActionResult MyOrders()
        {
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");

            int customerId;
            if (!int.TryParse(Session["CustomerID"].ToString(), out customerId))
                return RedirectToAction("Login", "Account");

            var orders = db.orders
                .Where(x => x.customer_id == customerId)
                .OrderByDescending(x => x.order_date)
                .ToList();

            return View("MyOrders", orders);
        }

        // Chi tiết đơn hàng - chỉ chủ tài khoản mới được xem
        public ActionResult Details(int id)
        {
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");

            int customerId;
            if (!int.TryParse(Session["CustomerID"].ToString(), out customerId))
                return RedirectToAction("Login", "Account");

            var order = db.orders
                .Include("order_items.book")
                .Include("customer_addresses")
                .FirstOrDefault(x => x.order_id == id && x.customer_id == customerId);

            if (order == null)
                return HttpNotFound();

            return View("Details", order);
        }

        // giải phóng db
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}