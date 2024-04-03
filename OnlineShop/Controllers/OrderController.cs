using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using OnlineShop.Models;
using OnlineShop.Models.Cart;
using OnlineShop.Models.Message;
using OnlineShop.Models.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class OrderController : Controller
    {
        private DBProjectContext _db;
        public OrderController(DBProjectContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Order> orders = _db.Orders
                                    .Include(o => o.User) // Include the User navigation property
                                    .Include(o => o.OrderItems) // Include OrderItems
                                    .ThenInclude(oi => oi.Product) // Include Product if needed
                                    .ToList();

            List<OrderDetailsViewModel> orderDetailsViewModels = orders.Select(order => new OrderDetailsViewModel
            {
                UserName = order.User?.UserName ?? "Guest",
                OrderId = order.Id,
                ShipStreet = order.ShipStreet,
                ShipCity = order.ShipCity,
                ShipCountry = order.ShipCountry,
                ShipZipCode = order.ShipZipCode,
                Status = order.Status,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                DiscountHas=order.DiscountHas,
                FinalPrice=order.FinalPrice,
                confirmationNumber=order.confirmationNumber,
                fourCardNumber=order.fourCardNumber,
                Items = order.OrderItems.Select(item => new OrderItemViewModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Name=item.Name
                }).ToList()
            }).ToList();

            return View(orderDetailsViewModels);
        }

        public IActionResult UserOrders()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Retrieve the user's ID.
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Retrieve the orders from the database that are associated with the current user.
                List<Order> userOrders = _db.Orders
                            .Include(o => o.User) // Include the User navigation property
                            .Where(o => o.UserId == userId)
                            .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                            .ToList();


                // Map the orders to OrderDetailsViewModel
                List<OrderDetailsViewModel> orderDetailsViewModels = userOrders.Select(order => new OrderDetailsViewModel
                {
                    UserName=order.User.UserName,
                    OrderId = order.Id,
                    ShipStreet= order.ShipStreet,
                    ShipCity= order.ShipCity,
                    ShipCountry= order.ShipCountry,
                    ShipZipCode= order.ShipZipCode,
                    Status = order.Status,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    DiscountHas=order.DiscountHas,
                    FinalPrice=order.FinalPrice,
                    confirmationNumber=order.confirmationNumber,
                    fourCardNumber=order.fourCardNumber,
                    Items = order.OrderItems.Select(item => new OrderItemViewModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Name=item.Name
                    }).ToList()
                }).ToList();

                // Pass the user's orders to the view
                return View(orderDetailsViewModels);
            }
            else
            {
                // Handle the scenario for non-authenticated users if needed.
                // You may want to redirect them to the login page or show a message.
                return RedirectToAction("Login", "Account");
            }
        }


        
        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _db.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found." });
            }

            order.Status = newStatus;

            if(newStatus==OrderStatus.Shipped)
            {
                var message = new MessageInbox
                {
                    Subject = "Order No."+order.confirmationNumber,
                    Content = "your order has been shipped.",
                    ReceivedTime = DateTime.Now,
                    IsRead = false,
                    UserId=order.UserId
                };
                _db.messages.Add(message);
            }
            else if(newStatus==OrderStatus.Completed)
            {
                var message = new MessageInbox
                {
                    Subject = "Order No."+order.confirmationNumber,
                    Content = "your order has been completed successfully .",
                    ReceivedTime = DateTime.Now,
                    IsRead = false,
                    UserId=order.UserId
                };
                _db.messages.Add(message);
            }
            else if (newStatus==OrderStatus.Cancelled)
            {
                var message = new MessageInbox
                {
                    Subject = "Order No."+order.confirmationNumber,
                    Content = "your order has been cancelled.",
                    ReceivedTime = DateTime.Now,
                    IsRead = false,
                    UserId=order.UserId
                };
                _db.messages.Add(message);
            }


            _db.SaveChanges();

            return Json(new { success = true });
        }


    }
}
