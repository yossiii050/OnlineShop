using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class User
    {
        
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
