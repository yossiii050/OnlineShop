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
        private readonly DBUsers _Category;

		public ProductsController(DBUsers Category)
        {
			_Category = Category;
        }

        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult DisplayCategory()
        {
			var categorys = _Category.Category.ToList();
			return View(categorys);
		}
   
    }
}
