using Microsoft.AspNetCore.Identity;
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

        public List<Category> Categories { get; set; }

        private readonly UserManager<User> _userManager;

        public DashboardModel(DBProjectContext context,UserManager<User> userManager)
        {
            _context = context;
            NewProduct = new Product();
            _userManager = userManager;
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
                Products = await _context.Products.Include(p => p.Category).ToListAsync();
                Categories = await _context.Categories.ToListAsync(); // Reload categories for the view
                return Page();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToPage(); // Or redirect to the appropriate page        }

        }

        public async Task<IActionResult> OnPostAddCategoryAsync(string name)
        {
            if (!ModelState.IsValid)
            {
                // Handle the case where the model state is not valid
                // You may want to reload necessary data and return to the dashboard page
               // Categories = await _context.Categories.ToListAsync(); // Reload categories for the view
                return Page();
            }

            var category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            // Redirect to the dashboard page after adding the category
            return RedirectToPage();
        }

        public async Task<IActionResult> UpgradeUserRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle user not found
                return NotFound();
            }

            var currentRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            string targetRoleName = currentRole == "Regular" ? "Advanced" : "Admin";

            var targetRole = await _context.Roles
                .Where(r => r.Name == targetRoleName)
                .FirstOrDefaultAsync();

            if (targetRole == null)
            {
                // Handle target role not found
                return NotFound();
            }

            // Remove user from current roles
            var removeResult = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (!removeResult.Succeeded)
            {
                // Handle error
                return BadRequest();
            }

            // Add user to the new role
            var addRoleResult = await _userManager.AddToRoleAsync(user, targetRoleName);
            if (!addRoleResult.Succeeded)
            {
                // Handle error
                return BadRequest();
            }

            return RedirectToPage("/Identity/Admin/Dashboard"); // Redirect to the user management page or appropriate page
        }

    }
}
