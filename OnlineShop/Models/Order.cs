using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public enum OrderStatus
    {
        Accepted,
        Shipped,
        Completed,
        Cancelled
    }

    public class Order
	{
		public int Id { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalPrice { get; set; }
		
		public string UserId { get; set; }
		public User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public string ShipStreet { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public string ShipZipCode { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Accepted; 

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
