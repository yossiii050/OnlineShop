using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace OnlineShop.Models
{
    
    public class User : IdentityUser
    {
        
        //public string Id { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }

        
        //public string Email { get; set; }
        
        //public string Password { get; set; }

		public ICollection<Order>? Orders { get; set; }
        public string CreditCardNumber { get; set; }

        [NotMapped]
        public Address Address { get; set; }

    }
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}
