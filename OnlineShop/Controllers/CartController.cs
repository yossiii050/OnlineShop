using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Cart;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.ViewModels;
using OnlineShop.Models.BrainTree;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Mailjet.Client.Resources;
using Braintree;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineShop.Models.Message;
//using System.Data.Entity;

namespace OnlineShop.Controllers
{
    public class CartController : BaseController
    {
        private DBProjectContext _db;
        
        private readonly IBraintreeService _braintreeService;

        public CartController(DBProjectContext db) : base(db)//, IBrainTreeGate brain)
        {
            //Console.WriteLine($"BrainTreeGate injected: {_brain != null}");
            _db = db;
            //_braintreeService = braintreeService;



        }

        public IActionResult Test()
        {
            var gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId= "q2h7nz489hsfj9r7",
                PublicKey= "9p7mxg8yjfg5vfgm",
                PrivateKey= "83f9cc7b1390a399bcc471aeaebfec5b"
            };

            TransactionRequest request = new TransactionRequest
            {
                Amount = 1000.00M,
                PaymentMethodNonce = "CreditCard",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                Console.WriteLine("Success!: " + transaction.Id);
            }
            else if (result.Transaction != null)
            {
                Transaction transaction = result.Transaction;
                Console.WriteLine("Error processing transaction:");
                Console.WriteLine("  Status: " + transaction.Status);
                Console.WriteLine("  Code: " + transaction.ProcessorResponseCode);
                Console.WriteLine("  Text: " + transaction.ProcessorResponseText);
            }
            else
            {
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    Console.WriteLine("Attribute: " + error.Attribute);
                    Console.WriteLine("  Code: " + error.Code);
                    Console.WriteLine("  Message: " + error.Message);
                }
            }
            return View();
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

        [HttpPost]
        public IActionResult CheckAvailability(int productId, int quantity)
        {
            var product = _db.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            if (quantity > product.Amount)
            {
                // Inform the user that the requested quantity is not available
                return Json(new { error = "Requested quantity is not available. Only " + product.Amount + " available." });
            }

            // Update the cart with the new quantity
            // (You will need to implement the logic to update the cart)
            UpdateQuantity(productId, quantity);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItem = _db.CartItems.FirstOrDefault(c => c.ProductId == id && c.UserId == userId);

                var product = _db.Products.Find(id);
                if (quantity>cartItem.Quantity)
                {
                    var NewQuant = quantity-cartItem.Quantity;

                    cartItem.Quantity += NewQuant;
                    product.Amount-=NewQuant;
                }
                else
                {
                    var difQuant = cartItem.Quantity-quantity;
                    cartItem.Quantity = quantity;
                    product.Amount += difQuant;
                }
                
                _db.SaveChanges();
            }
            else
            {
                var cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();

                var product = _db.Products.Find(id);


                // Add the product to the cart
                var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
                if (quantity>cartItem.Quantity)
                {
                    var NewQuant = quantity-cartItem.Quantity;

                    cartItem.Quantity += NewQuant;
                    product.Amount-=NewQuant;
                }
                else
                {
                    var difQuant = cartItem.Quantity-quantity;
                    cartItem.Quantity = quantity;
                    product.Amount += difQuant;
                }
               
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
        public IActionResult Checkout(CheckoutViewModel model, string selectedCardId,decimal PracentofDisc)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _db.Users.FirstOrDefault(u => u.Id == userId); 
                var cartItems = _db.CartItems.Where(c => c.UserId == userId).ToList();
                var cardinfo = _db.CreditCards
                    .Where(t => (t.Id).ToString() == selectedCardId)                    
                    .ToList();
                var cartItemsSum=cartItems.Sum(item => item.ProductPrice * item.Quantity);
                
                var order = new Order
                {
                    UserId = userId,
                    confirmationNumber=GenerateConfirmationNumber(),
                    fourCardNumber=cardinfo[0].fourLastNumber,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = cartItemsSum,
                    FinalPrice=cartItemsSum-cartItemsSum*PracentofDisc+25+cartItemsSum*18/100,
                    DiscountHas=PracentofDisc*100,
                    ShipStreet = model.x.ShipStreet,
                    ShipCity = model.x.ShipCity,
                    ShipCountry = model.x.ShipCountry,
                    ShipZipCode = model.x.ShipZipCode,
                    OrderItems = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.ProductPrice,
                        Name=item.ProductName
                    }).ToList()
                    
                };

                var message = new MessageInbox
                {
                    Subject = "Order No."+order.confirmationNumber,
                    Content = "your order has been successfully placed.",
                    ReceivedTime = DateTime.Now,
                    IsRead = false,
                    UserId=userId
                };
                if (user != null)
                {
                    user.Address.Street = model.x.ShipStreet;
                    user.Address.City = model.x.ShipCity;
                    user.Address.Country = model.x.ShipCountry;
                    user.Address.ZipCode = model.x.ShipZipCode;
                    _db.Users.Update(user);
                }
                _db.messages.Add(message);
                _db.Orders.Add(order);
                _db.CartItems.RemoveRange(cartItems); // Remove the cart items
                _db.SaveChanges();

                // Redirect to a confirmation page or order details page
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            else
            {
                var cartItems = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
                var cartItemsSum = cartItems.Sum(item => item.ProductPrice * item.Quantity);

                var order = new Order
                {
                    
                    UserId = null,
                    confirmationNumber=GenerateConfirmationNumber(),
                    fourCardNumber=model.CardNotRegUser.Substring(model.CardNotRegUser.Length - 4),
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = cartItemsSum,
                    FinalPrice=cartItemsSum-cartItemsSum*PracentofDisc+25+cartItemsSum*18/100,
                    DiscountHas=PracentofDisc*100,
                    ShipStreet = model.x.ShipStreet,
                    ShipCity = model.x.ShipCity,
                    ShipCountry = model.x.ShipCountry,
                    ShipZipCode = model.x.ShipZipCode,
                    OrderItems = cartItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.ProductPrice,
                        Name=item.ProductName
                    }).ToList()
                };

                _db.Orders.Add(order);
                //_db.CartItems.RemoveRange(cartItems); // Remove the cart items
                _db.SaveChanges();

                // Redirect to a confirmation page or order details page
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });

            }

            // Handle the case for non-authenticated users or add an error message
            return RedirectToAction("Index", "Home");
        }

        private string GenerateConfirmationNumber()
        {
            var random = new Random();
            var length = 10; // You can adjust the length as needed
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var confirmationNumber = new char[length];

            for (int i = 0; i < length; i++)
            {
                confirmationNumber[i] = chars[random.Next(chars.Length)];
            }

            return new string(confirmationNumber);
        }


        public IActionResult SubmitBillingInfo()
        {
            var viewModel = new CheckoutViewModel();
            List<string> countries = Models.Address.CountryList;
            ViewBag.CountryList = Models.Address.CountryList;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                viewModel.CartItems = _db.CartItems
                                        .Where(c => c.UserId == userId)
                                        .Include(c => c.Product)
                                        .ToList();
                viewModel.userCards= _db.CreditCards.Where(c => c.UserId == userId).ToList();

                return View(viewModel);

                //var gateway=_brain.GetGateway();
                // var clientToken = gateway.ClientToken.Generate();
                // ViewBag.ClientToken = clientToken;
            }
            else
            {
                viewModel.CartItems = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
                return View("SubmitBillingInfoNotRegUsers", viewModel);
                
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
            TempData["orderConf"] = order.confirmationNumber;
            return View(order);
        }

        public bool HasUserUsedPromoCode(string userId, string promoCodeName)
        {

            var result = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                 .Include(_db.UserPromoCodes, up => up.PromoCode)
                 .Any(up => up.UserId == userId && up.PromoCode.Code == promoCodeName);

            return result;
        }

        [HttpPost]
        public ActionResult ApplyPromoCode(string promoCodeCode)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (HasUserUsedPromoCode(userId, promoCodeCode))
            {
                // User has already used this promo code
                return Json(new { error = "You have already used this promo code." });
            }
            else
            {
                // Apply the promo code and save the record

                var promoCode = _db.PromoCodes.FirstOrDefault(p => p.Code == promoCodeCode && p.IsActive && p.ExpiryDate > DateTime.Now);

                if (promoCode != null)
                {
                    _db.UserPromoCodes.Add(new UserPromoCode
                    {
                        UserId = userId,
                        PromoCodeId = promoCode.Id
                    });
                    _db.SaveChanges();
                    return Json(new { discount = promoCode.DiscountPercentage/100 });
                }

                return Json(new { error = "Invalid promo code" });
            }
        }



    }
}
