using OnlineShop.Models.Cart;

namespace OnlineShop.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public Order x{ get; set; }

        public List<CreditCard> userCards { get; set; }
        
        public string CardNotRegUser { get; set; }
        public string ExpNotRegUser { get; set; }
        public string CvvdNotRegUser { get; set; }
        public string NameNotRegUser { get; set; }

    }
}
