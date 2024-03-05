using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Add this for EF Core
using OnlineShop;
using OnlineShop.Models; // Replace with the actual namespace of your DbContext

namespace OnlineShop.Areas.Identity.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly DBProjectContext _context; // Replace YourDbContext with your actual DbContext type

        // Property to hold user data
        public List<User> Users { get; set; } // Replace ApplicationUser with your user entity type

        // Inject your database context
        public DashboardModel(DBProjectContext context)
        {
            _context = context;
        }

        // Asynchronous method to load data when the page is accessed
        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync(); // Load users from the database

             
        }
            
    }
}
