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
    public class OrderController : BaseController
    {
        private DBProjectContext _db;
        public OrderController(DBProjectContext db):base(db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Order> orders = _db.Orders
                                    .Include(o => o.User) 
                                    .Include(o => o.OrderItems) 
                                    .ThenInclude(oi => oi.Product) 
                                    .ToList();

            List<OrderDetailsViewModel> orderDetailsViewModels = orders.Select(order => new OrderDetailsViewModel
            {
                UserName = order.User?.UserName ?? "Guest",
                UserNameandLname = (order.User?.FirstName ?? "Guest") + " " + (order.User?.LastName ?? ""),
                phoneNumber = order.phoneNumber,
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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                List<Order> userOrders = _db.Orders
                            .Include(o => o.User) 
                            .Where(o => o.UserId == userId)
                            .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                            .ToList();


                List<OrderDetailsViewModel> orderDetailsViewModels = userOrders.Select(order => new OrderDetailsViewModel
                {
                    UserName=order.User.UserName,
                    UserNameandLname=order.User.FirstName+order.User.LastName,
                    phoneNumber = order.phoneNumber,
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

                return View(orderDetailsViewModels);
            }
            else
            {
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
            if(order.UserId!= null )
            {
                if (newStatus==OrderStatus.Shipped)
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
                else if (newStatus==OrderStatus.Completed)
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
            }
            


            _db.SaveChanges();

            return Json(new { success = true });
        }


    }
}
