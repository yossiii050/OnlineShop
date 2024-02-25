using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.OracleClient; // Add the necessary using directive
using System.Configuration;
using OnlineShop.Models; // Add the necessary using directive
using Oracle.ManagedDataAccess.Client;
using Microsoft.AspNetCore.Authorization;

namespace OnlineShop.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductsController : Controller
    {
        private readonly DBProjectContext _Category;

		public ProductsController(DBProjectContext Category)
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
