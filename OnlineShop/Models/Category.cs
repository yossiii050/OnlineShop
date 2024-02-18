using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
	public class Category
	{
		public int Id { get; set; }
        [Required]
        public string Name { get; set; }
		public ICollection<Item>? Items { get; set; }
	}
}
