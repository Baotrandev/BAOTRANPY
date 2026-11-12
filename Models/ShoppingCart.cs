
using System.Collections.Generic;
using System.Linq;

namespace SmartBook.Models
{
    [System.Serializable]
    public class Cart
    {
        public List<CartItem> Items { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
        }

        // thêm sách vào giỏ
        public bool Add(book b)
        {
            if (b == null)
                return false;

            var item = Items.FirstOrDefault(
                x => x.BookID == b.book_id);

            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                Items.Add(new CartItem
                {
                    BookID = b.book_id,
                    BookTitle = b.book_title,
                    Image = b.cover_image_url,
                    UnitPrice = b.sale_price,
                    Quantity = 1
                });
            }

            return true;
        }

        // đổi số lượng
        public bool Update(int bookId, int quantity)
        {
            if (quantity <= 0)
                return false;

            var item = Items.FirstOrDefault(
                x => x.BookID == bookId);

            if (item == null)
                return false;

            item.Quantity = quantity;
            return true;
        }

        // xóa 1 sp
        public void Remove(int bookId)
        {
            Items.RemoveAll(x => x.BookID == bookId);
        }

        // xóa hết giỏ
        public void Clear()
        {
            Items.Clear();
        }

        // tổng số lượng
        public int TotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }

        // tổng tiền
        public decimal TotalMoney()
        {
            return Items.Sum(x => x.UnitPrice * x.Quantity);
        }
    }
}
