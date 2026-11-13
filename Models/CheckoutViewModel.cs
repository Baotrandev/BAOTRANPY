using System.ComponentModel.DataAnnotations;

namespace SmartBook.Models
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^[0-9]{9,11}$",
            ErrorMessage = "Số điện thoại phải có từ 9 đến 11 chữ số.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng.")]
        public string Address { get; set; }

        public string Note { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
        public string PaymentMethod { get; set; }

        public string BankingProvider { get; set; }

        public string CardType { get; set; }

        public string CardNumber { get; set; }

        public string CardHolder { get; set; }

        public string CardExpiry { get; set; }

        public string CardCvv { get; set; }
    }
}
