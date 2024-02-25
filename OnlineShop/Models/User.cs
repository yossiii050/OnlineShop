using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class User : IdentityUser
    {
        
        //public string Id { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [EmailAddress]
        //public string Email { get; set; }
        
        //public string Password { get; set; }

		public ICollection<Order>? Orders { get; set; }

	}
}
