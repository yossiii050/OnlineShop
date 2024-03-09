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

       
        public List<User> Users { get; set; } 

        // Inject your database context
        public DashboardModel(DBProjectContext context)
        {
            _context = context;
        }

     
        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync(); 

             
        }

            
    }
}
