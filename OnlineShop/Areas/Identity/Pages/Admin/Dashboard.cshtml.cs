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
            // Find the existing role for the user
            var currentRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (currentRole == null)
            {
                // Handle the case where the role is not found
                return Page();
            }

            var newRole1 = await _context.Roles
                            .FirstOrDefaultAsync(r => r.Name == "Advanced");
            if (newRole1 == null)
            {
                // Handle the situation where the role does not exist.
            }

            // Determine the new role ID based on the current role
            var newRoleId = (currentRole.RoleId == "RegularRoleId") ? "AdvancedRoleId" : "AdminRoleId";

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Remove the existing role
                    _context.UserRoles.Remove(currentRole);
                    await _context.SaveChangesAsync();

                    // Add the new role
                    var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Advanced");
                    if (newRole == null)
                    {
                        // The role doesn't exist in AspNetRoles table, handle the error
                        transaction.Rollback();
                        return Page();
                    }

                    var newUserRole = new IdentityUserRole<string>
                    {
                        UserId = userId,
                        RoleId = newRole.Id
                    };
                    _context.UserRoles.Add(newUserRole);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return RedirectToPage();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return RedirectToPage();
        }
    }
}
