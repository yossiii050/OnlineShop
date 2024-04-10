using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 
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
                Categories = await _context.Categories.ToListAsync();
                return Page();
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToPage(); 

        }

        public async Task<IActionResult> OnPostAddCategoryAsync(string name)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var category = new Category { Name = name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpgradeUserRoleAsync(string userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var currentRoleEntry = await _context.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == userId);

                    if (currentRoleEntry == null)
                    {
                        transaction.Rollback();
                        return Page();
                    }

                    var currentRoleName = await _context.Roles
                        .Where(r => r.Id == currentRoleEntry.RoleId)
                        .Select(r => r.Name)
                        .SingleOrDefaultAsync();

                    string newRoleName = currentRoleName == "Regular" ? "Advanced" : "Admin";

                    var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRoleName);
                    if (newRole == null)
                    {
                        transaction.Rollback();
                        return Page();
                    }
                    _context.UserRoles.Remove(currentRoleEntry);
                    await _context.SaveChangesAsync();

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
        }

        public async Task<IActionResult> OnPostDowngradeUserRoleAsync(string userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var currentRoleEntry = await _context.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == userId);

                    if (currentRoleEntry == null)
                    {
                        transaction.Rollback();
                        return Page();
                    }

                    var currentRoleName = await _context.Roles
                        .Where(r => r.Id == currentRoleEntry.RoleId)
                        .Select(r => r.Name)
                        .SingleOrDefaultAsync();

                    string newRoleName = currentRoleName == "Admin" ? "Advanced" : "Regular";

                    var newRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == newRoleName);
                    if (newRole == null)
                    {
                        transaction.Rollback();
                        return Page();
                    }

                    _context.UserRoles.Remove(currentRoleEntry);
                    await _context.SaveChangesAsync();

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
        }

    }
}
