using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        private readonly DBProjectContext _db;

        public CartController(DBProjectContext db)
        {
            _db=db;
        }

        public IActionResult Index()
        {
            
            return View();
        }
    }
}
