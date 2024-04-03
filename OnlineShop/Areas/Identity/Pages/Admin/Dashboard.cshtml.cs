using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Add this for EF Core
using OnlineShop;
using OnlineShop.Models; 

namespace OnlineShop.Areas.Identity.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly DBProjectContext _context;

        public Product NewProduct { get; set; }

        public List<Product> Products { get; set; }

        public List<User> Users { get; set; }

        public List<Category> Categories { get; set; } // To hold the categories for the dropdown

        public DashboardModel(DBProjectContext context)
        {
            _context = context;
            NewProduct = new Product(); 
        }

     
        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
            Categories = await _context.Categories.ToListAsync();
            Products = await _context.Products.Include(p => p.Category).ToListAsync();

        }

        public async Task<IActionResult> OnPostAsync(Product product)
        {
            ModelState.Remove("Category");
            ModelState.Remove("CartItems");

            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories.ToListAsync(); // Reload categories for the view
                return Page();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToPage(); // Or redirect to the appropriate page        }

        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Load the categories for the dropdown to repopulate the form.
            // You would typically pass the product to a form to edit it.
            Categories = await _context.Categories.ToListAsync();

            // You would need to have a way to show the edit form and populate it with the product details.
            // This could be done using a Partial View or a Modal Dialog.

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            // After deletion, redirect to the Dashboard page to refresh the product list.
            return RedirectToPage();
        }
    }
}
