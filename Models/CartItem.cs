
namespace SmartBook.Models
{
    [System.Serializable]
    public class CartItem
    {
        public int BookID { get; set; }

        public string BookTitle { get; set; }

        public string Image { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice
        {
            get { return UnitPrice * Quantity; }
        }
    }
}
