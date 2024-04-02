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


        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Products.Add(NewProduct);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // Redirect to the dashboard page to refresh the content
        }

    }
}
