using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalPrice { get; set; }
		
		public string UserId { get; set; }
		public User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }

    }

    public class OrderItem
    {
        public int Id { get; set; }
      
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
