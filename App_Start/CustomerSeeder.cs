
using System;
using System.Linq;
using SmartBook.Models;

namespace SmartBook
{
    public static class CustomerSeeder
    {
        public static void Seed()
        {
            using (var db = new bookstore_dbEntities())
            {
                var customers = new[]
                {
                    new
                    {
                        FullName = "Nguyễn Minh Anh",
                        Email = "minhanh@gmail.com",
                        Password = "SmartBook@123",
                        Phone = "0900000001",
                        Gender = "Nữ",
                        BirthDate = new DateTime(2004, 5, 12)
                    },
                    new
                    {
                        FullName = "Trần Hoàng Nam",
                        Email = "hoangnam@gmail.com",
                        Password = "SmartBook@123",
                        Phone = "0900000002",
                        Gender = "Nam",
                        BirthDate = new DateTime(2003, 8, 20)
                    },
                    new
                    {
                        FullName = "Lê Ngọc Hà",
                        Email = "ngocha@gmail.com",
                        Password = "SmartBook@123",
                        Phone = "0900000003",
                        Gender = "Nữ",
                        BirthDate = new DateTime(2005, 2, 15)
                    },
                    new
                    {
                        FullName = "Phạm Gia Bảo",
                        Email = "giabao@gmail.com",
                        Password = "SmartBook@123",
                        Phone = "0900000004",
                        Gender = "Nam",
                        BirthDate = new DateTime(2002, 11, 9)
                    },
                    new
                    {
                        FullName = "Võ Thanh Thảo",
                        Email = "thanhthao@gmail.com",
                        Password = "SmartBook@123",
                        Phone = "0900000005",
                        Gender = "Nữ",
                        BirthDate = new DateTime(2004, 7, 25)
                    }
                };

                foreach (var item in customers)
                {
                    bool exists = db.customers.Any(
                        x => x.email == item.Email
                    );

                    if (exists)
                        continue;

                    var customer = new customer
                    {
                        full_name = item.FullName,
                        email = item.Email,

                        // Ma hoa mat khau bang PasswordHelper
                        password_hash =
                            PasswordHelper.HashPassword(item.Password),

                        phone = item.Phone,
                        gender = item.Gender,
                        date_of_birth = item.BirthDate,
                        status = "Hoat dong",
                        created_at = DateTime.Now
                    };

                    db.customers.Add(customer);
                }

                db.SaveChanges();
            }
        }
    }
}
