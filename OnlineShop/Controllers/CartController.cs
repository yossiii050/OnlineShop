using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models.Cart;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            // Retrieve the cart from the session
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Add the product to the cart
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem { ProductId = id, Quantity = quantity });
            }

            // Save the cart back to the session
            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("Index", "Products");
        }


        public IActionResult DisplayCart()
        {
            return View();
        }


    }
}
