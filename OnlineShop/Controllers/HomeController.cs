using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.ViewModels;
using System.Data.Entity;
using System.Diagnostics;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class HomeController : BaseController
    {

        private readonly ILogger<HomeController> _logger;
        private readonly DBProjectContext _db;
        public HomeController(ILogger<HomeController> logger, DBProjectContext db) : base(db)
        {
            _logger = logger;
            _db=db;
            Console.WriteLine($"BrainTreeGate injected:");

        }

        public IActionResult Index()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Products.Include(p => p.Category),
                Categories = _db.Categories
            };
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ViewBag.UnreadMessages = _db.messages.Count(p => p.UserId == userId && !p.IsRead);
            }
            return View(homeVM);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
