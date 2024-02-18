﻿namespace OnlineShop.Models
{
	public class Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; } // Added Quantity property
		public int CategoryId { get; set; }
		public Category Category { get; set; }
	}
}
