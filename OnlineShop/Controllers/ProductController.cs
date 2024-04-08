using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Models.Message;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineShop.Controllers
{
	public class ProductController : BaseController
    {
		private readonly DBProjectContext _context;

		public ProductController(DBProjectContext context) : base(context)
        {
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var products = await _context.Products.Include(p => p.Category).ToListAsync();
			return View(products);
		}

        public IActionResult AddProduct()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product product)
        {
            ModelState.Remove("Category");
            ModelState.Remove("CartItems");
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(product);
        }


        [HttpPost]
        [Authorize]
        public ActionResult RequestNotification2(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notification = new ProductNotification { ProductId = productId, UserId = userId };
            // Add notification to the database
            _context.ProductNotifications.Add(notification);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize] // Make sure the user is logged in
        public ActionResult RequestNotification(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
            // Check if a notification request already exists for this user and product
            var existingNotification = _context.ProductNotifications.FirstOrDefault(n => n.ProductId == productId && n.UserId == userId);
            if (existingNotification != null)
            {
                return Json(new { success = false, message = "You have already requested a notification for this product." });
            }

            // Save the new notification request to the database
            var notification = new ProductNotification { ProductId = productId, UserId = userId };
            _context.ProductNotifications.Add(notification);
            _context.SaveChanges();

            return Json(new { success = true });
        }


        public async Task<IActionResult> Edit(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
			return View(product);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit1(Product product)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                if (product.Amount > 0) // Check if the product has been restocked
                {
                    var notifications = _context.ProductNotifications.Where(n => n.ProductId == product.Id).ToList();
                    foreach (var notification in notifications)
                    {

                        var message = new MessageInbox
                        {
                            Subject = "Product Restocked: " + product.Name,
                            Content = "The product '" + product.Name + "' you were interested in is now back in stock!",
                            ReceivedTime = DateTime.Now,
                            IsRead = false,
                            UserId = notification.UserId 
                        };
                        _context.ProductNotifications.Remove(notification);
                        _context.messages.Add(message);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            return View(product);
        }


        public async Task<IActionResult> Delete(int id)
		{
			var product = await _context.Products.FindAsync(id);
			if (product != null)
			{
				_context.Products.Remove(product);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}


	}
}
