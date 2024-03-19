using OnlineShop.Models.Cart;

namespace OnlineShop.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public Order x{ get; set; }
    }
}
