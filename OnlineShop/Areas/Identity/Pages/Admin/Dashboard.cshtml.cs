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

        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }


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

        public async Task<IActionResult> OnPostUpgradeUserRoleAsync(string userId)
        {
            var userRole = await _context.UserRoles
                                          .FirstOrDefaultAsync(ur => ur.UserId == userId);
            if (userRole == null)
            {
                // Handle the case where the user role is not found
                return Page();
            }

            // Fetch the current role name
            var currentRole = await _context.Roles
                                            .Where(r => r.Id == userRole.RoleId)
                                            .Select(r => r.Name)
                                            .FirstOrDefaultAsync();

            // Determine the new role based on the current role
            string newRoleName = currentRole == "Regular" ? "Advanced" : "Admin";
            var newRole = await _context.Roles
                                        .FirstOrDefaultAsync(r => r.Name == newRoleName);
            if (newRole == null)
            {
                // Handle the case where the new role is not found
                return Page();
            }

            // Update the user role
            userRole.RoleId = newRole.Id;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
