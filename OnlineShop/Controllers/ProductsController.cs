using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.OracleClient; // Add the necessary using directive
using System.Configuration;
using OnlineShop.Models; // Add the necessary using directive
using Oracle.ManagedDataAccess.Client;

using Microsoft.AspNetCore.Authorization;

using System.Diagnostics;


namespace OnlineShop.Controllers
{
    
    public class ProductsController : Controller
    {
        private readonly DBProjectContext _Category;

		public ProductsController(DBProjectContext Category)
        {
			_Category = Category;
        }
        
        public IActionResult Index()
        {
            IEnumerable<Product> objlist = _Category.Product;

            return View(objlist);
        }

        public IActionResult AddProduct()
        {
            ViewBag.Categories = _Category.Category.ToList();
            return View();
        }


        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < product.Quantity; i++)
                {
                    var newProduct = new Product
                    {
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Image = product.Image,
                        CategoryId = product.CategoryId
                    };

                    _Category.Product.Add(newProduct);
                }

                _Category.SaveChanges();
                return RedirectToAction("DisplayProducts");
            }

            ViewBag.Categories = _Category.Category.ToList(); // Repopulate categories
            return View(product);
        }


        public IActionResult DisplayProducts()
        {
            var products = _Category.Product
        .GroupBy(p => new { p.Name, p.Description, p.Price, p.Image })
        .Select(g => new Product
        {
            Id = g.FirstOrDefault().Id,
            Name = g.Key.Name,
            Description = g.Key.Description,
            Price = g.Key.Price,
            Image = g.Key.Image,
            Quantity = g.Sum(p => p.Quantity)
        })
        .ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = _Category.Product.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _Category.Product.Remove(product);
            _Category.SaveChanges();

            return RedirectToAction("DisplayProducts");
        }


        public IActionResult DisplayCategory()
        {
			var categorys = _Category.Category.ToList();
			return View(categorys);
		}

        public IActionResult AddCategory()
        {
            return View("AddCategory");
        }

        [HttpPost]
        public IActionResult AddCategory(Category category) 
        {
            if (!ModelState.IsValid)
            {
                return View("AddCategory");
            }
            try
            {
                _Category.Category.Add(category);
                _Category.SaveChanges();
                TempData["msg"]="Added successfully";
                return RedirectToAction("DisplayCategory");
            }
            catch (Exception)
            {
                TempData["msg"]="Could not added!";
                return View("AddCategory");
            }
        }

        public IActionResult EditCategory(int id)
        {
            Category category = _Category.Category.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                _Category.Category.Update(category);
                _Category.SaveChanges();
                return RedirectToAction("DisplayCategory");
            }
            catch (Exception)
            {
                TempData["msg"]="Could not Update!";
                return View();
            }
        }
        
        public IActionResult DeleteCategory(int Id) 
        {
            try
            {
                var cat_dl = _Category.Category.Find(Id);
                if (cat_dl!=null)
                {
                    _Category.Category.Remove(cat_dl);
                    _Category.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("DisplayCategory");
        }
    }

}
