using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Cart;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private DBProjectContext _db;
        public CartController(DBProjectContext db)
        {
            _db = db;
        }


        [HttpPost]
        public IActionResult AddToCart(int id, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);             
                var cartItem = _db.CartItems.FirstOrDefault(c => c.ProductId == id && c.UserId == userId);

                var product = _db.Products.Find(id);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    _db.CartItems.Add(new CartItem
                    {
                        ProductId = id,
                        ProductName= product.Name,
                        UserId = userId,
                        ProductPrice = product.Price,
                        Quantity = quantity
                    });
                }
                product.Amount-=quantity;
                _db.SaveChanges();
            }
            else
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();

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
                product.Amount-=quantity;
                _db.SaveChanges();
                // Save the cart back to the session
                HttpContext.Session.SetObject("cart", cart);
            }

            return RedirectToAction("DisplayCart");
        }


        [HttpPost]
        public IActionResult AddToCart1(int id, int quantity)
        {
            var cartSessionKey = GetCartSessionKey();
            var cart = HttpContext.Session.GetObject<List<CartItem>>(cartSessionKey) ?? new List<CartItem>();


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
            product.Amount-=quantity;
            _db.SaveChanges();
            // Save the cart back to the session
            HttpContext.Session.SetObject(cartSessionKey, cart);

            return RedirectToAction("DisplayCart");
        }
        public IActionResult DisplayCart()
        {
            List<CartItem> cart;

            if (User.Identity.IsAuthenticated)
            {
                // For authenticated users, retrieve the cart from the database
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                cart = _db.CartItems
                          .Where(c => c.UserId == userId)
                          .Include(c => c.Product) // Include related Product data
                          .ToList();
            }
            else
            {
                // For non-authenticated users, retrieve the cart from the session
                
                cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
            }

            // Pass the cart to the view
            return View(cart);
        }

        public IActionResult DisplayCart1()
        {
            var cartSessionKey = GetCartSessionKey();
            // Retrieve the cart from the session
            var cart = HttpContext.Session.GetObject<List<CartItem>>(cartSessionKey) ?? new List<CartItem>();

            // Pass the cart to the view
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItem = _db.CartItems.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

                if (cartItem != null)
                {
                    _db.CartItems.Remove(cartItem);
                }

                var product = _db.Products.Find(productId);
                if (product != null)
                {
                    product.Amount += quantity; // Restock the product
                }

                _db.SaveChanges();
            }
            else
            {
                
                var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();

                var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);
                if (cartItem != null)
                {
                    cart.Remove(cartItem);
                    HttpContext.Session.SetObject("cart", cart); // Save the updated cart back to the session
                }

                var product = _db.Products.Find(productId);
                if (product != null)
                {
                    product.Amount += quantity; // Restock the product
                }

                _db.SaveChanges();
            }

            return RedirectToAction("DisplayCart");
        }


        private string GetCartSessionKey()
        {
            if (User.Identity.IsAuthenticated)
            {
                return $"Cart_{User.Identity.Name}";
            }
            return "Cart_Anonymous";
        }
    }
}
