using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using OnlineShop.Models;
using OnlineShop.Models.Cart;

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
            List<Order> orders = _db.Orders.ToList();
            foreach (var order in orders)
            {
                _db.Entry(order).Collection(o => o.OrderItems).Load();
            }
            List<OrderDetailsViewModel> orderDetailsViewModels = orders.Select(order => new OrderDetailsViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Items = order.OrderItems?.Select(item => new OrderItemViewModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList() ?? new List<OrderItemViewModel>()
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
                                            .Where(o => o.UserId == userId) // Filter by User ID
                                            .Include(o => o.OrderItems) // Include related OrderItems data
                                            .ThenInclude(oi => oi.Product) // Include Product data if needed
                                            .ToList();

                // Map the orders to OrderDetailsViewModel
                List<OrderDetailsViewModel> orderDetailsViewModels = userOrders.Select(order => new OrderDetailsViewModel
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Items = order.OrderItems.Select(item => new OrderItemViewModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        //ProductName = item.Product.Name // Assuming you have a Product.Name property
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





    }
}
