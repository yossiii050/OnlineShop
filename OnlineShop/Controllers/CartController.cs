using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Cart;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.ViewModels;
using OnlineShop.Models.BrainTree;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private DBProjectContext _db;
        //private readonly IBrainTreeGate _brain;
        public CartController(DBProjectContext db)//, IBrainTreeGate brain)
        {
            //Console.WriteLine($"BrainTreeGate injected: {_brain != null}");
            _db = db;
            //_brain=brain;
            

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
                        Quantity = quantity,
                        Image= product.Image,
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
                        Quantity = quantity,
                        Image = product.Image,
                    });
                }
                product.Amount-=quantity;
                _db.SaveChanges();
                // Save the cart back to the session
                HttpContext.Session.SetObject("cart", cart);
            }

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
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItems = _db.CartItems.Where(c => c.UserId == userId).ToList();

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = cartItems.Sum(item => item.ProductPrice * item.Quantity),
                    ShipStreet = model.x.ShipStreet,
                    ShipCity = model.x.ShipCity,
                    ShipCountry = model.x.ShipCountry,
                    ShipZipCode = model.x.ShipZipCode,
                    OrderItems = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.ProductPrice
                    }).ToList()
                };

                _db.Orders.Add(order);
                _db.CartItems.RemoveRange(cartItems); // Remove the cart items
                _db.SaveChanges();

                // Redirect to a confirmation page or order details page
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            // Handle the case for non-authenticated users or add an error message
            return RedirectToAction("Index", "Home");
        }

        

        public IActionResult SubmitBillingInfo()
        {
            var viewModel = new CheckoutViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                viewModel.CartItems = _db.CartItems
                                        .Where(c => c.UserId == userId)
                                        .Include(c => c.Product)
                                        .ToList();

                //var gateway=_brain.GetGateway();
               // var clientToken = gateway.ClientToken.Generate();
               // ViewBag.ClientToken = clientToken;
            }
            else
            {
                viewModel.CartItems = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
            }

            return View(viewModel);
        }

       

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderItems)
                                  .ThenInclude(oi => oi.Product)
                                  .FirstOrDefault(o => o.Id == orderId);

            if (order == null || (User.Identity.IsAuthenticated && order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                // Handle the case where the order is not found or does not belong to the current user
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

    }
}
