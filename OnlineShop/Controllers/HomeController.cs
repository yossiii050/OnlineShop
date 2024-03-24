using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.ViewModels;
using System.Data.Entity;
using System.Diagnostics;

namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly DBProjectContext _db;
        public HomeController(ILogger<HomeController> logger, DBProjectContext db)
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
            return View(homeVM);
        }


        public IActionResult About()
        {

            return View();
           


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
