using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SmartBook.Models;

namespace SmartBook.Controllers
{
    public class OrdersController : Controller
    {
        private readonly bookstore_dbEntities db =
            new bookstore_dbEntities();

        private const string CartKey = "SmartBookCart";

        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = Session[CartKey]
                as List<CartItemViewModel>;

            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            var model = new CheckoutViewModel
            {
                PaymentMethod = "COD"
            };

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

            ViewBag.CartItems = cart;
            ViewBag.CartTotal = cart.Sum(x => x.LineTotal);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var cart = Session[CartKey]
                as List<CartItemViewModel>;

            if (cart == null || !cart.Any())
                return RedirectToAction("Index", "Cart");

            ViewBag.CartItems = cart;
            ViewBag.CartTotal = cart.Sum(x => x.LineTotal);

            if (!ModelState.IsValid)
                return View(model);

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

            // check xem khách chọn thanh toán kiểu nào để validate đúng form đó
            if (model.PaymentMethod == "OnlineBanking")
            {
                if (string.IsNullOrWhiteSpace(model.BankingProvider))
                {
                    ModelState.AddModelError(
                        "BankingProvider",
                        "Vui lòng chọn ví điện tử hoặc cổng thanh toán.");
                    return View(model);
                }
            }
            else if (model.PaymentMethod == "Card")
            {
                if (string.IsNullOrWhiteSpace(model.CardType))
                {
                    ModelState.AddModelError(
                        "CardType",
                        "Vui lòng chọn loại thẻ.");
                    return View(model);
                }

                string cleanCard = string.IsNullOrWhiteSpace(model.CardNumber)
                    ? ""
                    : model.CardNumber.Replace(" ", "").Replace("-", "");

                if (!Regex.IsMatch(cleanCard, @"^[0-9]{12,19}$"))
                {
                    ModelState.AddModelError(
                        "CardNumber",
                        "Số thẻ không hợp lệ (cần từ 12 đến 19 chữ số).");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.CardHolder))
                {
                    ModelState.AddModelError(
                        "CardHolder",
                        "Vui lòng nhập tên chủ thẻ.");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.CardExpiry)
                    || !Regex.IsMatch(model.CardExpiry.Trim(), @"^(0[1-9]|1[0-2])\/[0-9]{2}$"))
                {
                    ModelState.AddModelError(
                        "CardExpiry",
                        "Ngày hết hạn không đúng định dạng MM/YY.");
                    return View(model);
                }

                if (string.IsNullOrWhiteSpace(model.CardCvv)
                    || !Regex.IsMatch(model.CardCvv.Trim(), @"^[0-9]{3,4}$"))
                {
                    ModelState.AddModelError(
                        "CardCvv",
                        "Mã CVV phải gồm 3 hoặc 4 chữ số.");
                    return View(model);
                }
            }
            else if (model.PaymentMethod != "COD")
            {
                ModelState.AddModelError(
                    "PaymentMethod",
                    "Phương thức thanh toán không hợp lệ.");
                return View(model);
            }

            using (var transaction =
                db.Database.BeginTransaction())
            {
                try
                {
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

                        item.UnitPrice = book.sale_price;

                        subtotal +=
                            item.UnitPrice * item.Quantity;
                    }

                    decimal discount = 0;
                    decimal shipping = 0;
                    decimal total = subtotal - discount + shipping;

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

                    string orderCode =
                        "SB" +
                        DateTime.Now.ToString("yyyyMMddHHmmss") +
                        Guid.NewGuid()
                            .ToString("N")
                            .Substring(0, 4)
                            .ToUpper();

                    string paymentMethodLabel = "COD";
                    string transactionCode = null;
                    string paymentStatus = "Chưa thanh toán";
                    object paymentDate = DBNull.Value;

                    if (model.PaymentMethod == "OnlineBanking")
                    {
                        paymentMethodLabel = "Online - " + model.BankingProvider.Trim();
                        transactionCode = "ONL" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        paymentStatus = "Chờ xác nhận";
                        paymentDate = DateTime.Now;
                    }
                    else if (model.PaymentMethod == "Card")
                    {
                        paymentMethodLabel = "Thẻ - " + model.CardType.Trim();
                        transactionCode = "CARD" + DateTime.Now.ToString("yyyyMMddHHmmss");
                        paymentStatus = "Đã thanh toán";
                        paymentDate = DateTime.Now;
                    }

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
                                payment_status,
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
                                @p7,
                                @p8
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
                        paymentStatus,
                        string.IsNullOrWhiteSpace(model.Note)
                            ? (object)DBNull.Value
                            : model.Note.Trim()
                    ).Single();

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

                    db.Database.ExecuteSqlCommand(
                        @"
                        INSERT INTO dbo.payments
                            (
                                order_id,
                                payment_method,
                                transaction_code,
                                amount,
                                payment_status,
                                payment_date,
                                created_at
                            )
                        VALUES
                            (
                                @p0,
                                @p1,
                                @p2,
                                @p3,
                                @p4,
                                @p5,
                                SYSDATETIME()
                            );
                        ",
                        orderId,
                        paymentMethodLabel,
                        string.IsNullOrEmpty(transactionCode)
                            ? (object)DBNull.Value
                            : transactionCode,
                        total,
                        paymentStatus,
                        paymentDate
                    );

                    transaction.Commit();

                    Session.Remove(CartKey);

                    ViewBag.OrderCode = orderCode;
                    ViewBag.Total = total;
                    ViewBag.PaymentMethod = paymentMethodLabel;
                    ViewBag.PaymentStatus = paymentStatus;

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

        [HttpGet]
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

            return View(orders);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (Session["CustomerID"] == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return RedirectToAction("MyOrders");

            int customerId;
            if (!int.TryParse(Session["CustomerID"].ToString(), out customerId))
                return RedirectToAction("Login", "Account");

            var order = db.orders
                .Include(x => x.customer_addresses)
                .Include(x => x.order_items.Select(i => i.book))
                .Include(x => x.payments)
                .FirstOrDefault(x => x.order_id == id.Value && x.customer_id == customerId);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}