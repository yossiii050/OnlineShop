using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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


        public IActionResult Search(string search)
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = _db.Products.Include(p => p.Category).Where(p=>p.Name.Contains(search)),
                Categories = _db.Categories
            };
            if (homeVM.Products.Count() > 0) { return View("Index", homeVM); }
            else return RedirectToAction("Index");
        }

        public IActionResult Filter(string sort, int? category, DateTime? date, string priceRange, bool onSale)
        {
            var products = _db.Products.Include(p => p.Category).AsQueryable();

            if (category.HasValue)
            {
                products = products.Where(p => p.CategoryId == category);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                var priceParts = priceRange.Split('-');
                if (priceParts.Length == 2 && decimal.TryParse(priceParts[0], out decimal minPrice) && decimal.TryParse(priceParts[1], out decimal maxPrice))
                {
                    products = products.Where(p => (p.Price*(100-p.DiscountPercentage)/100) >= minPrice && (p.Price* (100 - p.DiscountPercentage) / 100) <= maxPrice);
                }
            }

            if (onSale)
            {
                products = products.Where(p => p.DiscountPercentage > 0);
            }

            switch (sort)
            {
                case "price_asc":
                    products = products.OrderBy(p => (p.Price * (100 - p.DiscountPercentage) / 100));
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => (p.Price * (100 - p.DiscountPercentage) / 100));
                    break;
                case "popular":
                    var productList = products.ToList();
                    productList = productList.OrderByDescending(p => p.Popularity).ToList();
                    products = productList.AsQueryable();
                    break;
            }

            var viewModel = new HomeVM
            {
                Products = products.ToList(),
                Categories = _db.Categories.ToList()
            };

            return View("Index", viewModel);
        }



    }
}
