using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Cart;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DBProjectContext _db;
        public CartController(DBProjectContext db)
        {
            _db = db;
        }

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

            // Retrieve the product details from the database
            var product = _db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Add the product to the cart
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = id,
                    ProductName = product.Name,  // Set the product name
                    ProductPrice = product.Price,       // Set the product price
                    Quantity = quantity
                });
            }

            // Save the cart back to the session
            HttpContext.Session.SetObject("Cart", cart);

            return RedirectToAction("DisplayCart", "Cart");
        }


        public IActionResult DisplayCart()
        {
            // Retrieve the cart from the session
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Pass the cart to the view
            return View(cart);
        }



    }
}
