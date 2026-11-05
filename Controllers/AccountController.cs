using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using SmartBook.Models;
using SmartBook.ViewModels;

namespace SmartBook.Controllers
{
    public class AccountController : Controller
    {
        private bookstore_dbEntities db =
            new bookstore_dbEntities();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string email = model.email.Trim().ToLower();

            bool exists = db.customers.Any(
                c => c.email == email);

            if (exists)
            {
                ModelState.AddModelError(
                    "email",
                    "Email này đã được đăng ký.");

                return View(model);
            }

            customer newCustomer = new customer
            {
                full_name = model.full_name.Trim(),
                email = email,
                password_hash =
                    PasswordHelper.HashPassword(model.password),
                phone = model.phone,
                status = "Hoat dong",
                created_at = DateTime.Now
            };

            db.customers.Add(newCustomer);
            db.SaveChanges();

            TempData["Success"] =
                "Đăng ký thành công! Hãy đăng nhập.";

            return RedirectToAction("Login");
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string email = model.email.Trim().ToLower();

            var user = db.customers.FirstOrDefault(
                c => c.email == email &&
                     c.status == "Hoat dong");

            if (user == null ||
                !PasswordHelper.VerifyPassword(
                    model.password, user.password_hash))
            {
                ModelState.AddModelError(
                    "",
                    "Email hoặc mật khẩu không chính xác.");

                return View(model);
            }

            FormsAuthentication.SetAuthCookie(
                user.customer_id.ToString(), false);

            Session["CustomerID"] = user.customer_id;
            Session["CustomerName"] = user.full_name;

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout
        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Index", "Home");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}

