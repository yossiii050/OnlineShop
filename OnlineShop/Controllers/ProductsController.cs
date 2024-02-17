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
        private readonly IConfiguration _configuration;

        public ProductsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<User> products = GetProductsFromDatabase();
            return View(products);
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("localhost");
        }

        private List<User> GetProductsFromDatabase()
        {
            List<User> products = new List<User>();
           


            

            return products;
        }
    }
}
