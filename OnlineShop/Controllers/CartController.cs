﻿using Microsoft.AspNetCore.Mvc;
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

namespace OnlineShop.Controllers
{
    public class CartController : BaseController
    {
        private DBProjectContext _db;
        private readonly IAESSettings _aesSettings;

        private readonly IBraintreeService _braintreeService;

        public CartController(DBProjectContext db, IAESSettings aesSettings) : base(db)
        {
            _db = db;
            _aesSettings=aesSettings;
        }

    /*    public IActionResult Test()
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
        }*/

        

    

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
                        ProductPrice = product.IsOnSale ? product.Price * (100 - product.DiscountPercentage.Value) / 100 : product.Price,
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
                        ProductName = product.Name, 
                        ProductPrice = product.IsOnSale ? product.Price * (100 - product.DiscountPercentage.Value) / 100 : product.Price,      
                        Quantity = quantity,
                        Image = product.Image,
                    });
                }
                product.Amount-=quantity;
                _db.SaveChanges();
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
                return Json(new { error = "Requested quantity is not available. Only " + product.Amount + " available." });
            }

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
                HttpContext.Session.SetObject("cart", cart);
            }

            return RedirectToAction("DisplayCart");
        }


        public IActionResult DisplayCart()
        {
            List<CartItem> cart;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                cart = _db.CartItems
                          .Where(c => c.UserId == userId)
                          .Include(c => c.Product) 
                          .ToList();
            }
            else
            {
                
                cart = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
            }

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
                    product.Amount += quantity; 
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
                    HttpContext.Session.SetObject("cart", cart); 
                }

                var product = _db.Products.Find(productId);
                if (product != null)
                {
                    product.Amount += quantity; 
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
                    CreditCardUser=cardinfo[0],
                    TotalPrice = cartItemsSum,
                    FinalPrice=cartItemsSum-cartItemsSum*PracentofDisc+25+cartItemsSum*18/100,
                    DiscountHas=PracentofDisc*100,
                    ShipStreet = model.x.ShipStreet,
                    phoneNumber = model.x.phoneNumber,
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
                    user.PhoneNumber = model.x.phoneNumber;
                    
                    _db.Users.Update(user);
                }
                _db.messages.Add(message);
                _db.Orders.Add(order);
                _db.CartItems.RemoveRange(cartItems);
                _db.SaveChanges();
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            else
            {
                byte[] key = Convert.FromBase64String(_aesSettings.Key);
                byte[] iv = Convert.FromBase64String(_aesSettings.IV);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                byte[] encryptedCardNumber = OnlineShop.Utillity.EncryptionHelper.EncryptStringToBytes_Aes(model.CardNotRegUser, key, iv);
                byte[] encryptedCVV = OnlineShop.Utillity.EncryptionHelper.EncryptStringToBytes_Aes(model.CvvdNotRegUser, key, iv);
                var cardinfo = new Models.CreditCard
                {
                    EncryptedCardNumber = encryptedCardNumber,
                    EncryptedExpirationDate=model.ExpNotRegUser,
                    EncryptedCVV = encryptedCVV,
                    UserId=userId,
                    NameCardOwner=model.NameNotRegUser,
                    fourLastNumber=model.CardNotRegUser.Substring(model.CardNotRegUser.Length - 4)
                };
                _db.CreditCards.Add(cardinfo);
                _db.SaveChanges();
                var cartItems = HttpContext.Session.GetObject<List<CartItem>>("cart") ?? new List<CartItem>();
                var cartItemsSum = cartItems.Sum(item => item.ProductPrice * item.Quantity);

                var order = new Order
                {
                    UserId = null,
                    confirmationNumber=GenerateConfirmationNumber(),
                    fourCardNumber=model.CardNotRegUser.Substring(model.CardNotRegUser.Length - 4),
                    OrderDate = DateTime.UtcNow,
                    CreditCardUser=cardinfo,
                    TotalPrice = cartItemsSum,
                    FinalPrice=cartItemsSum-cartItemsSum*PracentofDisc+25+cartItemsSum*18/100,
                    DiscountHas=PracentofDisc*100,
                    ShipStreet = model.x.ShipStreet,
                    phoneNumber = model.x.phoneNumber,
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
                _db.SaveChanges();

                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });

            }

            return RedirectToAction("Index", "Home");
        }

        private string GenerateConfirmationNumber()
        {
            var random = new Random();
            var length = 10;
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
            if (userId == null)
            {
                var promoCode = _db.PromoCodes.FirstOrDefault(p => p.Code == promoCodeCode && p.IsActive && p.ExpiryDate > DateTime.Now);
                if( promoCode != null)
                {
                    return Json(new { discount = promoCode.DiscountPercentage / 100 });

                }
                return Json(new { error = "Invalid promo code" });

            }

            if (HasUserUsedPromoCode(userId, promoCodeCode))
            {
                return Json(new { error = "You have already used this promo code." });
            }
            else
            {

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
