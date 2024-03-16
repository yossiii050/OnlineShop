using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Controllers
{
	public class CategoryController : Controller
	{
		private readonly DBProjectContext _context;

		public CategoryController(DBProjectContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var categories = await _context.Categories.ToListAsync();
			return View(categories);
		}

		public IActionResult AddCategory()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddCategory(Category category)
		{
			if (ModelState.IsValid)
			{
				_context.Categories.Add(category);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(category);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return View(category);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				_context.Update(category);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(category);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category != null)
			{
				_context.Categories.Remove(category);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Index));
		}
	}
}
