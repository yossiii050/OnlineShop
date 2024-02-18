using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.OracleClient; // Add the necessary using directive
using System.Configuration;
using OnlineShop.Models; // Add the necessary using directive
using Oracle.ManagedDataAccess.Client;

namespace OnlineShop.Controllers
{
    public class ProductsController : Controller
    {
        

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult DisplayProducts()
        {
			var users = _users.User.ToList();
			return View(users);
		}
   
    }
}
