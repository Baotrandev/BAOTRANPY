
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

        // Thêm sách vào giỏ
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

        // Cập nhật số lượng
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

        // Xóa một sản phẩm
        public void Remove(int bookId)
        {
            Items.RemoveAll(x => x.BookID == bookId);
        }

        // Xóa toàn bộ giỏ hàng
        public void Clear()
        {
            Items.Clear();
        }

        // Tổng số lượng
        public int TotalQuantity()
        {
            return Items.Sum(x => x.Quantity);
        }

        // Tổng tiền
        public decimal TotalMoney()
        {
            return Items.Sum(x => x.UnitPrice * x.Quantity);
        }
    }
}
