using OnlineShop.Models.Cart;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class Product
    {
		public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }
  
        public string Description { get; set; }
        
        public string Image { get; set; }
        
        private int? _discountPercentage;
        public int? DiscountPercentage
        {
            get { return _discountPercentage; }
            set { _discountPercentage = value ?? 0; }
        }

        // Calculated property to determine if the product is on sale
        [NotMapped]
        public bool IsOnSale => DiscountPercentage.HasValue && DiscountPercentage > 0;
        [NotMapped]
        public int Popularity => CartItems?.Sum(item => item.Quantity) ?? 0;

        [Range(0, int.MaxValue, ErrorMessage = "Amount must be a at least 1.")]
        public int Amount { get; set; }

        
        [Display(Name = "Category")]
        public int CategoryId { get; set; }


        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }


    }
}
