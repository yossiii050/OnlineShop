using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

using OnlineShop.Models;
using OnlineShop.Models.Cart;
using System.Data.Entity;

namespace OnlineShop.Controllers
{
    public class OrderController : Controller
    {
        private DBProjectContext _db;
        public OrderController(DBProjectContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _db.Orders.ToListAsync();
            return View(orders);
            
        }

        public IActionResult UserOrders() 
        {
            return View();
        }
    }
}
